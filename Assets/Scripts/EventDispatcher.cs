using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    public void Dispatch(string eventName) => SendMessageUpwards(eventName, SendMessageOptions.DontRequireReceiver);
}
