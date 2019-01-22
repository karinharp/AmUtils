using UnityEngine;   
using UnityEngine.UI;   

namespace am
{

public static class AmGameObjectExtention
{
    
    public static GameObject SetParent(this GameObject child, GameObject parent){	
	child.transform.parent = parent.transform;
	return child;
    }

    public static GameObject SetUIParent(this GameObject child, GameObject parent, bool worldPositionStays = false){
	var childRtf  = child.GetComponent<RectTransform>();
	// うまく動かない模様。。（ぇぇ。。
	if(childRtf.parent != parent)
	{
	    childRtf.SetParent(parent.transform, worldPositionStays);	    	    
	}
	return child;
    }

    public static GameObject InstantiateWithOrigName(this GameObject pf){
	var go = UnityEngine.Object.Instantiate(pf) as GameObject;
	go.name = pf.name;
	return go;
    }

    public static GameObject GetRootGameObject(this GameObject child){
	GameObject go = child;
	while((go.transform.parent != null) && (go.transform.parent.gameObject != null))
	    go = go.transform.parent.gameObject;	    
	return go;
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C../"
 * End:
 */
