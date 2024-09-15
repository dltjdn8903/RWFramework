using System;
using System.Collections.Generic;
using System.Linq;


[Serializable]
public class MetaFactor
{
    public string tag;
    public bool isInternalFactor = true;
    public bool isLimitZero;
    public CalculateType type = CalculateType.Add;
    public List<MetaFactor> metaAttributeList = new List<MetaFactor>();
}

public static class RWCommon
{
    public static RWFactorData InteractFactor(string metaKey, Dictionary<string, float> internalDictionary, Dictionary<string, float> externalDictionary = null)
    {
        float CalculatorMetaFactor(MetaFactor metaFactor)
        {
            var isInternalCase = externalDictionary == null || metaFactor.isInternalFactor == true;
            var selectDictionary = isInternalCase ? internalDictionary : externalDictionary;

            selectDictionary.TryGetValue(metaFactor.tag, out var result);
            if (result == 0f)
            {
                var factor = RWGlobalFactorConfig.Config.GetGolbalFactorData(metaFactor.tag);
                result = factor == null ? 0 : factor.value;
            }

            var childMetaAttributeList = metaFactor.metaAttributeList;
            if (childMetaAttributeList.Count > 0)
            {
                result = CalculateMetaFactorList(childMetaAttributeList, result, CalculatorMetaFactor);
            }
            else
            {
                if (metaFactor.isInternalFactor == true)
                {
                    internalDictionary.TryGetValue(metaFactor.tag, out result);
                    if (result == 0f)
                    {
                        var factor = RWGlobalFactorConfig.Config.GetGolbalFactorData(metaFactor.tag);
                        result = factor == null ? 0 : factor.value;
                    }
                }
                else
                {
                    externalDictionary.TryGetValue(metaFactor.tag, out result);
                    if (result == 0f)
                    {
                        var factor = RWGlobalFactorConfig.Config.GetGolbalFactorData(metaFactor.tag);
                        result = factor == null ? 0 : factor.value;
                    }
                }
            }

            if (metaFactor.isLimitZero == true && result <= 0f)
            {
                result = 0f;
            }

            return result;
        }

        var meta = RWGlobalFactorConfig.Config.GetGolbalMetaFactorSet(metaKey);
        var result = new RWFactorData();
        result.factorTag = meta.retunTag;
        result.type = meta.type;
        result.value = CalculateMetaFactorList(meta.metaFactorList, result.value, CalculatorMetaFactor);
        return result;
    }

    private static float CalculateMetaFactorList(List<MetaFactor> metaFactorList, float result, Func<MetaFactor, float> calculateEvent)
    {
        foreach (var metaFactor in metaFactorList)
        {
            switch (metaFactor.type)
            {
                case CalculateType.Add:
                    {
                        result += calculateEvent(metaFactor);
                    }
                    break;
                case CalculateType.Subtract:
                    {
                        result -= calculateEvent(metaFactor);
                    }
                    break;
                case CalculateType.Multiply:
                    {
                        result *= calculateEvent(metaFactor);
                    }
                    break;
                case CalculateType.Divide:
                    {
                        result /= calculateEvent(metaFactor);
                    }
                    break;
                default:
                    break;
            }
        }



        return result;
    }

    public static Dictionary<string, float> FactorConvertToDictionaryByList(List<RWFactorData> list, Dictionary<string, float> targetDictionary = null)
    {
        if (targetDictionary == null)
        {
            targetDictionary = new Dictionary<string, float>();
        }

        foreach (var attribute in list)
        {
            var lastTag = attribute.factorTag.Split('|').Last();
            switch (attribute.type)
            {
                case CalculateType.Add:
                    {
                        if (targetDictionary.ContainsKey(lastTag) == false)
                        {
                            targetDictionary.Add(lastTag, attribute.value);
                        }
                        else
                        {
                            targetDictionary[lastTag] += attribute.value;
                        }
                    }
                    break;
                case CalculateType.Subtract:
                    {
                        if (targetDictionary.ContainsKey(lastTag) == false)
                        {
                            targetDictionary.Add(lastTag, attribute.value);
                        }
                        else
                        {
                            targetDictionary[lastTag] -= attribute.value;
                        }
                    }
                    break;
                case CalculateType.Multiply:
                    {
                        if (targetDictionary.ContainsKey(lastTag) == false)
                        {
                            targetDictionary.Add(lastTag, attribute.value);
                        }
                        else
                        {
                            targetDictionary[lastTag] *= attribute.value;
                        }
                    }
                    break;
                case CalculateType.Divide:
                    {
                        if (targetDictionary.ContainsKey(lastTag) == false)
                        {
                            targetDictionary.Add(lastTag, attribute.value);
                        }
                        else
                        {
                            targetDictionary[lastTag] /= attribute.value;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        return targetDictionary;
    }
}
