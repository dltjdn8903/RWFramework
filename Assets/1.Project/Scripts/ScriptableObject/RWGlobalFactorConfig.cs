using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class FactorMetaSet
{
    public string key;
    public string returnKey;
    public string factorTag;
    public CalculateType type = CalculateType.Add;
    public List<MetaFactor> metaFactorList = new List<MetaFactor>();
}

[System.Flags]
public enum BindingType
{
    Max = 1 << 0,  // 1
    Minimun = 1 << 1,  // 2
}

[Serializable]
public class BindData
{
    public string bindingTag;
    public BindingType bindingType;
}

[Serializable]
public class FactorInfomation
{
    public string targetTag;
    public List<BindData> bindingTagList;

    public bool isUnsigned = false;
}

public class RWGlobalFactorConfig : ScriptableObject
{
    public List<RWFactorData> globalFactorList = new List<RWFactorData>();
    private Dictionary<string, RWFactorData> globalFactorDictionary = new Dictionary<string, RWFactorData>();

    public List<FactorInfomation> globalFactorInfomationList = new List<FactorInfomation>();
    private Dictionary<string, FactorInfomation> globalFactorInfomationDictionary = new Dictionary<string, FactorInfomation>();

    public List<FactorMetaSet> globalMetaFactorList = new List<FactorMetaSet>();
    private Dictionary<string, FactorMetaSet> globalMetaFactorDictionary = new Dictionary<string, FactorMetaSet>();

    public RWFactorData GetGolbalFactorData(string key)
    {
        var result = new RWFactorData();

        if (globalFactorDictionary.Count == 0)
        {
            foreach (var factor in globalFactorList)
            {
                globalFactorDictionary.Add(factor.factorTag, factor);
            }
        }

        globalFactorDictionary.TryGetValue(key, out result);
        return result;
    }

    public FactorInfomation GetGolbalFactorInfomation(string key)
    {
        var result = new FactorInfomation();

        if (globalFactorInfomationDictionary.Count == 0)
        {
            foreach (var factor in globalFactorInfomationList)
            {
                globalFactorInfomationDictionary.Add(factor.targetTag, factor);
            }
        }

        globalFactorInfomationDictionary.TryGetValue(key, out result);
        return result;
    }

    public FactorMetaSet GetGolbalMetaFactorSet(string key)
    {
        var result = new FactorMetaSet();

        if (globalMetaFactorDictionary.Count == 0)
        {
            foreach (var factor in globalMetaFactorList)
            {
                globalMetaFactorDictionary.Add(factor.key, factor);
            }
        }

        globalMetaFactorDictionary.TryGetValue(key, out result);
        return result;
    }

    public static RWGlobalFactorConfig Config
    {
        get
        {
            return Game.gameData.metaAttributeConfig;
        }
    }
}
