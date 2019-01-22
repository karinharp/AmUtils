using UnityEngine;   
using System;
using System.Collections;

namespace am
{
    
public static class AmMonoBehaviourExtention
{
    public static void DelayTask(this MonoBehaviour pMb, float waitTime, Action action){
	pMb.StartCoroutine(DelayMethod(waitTime, action)); 
    }
    public static IEnumerator DelayMethod(float waitTime, Action action){
	yield return new WaitForSeconds(waitTime);
	action();
    }
    public static void DelayTask<T1>(this MonoBehaviour pMb, float waitTime, Action<T1> action, T1 arg1){
	pMb.StartCoroutine(DelayMethod(waitTime, action, arg1)); 
    }
    public static IEnumerator DelayMethod<T1>(float waitTime, Action<T1> action, T1 arg1){
	yield return new WaitForSeconds(waitTime);
	action(arg1);
    }
    
}
}

/*
 * Local variables:
 * compile-command: "make -C../"
 * End:
 */
