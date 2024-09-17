using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Breakpoint : ActionNode
{
    protected override void OnStart() {
        UnityEngine.Debug.Log("Trigging Breakpoint");
        UnityEngine.Debug.Break();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
