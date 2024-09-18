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

public class SLSkillBase : MonoBehaviour
{
    public SLSkillInitData skillData = null;

    public bool IsReady
    {
        get
        {
            return skillData.factorSet.Count > 0;
        }
    }

    public void SetSkillData(SLSkillInitData data)
    {
        skillData = data;
    }

    public SLSkillInitData GetSkillData()
    {
        return skillData;
    }
}
