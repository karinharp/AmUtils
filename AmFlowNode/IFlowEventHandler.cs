using UnityEngine.EventSystems;

namespace am
{    
public interface IFlowEventHandler : IEventSystemHandler {
    void OnRecieveFlowEvent(FlowEvent evt);
}
}
