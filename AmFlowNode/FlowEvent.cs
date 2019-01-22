using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace am
{
    
public class FlowEvent : BaseEventData {

    public enum Type {
	CALL_FUNC,	
    }

    public enum ArgType {
	DYNAMIC,
	INT,
	STR,
	BOOL,
	LINE,
	NONE,
    }
    
    [Serializable]
    public class Data {
	public Type         type;
	public string       funcName = "";
	public ArgType      argType  = ArgType.INT;
	public int          num      = 0;
	public string       str      = "";
	public bool         b        = false;
	// リフレクションで呼び出した先でTaskを作る場合はここにセットされる。
	public Task         task     = null;
    }

    public Data data { get; set; }
    
    public FlowEvent(EventSystem eventSystem) : base(eventSystem){
    }
    
}
}
