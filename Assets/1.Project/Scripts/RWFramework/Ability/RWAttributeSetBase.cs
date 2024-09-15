using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

public class RWAttributeSetBase : MonoBehaviour
{
    protected SFAttribute hp = null;
    protected SFAttribute maxHP = null;
    protected SFAttribute mp = null;
    protected SFAttribute maxMP = null;

    protected virtual void Start()
    {
    }
}
