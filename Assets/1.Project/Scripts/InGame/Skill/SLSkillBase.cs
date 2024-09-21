using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class SLSkillInitData
{
    public int ownerID;
    public string skillKey;
    public List<RWFactorData> factorSet = new List<RWFactorData>();
}

public class SkillInteractionData
{
    public int ownerID;
    public List<SkillFactorMeta> skillFactorMetaList = new List<SkillFactorMeta>();
    public List<RWFactorData> factorSet = new List<RWFactorData>();
}

public class SLSkillBase : MonoBehaviour
{
    public int ownerID;

    public string skillKey;

    public List<SkillFactorMeta> skillFactorMetaList = new List<SkillFactorMeta>();

    public List<RWFactorData> factorSet = new List<RWFactorData>();

    public void InitSkillData(SLSkillInitData data)
    {
        var tableSkillMeta = RWTableDataSkill.Config.GetSkillTableData(data.skillKey);
        ownerID = data.ownerID;
        skillKey = data.skillKey;
        factorSet = data.factorSet;
        skillFactorMetaList = tableSkillMeta.skillFactorMetaList;
        foreach (var factor in factorSet)
        {
            factor.owerID = ownerID;
            factor.instanceID = gameObject.GetInstanceID();
        }
    }

    public SkillInteractionData GetSkillInteractionData()
    {
        var result = new SkillInteractionData();
        result.ownerID = ownerID;
        result.skillFactorMetaList = skillFactorMetaList;
        result.factorSet = factorSet;

        return result;
    }
}
