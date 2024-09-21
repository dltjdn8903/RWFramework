using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;



[Serializable]
public class RWFactorData
{
    public string factorTag;
    public int owerID = 0;
    public int instanceID = 0;
    public CalculateType type = CalculateType.Add;
    public float value = 0f;

    public RWFactorData Duplicate()
    {
        RWFactorData result = new RWFactorData();
        result.owerID = owerID;
        result.instanceID = instanceID;
        result.factorTag = factorTag;
        result.type = type;
        result.value = value;
        return result;
    }
}

[Serializable]
public class RWReactiveFactorData
{
    public string factorTag;
    public CalculateType type = CalculateType.Add;
    public ReactiveProperty<float> value = new ReactiveProperty<float>(0f);

    public RWReactiveFactorData Duplicate()
    {
        RWReactiveFactorData result = new RWReactiveFactorData();
        result.factorTag = factorTag;
        result.type = type;
        result.value = value;
        return result;
    }
}

[Serializable]
public class RWFactorDataSet
{
    public string attributeTag = string.Empty;
    public int calculateSequence = 1;

    public List<RWFactorData> factorDataList = new List<RWFactorData>();
}

[Serializable]
public abstract class RWFactorDataSetBase : MonoBehaviour
{
    public RWFactorDataSet factorDataSet = new RWFactorDataSet();

    public void InitDataSet(int owerID)
    {
        foreach (var item in factorDataSet.factorDataList)
        {
            item.owerID = owerID;
            item.instanceID = gameObject.GetInstanceID();
        }
    }
}
