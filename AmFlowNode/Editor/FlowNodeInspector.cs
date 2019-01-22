using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace am
{

[CustomEditor(typeof(FlowNode))]
public class FlowNodeInspector : InspectorBase<FlowNode>
{
    static bool s_foldingTrigger  = false;
    static bool s_foldingParam    = false;
    static bool s_foldingNextNode = false;
    static bool s_foldingProc     = false;
    
    protected override void DrawCustomInspector(){
	var fo = target as FlowNode;

	GUILayout.Space(5);		
	DrawSimpleLabelField("Basic Info", "", EditorStyles.boldLabel);
	DrawSimpleTextField(fo, "FlowLabel", ref fo.flowLabel);
	GUILayout.Space(10);
	DrawSimpleLabelField("Params", "", EditorStyles.boldLabel);
	GUILayout.Space(10);
	
	if(s_foldingProc = Foldout(s_foldingProc, "Flow Proc")){
	    DrawList<FlowNode.Proc>(fo, fo.procList, "Flow List");
	}
	GUILayout.Space(5);
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(8f));
	GUILayout.Space(5);	     	
	if(s_foldingNextNode = Foldout(s_foldingNextNode, "Flow NextNode")){
	    DrawList<FlowNode.NextNodeDef>(fo, fo.nextFlowList, "NextNode List");
	}	
	GUILayout.Space(5);
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(8f));
	GUILayout.Space(5);		
	if(s_foldingTrigger = Foldout(s_foldingTrigger, "Flow Triggers")){
	    DrawList<FlowNode.Trigger>(fo, fo.triggerList, "Trigger List");
	}
	GUILayout.Space(5);
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(8f));
	GUILayout.Space(5);	
	if(s_foldingParam = Foldout(s_foldingParam, "Flow Params")){
	    DrawList<FlowNode.Param>(fo, fo.paramList, "Param List");
	}
	GUILayout.Space(5);
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(8f));
    }

    protected override void DrawListNode<NodeT>(NodeT node){
	if     (node is FlowNode.Proc)       { var _node = node as FlowNode.Proc;        _DrawListNode(_node); }
	else if(node is FlowNode.ProcAsset)  { var _node = node as FlowNode.ProcAsset;   _DrawListNode(_node); }
	else if(node is FlowNode.Param)      { var _node = node as FlowNode.Param;       _DrawListNode(_node); }
	else if(node is FlowNode.Trigger)    { var _node = node as FlowNode.Trigger;     _DrawListNode(_node); }
	else if(node is FlowNode.NextNodeDef){ var _node = node as FlowNode.NextNodeDef; _DrawListNode(_node); }
    }
    
    void _DrawListNode(FlowNode.Param node){
	var fo = target as FlowNode;
	EditorGUILayout.BeginHorizontal();
	{	    	
	    DrawSimpleTextField(fo, "Key", ref node.key, 120f);
	    DrawSimpleEnumField(fo, "Type", ref node.type, 50f);
	}
	EditorGUILayout.EndHorizontal();	   	
	switch(node.type){
	    case FlowEvent.ArgType.DYNAMIC:
		DrawSimpleLabelField("YOU MUST USE OTHER TYPE !", "", EditorStyles.boldLabel, 320f);
		break;
	    case FlowEvent.ArgType.INT:
		DrawSimpleIntField(fo, "Default Int Value", ref node.num, 120f);
		break;
	    case FlowEvent.ArgType.STR:
		DrawSimpleTextField(fo, "Default String Value", ref node.str, 120f);			
		break;
	    case FlowEvent.ArgType.BOOL:
		DrawSimpleBoolField(fo, "Default Bool Value", ref node.b, 120f);
		break;
	    case FlowEvent.ArgType.LINE:
		DrawSimpleTextAreaField(fo, "Default Line Value", ref node.str, 40f, 120f);			
		break;
	    case FlowEvent.ArgType.NONE:
	    default:
		break;
	}
    }
    void _DrawListNode(FlowNode.Trigger node){
	var fo = target as FlowNode;
	DrawSimpleTextField(fo, "Key", ref node.key, 120f);
    }
    void _DrawListNode(FlowNode.NextNodeDef node){
	var fo = target as FlowNode;
	DrawSimpleObjectField(fo, "Next FlowNode", ref node.node, 120f);
	DrawSimpleBoolField(fo, "Is Selected (default)", ref node.isSelected, 120f);
    }
    void _DrawListNode(FlowNode.Proc node){
	var fo = target as FlowNode;

	DrawSimpleLabelField("Process List", "", EditorStyles.boldLabel);
	GUILayout.Space(3);	
	EditorGUI.indentLevel++;
	DrawList<FlowNode.ProcAsset>(fo, node.procList, "Proc List");
	EditorGUI.indentLevel--;	
	GUILayout.Space(5);

	EditorGUILayout.BeginHorizontal();
	{	
	    DrawSimpleEnumField(fo, "*Seek Condition", ref node.seekCond, 100f);
	    if(node.seekCond == FlowNode.SeekCondition.TIME_WAIT){
		DrawSimpleIntField(fo, "WaitMsec", ref node.seekWaitTimeMsec, 100f);
	    }
	    else if(node.seekCond == FlowNode.SeekCondition.TRIGGER){
		DrawSimpleTextField(fo, "Trigger Key", ref node.seekWaitTriggerKey, 100f);
	    }
	}
	EditorGUILayout.EndHorizontal();

	EditorGUILayout.BeginHorizontal();
	{
	    DrawSimpleBoolField(fo, "*Is Loop", ref node.isLoop, 100f);
	    if(node.isLoop){
		DrawSimpleTextField(fo, "Break Key", ref node.loopBreakTriggerKey, 100f);	    
	    }
	}
	EditorGUILayout.EndHorizontal();

    }
    void _DrawListNode(FlowNode.ProcAsset node){
	var fo = target as FlowNode;
	
	DrawSimpleEnumField(fo, "Proc Type", ref node.type, 120f);
	if(node.type == FlowNode.ProcType.GOTO_NEXT_SCENE){
	    EditorGUILayout.BeginHorizontal();
	    {
		DrawSimpleBoolField(fo, "Use Param", ref node.arg.b, 80f);
		if(node.arg.b){
		    DrawSimpleTextField(fo, "Param Key", ref node.arg.str, 100f);
		}
		else {
		    DrawSimpleTextField(fo, "Scene Name", ref node.arg.str, 100f);
		}
	    }
	    EditorGUILayout.EndHorizontal();	   
	}
	else if(node.type == FlowNode.ProcType.LOG){
	    EditorGUILayout.BeginHorizontal();
	    {
		DrawSimpleTextField(fo, "Msg", ref node.arg.str, 70f);
	    }
	    EditorGUILayout.EndHorizontal();	   
	}
 	else if(node.type == FlowNode.ProcType.DISPATCH_FLOW_EVENT){
	    
	    EditorGUILayout.BeginHorizontal();
	    {	    
		DrawSimpleEnumField(fo, "Event Type", ref node.arg.type, 120f);
		DrawSimpleEnumField(fo, "Arg Type", ref node.arg.argType, 70f);
	    }
	    EditorGUILayout.EndHorizontal();	   
	    
	    if(node.arg.type == FlowEvent.Type.CALL_FUNC){
		DrawSimpleTextField(fo, "Func Name", ref node.arg.funcName, 120f);		
		switch(node.arg.argType){
		    case FlowEvent.ArgType.DYNAMIC:
			DrawSimpleTextField(fo, "Param Key", ref node.arg.str, 120f);
			break;
		    case FlowEvent.ArgType.INT:
			DrawSimpleIntField(fo, "Arg (int)", ref node.arg.num, 120f);
			break;
		    case FlowEvent.ArgType.STR:
			DrawSimpleTextField(fo, "Arg (string)", ref node.arg.str, 120f);			
			break;
		    case FlowEvent.ArgType.BOOL:
			DrawSimpleBoolField(fo, "Arg (bool)", ref node.arg.b, 120f);
			break;
		    case FlowEvent.ArgType.LINE:
			DrawSimpleTextAreaField(fo, "Arg (line)", ref node.arg.str, 40f, 120f);			
			break;
		    case FlowEvent.ArgType.NONE:
		    default:
			break;			
		}		
	    }	    
	}	
    }
}
}    

/*
 * Local variables:
 * compile-command: ""
 * End:
 */
