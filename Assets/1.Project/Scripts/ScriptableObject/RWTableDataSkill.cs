using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SkillData
{
    public string key;
    public string animStateParameter;
    public string objectPath;
}

public class RWTableDataSkill : ScriptableObject
{
    public List<SkillData> skillDataList = new List<SkillData>();
    private Dictionary<string, SkillData> skillDataDictionary = new Dictionary<string, SkillData>();

    public SkillData GetSkillTableData(string key)
    {
        var result = new SkillData();

        if (skillDataDictionary.Count == 0)
        {
            foreach (var factor in skillDataList)
            {
                skillDataDictionary.Add(factor.key, factor);
            }
        }

        skillDataDictionary.TryGetValue(key, out result);
        return result;
    }

    public static RWTableDataSkill Config
    {
        get
        {
            return Game.gameData.skillConfig;
        }
    }
}
