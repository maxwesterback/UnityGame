using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SignalListener : MonoBehaviour
{

    public Message message;
    public UnityEvent signalEvent;

    public void OnSignalRaised()
    {
        signalEvent.Invoke();
    }

    private void OnEnable()
    {
        message.RegisterListener(this);
    }
    private void OnDisable()
    {
        message.DeRegisterListener(this);
    }
}
