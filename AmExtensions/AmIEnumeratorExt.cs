using System.Collections;
using System.Threading.Tasks;

namespace am
{
    
public static class IEnumeratorExt
{
    public static IEnumerator ToEnumerator(this Task task){
	while (!task.IsCompleted){ yield return null; }
    }

    public static IEnumerator ToEnumerator<T>(this Task<T> task){
	while (!task.IsCompleted){ yield return null; }
    }
}
}
