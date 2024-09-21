using System;
using UnityEngine;

public class SLCheckAnimationEvent : MonoBehaviour
{
    public Action startEventByView = null;
    public Action endEventByView = null;

    public void SetViewEvent(Action start, Action end)
    {
        startEventByView = start;
        endEventByView = end;
    }

    public void StartEvent()
    {
        startEventByView?.Invoke();
    }

    public void EndEvent()
    {
        endEventByView?.Invoke();
    }
}
