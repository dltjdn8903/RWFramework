using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;


public class SFTag
{
    public string tag;
    public List<string> subTagList = new List<string>();
}

public class SFAttribute
{
    public string tag;
    public float baseValue;
    public float currentValue;

    private SFAttribute() { }
    public SFAttribute(string tag, float baseValue)
    {
        this.tag = tag;
        this.baseValue = baseValue;
        this.currentValue = baseValue;
    }
}

public class SFAbilityManager : RWSingleton<SFAbilityManager>
{
    private Dictionary<string, List<string>> tagDictionary = new Dictionary<string, List<string>>();
    private Dictionary<string, SFAttribute> attributeDictionary = new Dictionary<string, SFAttribute>();

    public bool AdjustAbilityComponent()
    {
        bool result = false;
        return result;
    }

    public void SetAttribute(SFAttribute value)
    {
        RegisterTags(value.tag);

        if (attributeDictionary.ContainsKey(value.tag) == false)
        {
            attributeDictionary.Add(value.tag, value);
        }
        else
        {
            attributeDictionary[value.tag] = value;
        }
    }

    //public SFAttribute GetCurrentAttribute(string factorTag)
    //{

    //}

    private void RegisterTags(string rawTag)
    {
        rawTag.Trim();
        var tagArray = rawTag.Split('|');
        var tagList = new List<string>(tagArray);

        //단일 태그가 아닐때
        if (tagList.Count >= 2)
        {
            var tag = tagList.Last();
            tagList.RemoveAt(tagList.Count - 1);

            var subTags = GetTagAppender(tagList);

            if (tagDictionary.ContainsKey(subTags) == false)
            {
                var subTagList = new List<string>();
                subTagList.Add(tag);

                tagDictionary.Add(subTags, subTagList);

                RegisterTags(subTags);
            }
            else
            {
                var subTagList = tagDictionary[subTags];
                if (subTagList.Contains(tag) == false)
                {
                    subTagList.Add(tag);

                    //var subTags = GetTagAppender(subTagList);
                    //RegisterTags(subTags);
                }
            }
        }
        else
        {
            if (tagDictionary.ContainsKey(rawTag) == false)
            {
                var subTagList = new List<string>();
                tagDictionary.Add(rawTag, subTagList);
            }
        }
    }

    private string GetTagAppender(List<string> tagList)
    {
        string result = string.Empty;
        var tagBuilder = new StringBuilder();

        foreach (var tag in tagList)
        {
            tagBuilder.Append(tag);
            tagBuilder.Append('|');
        }

        if (tagBuilder.Length > 0)
        {
            result = tagBuilder.ToString();
            result = result.TrimEnd('|');
        }

        return result;
    }
}
