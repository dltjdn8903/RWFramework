using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactorSetInitData
{
    public int ownerID;
    public string skillKey;
    public List<RWFactorData> factorSet = new List<RWFactorData>();
}

public class SLSkillFactorDataSet : RWFactorDataSetBase
{


    public void Init(SkillFactorSetInitData data)
    {

    }
}
