using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;
using System.Net.Http;

namespace am
{

[Serializable, CreateAssetMenu( fileName = "FlowNode", menuName = "FlowNode/Default", order = 1000 )]
public class FlowNode : ScriptableObject
{
    /*==================================================================*/

    public string     flowLabel = "Default";
	
    /*==================================================================*/
    
    public enum SeekCondition : int {
	NODELAY     = 0,
	FRAME_WAIT  = 1,
	TIME_WAIT   = 2,
	ANY_TOUCH   = 3,
	TRIGGER     = 4,
    }
    
    public enum ProcType : int {
	NOOP                = 0,	
	GOTO_NEXT_SCENE     = 1,	
	DISPATCH_FLOW_EVENT = 2,	
	LOG                 = 3,	
    }
    
    [Serializable]
    public class ProcAsset {
	public ProcType       type;
	public FlowEvent.Data arg;	
    }
    
    [Serializable]
    public class Proc {
	public List<ProcAsset> procList = new List<ProcAsset>();
	public SeekCondition   seekCond;
	public int             seekWaitTimeMsec   = 0;
	public string          seekWaitTriggerKey = "";
	public bool            isLoop = false;
	public string          loopBreakTriggerKey = "";
    }

    [Serializable]
    public class NextNodeDef {
	public FlowNode node;
	public bool     isSelected;
    }
    
    public List<Proc>        procList     = new List<Proc>();
    public List<NextNodeDef> nextFlowList = new List<NextNodeDef>();            

    public GameObject listener { get; set; } = null;
    public bool       isAbort  { get; set; } = false;
    public Action     fInternalAbortProcess { get; set; } = null;
    public Action     fInternalTermProcess { get; set; } = null;
    
    /*==================================================================*/
    
    [Serializable]
    public class Trigger {
	public string key;
	public bool   isOn;
    }
    [Serializable]
    public class Param {
	public FlowEvent.ArgType type;
	public string key;
	public int    num;
	public string str;
	public bool   b;
    }
    public List<Trigger> triggerList = new List<Trigger>();
    public List<Param>   paramList = new List<Param>();
    
    protected bool m_anyTouch = false;
    public void RecieveAnyTouchHandler(){ m_anyTouch = true; }
    public void RecieveAbortHandler(){ isAbort = true; }
    public string nextFlowAfterAbort { get; set; } = ""; // Abortされた時に強制セットする次のFlowLabel
    public void RecieveTriggerHandler(string key){
	var trigger = triggerList.FirstOrDefault(_p => (_p.key == key));
	if(trigger != null){ trigger.isOn = true; }
    }
    public void RecieveParamHandler(string key, string value){
	var param = paramList.FirstOrDefault(_p => (_p.key == key));
	if(param != null){ param.str = value; }
	//Debug.Log("RecieveParamHandler : " + value);
    }
    public void RecieveParamHandler(string key, int value){
	var param = paramList.FirstOrDefault(_p => (_p.key == key));
	if(param != null){ param.num = value; }
	//Debug.Log("RecieveParamHandler : " + value);
    }
    public void RecieveParamHandler(string key, bool value){
	var param = paramList.FirstOrDefault(_p => (_p.key == key));
	if(param != null){ param.b = value; }
	//Debug.Log("RecieveParamHandler : " + value);
    }
    // あまりによく使うので、ショートカット定義
    public void BreakLoop(string key){
	var param = paramList.FirstOrDefault(_p => (_p.key == key));
	if(param != null){ param.b = true; }
    }
    public void SetNextFlow(string label){
	foreach(var flow in nextFlowList){
	    if(flow.node.flowLabel == label){ flow.isSelected = true; }
	    else                            { flow.isSelected = false; }
	}
    }

    protected int m_waitPollTriggerIntervalMsec = 33;

    /*==================================================================*/

    FlowEvent.Data m_dispatchEventArg = new FlowEvent.Data();
    
    public async Task<FlowNode> Start(){
	NextNodeDef nextFlowDef = null;
	
	for(int seek = 0; seek < procList.Count; ++seek){

	    //Debug.Log("ProcSeek : " + seek.ToString());
	    
	    var node = procList[seek];
	    
	    foreach(var proc in node.procList){
		
		//Debug.Log("> " + proc.type.ToString());
		
		switch(proc.type){
		    case ProcType.NOOP: break;
		    case ProcType.LOG:  Debug.Log("FlowNodeLog : " + proc.arg.str); break;
		    case ProcType.GOTO_NEXT_SCENE:
			if(proc.arg.b){
			    var param = paramList.FirstOrDefault(_p => (_p.key == proc.arg.str));
			    SceneManager.LoadScene(param.str);
			}
			else {
			    SceneManager.LoadScene(proc.arg.str);
			}			
			return null;			
		    case ProcType.DISPATCH_FLOW_EVENT:
			if(listener != null){
			    m_dispatchEventArg.funcName = proc.arg.funcName;
			    switch(proc.arg.argType){
				case FlowEvent.ArgType.DYNAMIC:
				{
				    var p = paramList.FirstOrDefault(_p => (_p.key == proc.arg.str));
				    switch(p.type){
					case FlowEvent.ArgType.INT:  m_dispatchEventArg.num = p.num; break;
					case FlowEvent.ArgType.STR:  m_dispatchEventArg.str = p.str; break;
					case FlowEvent.ArgType.BOOL: m_dispatchEventArg.b   = p.b;   break;
					case FlowEvent.ArgType.LINE: m_dispatchEventArg.str = p.str; break;
					case FlowEvent.ArgType.NONE:
					default: break;					    
				    }
				}
				break;
				case FlowEvent.ArgType.INT:     m_dispatchEventArg.num = proc.arg.num; break;
				case FlowEvent.ArgType.STR:     m_dispatchEventArg.str = proc.arg.str; break;
				case FlowEvent.ArgType.BOOL:    m_dispatchEventArg.b   = proc.arg.b;   break;
				case FlowEvent.ArgType.LINE:    m_dispatchEventArg.str = proc.arg.str; break;
				case FlowEvent.ArgType.NONE:
				default: break;					    
			    }
			    
			    ExecuteEvents.Execute<IFlowEventHandler>
				(
				 target: listener,
				 eventData: new FlowEvent(EventSystem.current){ data = m_dispatchEventArg },
				 functor: (_target, eventData) => _target.OnRecieveFlowEvent(eventData as FlowEvent)
				 );
			    if(m_dispatchEventArg.task != null){
				await m_dispatchEventArg.task;
				m_dispatchEventArg.task = null;			       
			    }
			}
			break;
		}
	    }

	    //Debug.Log("CheckCond : " + node.seekCond.ToString());
	    
	    switch(node.seekCond){		
		case SeekCondition.NODELAY:                                             break;
		case SeekCondition.FRAME_WAIT: await Task.Delay(16);                    break;
		case SeekCondition.TIME_WAIT:  await Task.Delay(node.seekWaitTimeMsec); break;
		case SeekCondition.ANY_TOUCH:
		    m_anyTouch = false;
		    while((!m_anyTouch) && (!isAbort)){ await Task.Delay(m_waitPollTriggerIntervalMsec); }
		    break;
		case SeekCondition.TRIGGER:
		    var trigger = triggerList.FirstOrDefault(_p => (_p.key == node.seekWaitTriggerKey));
		    if(trigger == null){
			Debug.Log("Trigger Not Found. >> Force Skip.");
			break;
		    }
		    trigger.isOn = false;
		    while((!trigger.isOn)  && (!isAbort)){ await Task.Delay(m_waitPollTriggerIntervalMsec); }
		    break;
	    }
	    if(isAbort){ break; }
	    if(node.isLoop){
		var param = paramList.FirstOrDefault(_p => (_p.key == node.loopBreakTriggerKey));
		if((param != null) && (! param.b)){ --seek; }
	    }
	}
	
	if(isAbort){
	    // Abort時に通常は全てを解除する（特にセットされてなければ全解除になるはず）
	    SetNextFlow(nextFlowAfterAbort);
	    if(fInternalAbortProcess != null){ fInternalAbortProcess(); }
	}
	else {
	    if(fInternalTermProcess != null){ fInternalTermProcess(); }
	}
	nextFlowDef = nextFlowList.FirstOrDefault(_p => (_p.isSelected));	
	

	// if(nextFlowDef == null){ Debug.Log("Check NextFlow : NULL"); }
	// else                   { Debug.Log("Check NextFlow : " + nextFlowDef.node.flowLabel); }
	
	return (nextFlowDef == null) ? null : GameObject.Instantiate(nextFlowDef.node) as FlowNode;
    }
    
}
}

/*
 * Local variables:
 * compile-command: ""
 * End:
 */
