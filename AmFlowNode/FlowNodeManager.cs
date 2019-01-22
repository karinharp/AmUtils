using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace am
{
    
public class FlowNodeManager : MonoBehaviour, IFlowEventHandler
{
    
    [SerializeField]
    protected FlowNode m_flowNodePf;

    [SerializeField]
    protected bool m_isLoop; // フローが最後まで回った時に、初期Flowをに戻るか。
    
    //[SerializeField]
    protected FlowNode m_flowNode = null;
    public FlowNode flowNode {
	get {
	    if((m_flowNode == null) && (m_flowNodePf != null)){ m_flowNode = Instantiate(m_flowNodePf) as FlowNode; }
	    return m_flowNode;
	}
    }
    
    public virtual void OnRecieveFlowEvent(FlowEvent evt){
	
	//Debug.Log("OnRecieveFlowEvent : " + evt.data.type.ToString());	
	if(evt.data.type == FlowEvent.Type.CALL_FUNC){
	    try {
		var t = this.GetType();
		MethodInfo method = t.GetMethod(evt.data.funcName);
		var dg = (Action<FlowEvent.Data>)Delegate.CreateDelegate(typeof(Action<FlowEvent.Data>), this, method);
		dg(evt.data);
	    }
	    catch(Exception ex){
		Debug.Log("[" + evt.data.funcName + "] >> Func Not Found.");
		Debug.Log(ex.ToString());
	    }
	}	
    }

    public virtual async Task StartFlow(){
	Debug.Log("Start Flow");
	while(flowNode != null){
	    flowNode.listener = this.gameObject;
	    m_flowNode = await flowNode.Start();
	    if((m_flowNode == null) && (! m_isLoop)){ break; } 
	};
	Debug.Log("End Flow");	
    }

    protected virtual void OnDisable(){
	if(m_flowNode != null){
	    m_flowNode.RecieveAbortHandler();
	    m_isLoop = false; // StartFlowが1Loopまわってしまうため
	}
    }

    [Preserve]
    public void QuitApplication(FlowEvent.Data data){
#if UNITY_EDITOR
	UnityEditor.EditorApplication.isPlaying = false;
#else
	Application.Quit();
#endif	
    }
    
    /*
     * 継承先のFlowNodeManagerで実装する
     * ==================================================================    
     public void UnLockUI(FlowEvent.Data data){ uiLock.UnLock(); }
     * ==================================================================
     */
	
}
}
