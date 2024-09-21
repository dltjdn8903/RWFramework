using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;



public enum CalculateType
{
    None,
    Add,
    Subtract,
    Multiply,
    Divide,
}

public enum EAbilityComponentState
{
    None,
    PauseRecalculate,
}

public class SFAbilityComponent : MonoBehaviour
{
    [SerializeField] private List<RWFactorDataSetBase> factorDataSetList = new List<RWFactorDataSetBase>();

    [SerializeField] private List<RWFactorData> internalDataList = new List<RWFactorData>();
    [SerializeField] private List<RWFactorData> externalDataList = new List<RWFactorData>();

    private Dictionary<string, float> previousfactorDataDictionary = new Dictionary<string, float>();
    private Dictionary<string, ReactiveProperty<float>> resultDataDictionary = new Dictionary<string, ReactiveProperty<float>>();

    private int owerID = 0;

    private bool isChanged = false;

    private EAbilityComponentState state = EAbilityComponentState.PauseRecalculate;
    public EAbilityComponentState State
    {
        get => state;
        set
        {
            if (value == EAbilityComponentState.None)
            {
                state = EAbilityComponentState.None;
                RecalculateAttributeList();
            }
        }
    }

    private void Awake()
    {
        owerID = gameObject.GetInstanceID();
    }

    private void Start()
    {
        State = EAbilityComponentState.None;
    }

    public void AddDataSet(RWFactorDataSetBase attributeSet)
    {
        if (attributeSet == null)
        {
            return;
        }

        factorDataSetList.Add(attributeSet);
        factorDataSetList = factorDataSetList.OrderBy(value => value.factorDataSet.calculateSequence).ToList();
        internalDataList.AddRange(attributeSet.factorDataSet.factorDataList);

        internalDataList = internalDataList.OrderBy(value => value.type).ToList();

        isChanged = true;
        RecalculateAttributeList();
    }

    public void AddFactorData(RWFactorData factorData)
    {
        externalDataList.Add(factorData);

        isChanged = true;
        RecalculateAttributeList();
    }

    public void RemoveAttributeSet(RWFactorDataSetBase attributeSet)
    {
        foreach (var item in attributeSet.factorDataSet.factorDataList)
        {
            internalDataList.Remove(item);
        }

        factorDataSetList.Remove(attributeSet);
        factorDataSetList = factorDataSetList.OrderBy(value => value.factorDataSet.calculateSequence).ToList();

        isChanged = true;
        RecalculateAttributeList();
    }

    public void RemoveFactorData(string factorTag)
    {
        var factorData = externalDataList.Find(value => value.factorTag == factorTag);
        if (factorData != null)
        {
            externalDataList.Remove(factorData);
        }

        isChanged = true;
        RecalculateAttributeList();
    }

    public float GetFactorValue(string tag)
    {
        float result = 0f;

        if (resultDataDictionary.ContainsKey(tag) == true)
        {
            result = resultDataDictionary[tag].Value;
        }

        return result;
    }
    
    private void RecalculateAttributeList()
    {
        if (state == EAbilityComponentState.PauseRecalculate || isChanged == false)
        {
            return;
        }
        var currentFactorDataDictionary = RWCommon.FactorConvertToDictionaryByList(internalDataList);
        RWCommon.FactorConvertToDictionaryByList(externalDataList, currentFactorDataDictionary);

        foreach (var factorData in currentFactorDataDictionary)
        {
            var key = factorData.Key;
            var value = factorData.Value;

            if (resultDataDictionary.ContainsKey(key) == true)
            {
                resultDataDictionary[key].Value = value;
            }
            else
            {
                resultDataDictionary.Add(key, new ReactiveProperty<float>(value));
            }
        }

        previousfactorDataDictionary = currentFactorDataDictionary;

        isChanged = false;
    }

    public void SubscribeFactor(string tag, Action<float, ReactiveProperty<float>> onChangeEvent)
    {
        if (previousfactorDataDictionary.ContainsKey(tag) == false)
        {
            previousfactorDataDictionary.Add(tag, 0f);
        }

        if (resultDataDictionary.ContainsKey(tag) == false)
        {
            var factor = new ReactiveProperty<float>(0f);
            resultDataDictionary.Add(tag, factor);

            var disposibale = DisposableObjectFactory.GetOrAdd(gameObject);
            disposibale.ObserveReactiveOnToggle(factor, value => 
            {
                onChangeEvent?.Invoke(previousfactorDataDictionary[tag], factor);
            });
        }
        else
        {
            var disposibale = DisposableObjectFactory.GetOrAdd(gameObject);
            disposibale.ObserveReactiveOnToggle(resultDataDictionary[tag], value =>
            {
                onChangeEvent?.Invoke(previousfactorDataDictionary[tag], resultDataDictionary[tag]);
            });
        }
    }

    public RWFactorData CreateFactorByMeta(string targetKey)
    {
        var internalDictionary = RWCommon.FactorConvertToDictionaryByList(internalDataList);

        var result = RWCommon.InteractFactor(targetKey, internalDictionary);
        return result;
    }

    public void InteractWithExternalFactorData(string metaKey, RWFactorData externalFactorData)
    {
        var overlapFactor = externalDataList.Find(value => value.owerID == externalFactorData.owerID && value.instanceID == externalFactorData.instanceID && value.factorTag == externalFactorData.factorTag);
        if (overlapFactor != null)
        {
            return;
        }

        var internalDictionary = RWCommon.FactorConvertToDictionaryByList(internalDataList);

        var tempDataList = new List<RWFactorData>();
        tempDataList.Add(externalFactorData);
        var externalDictionary = RWCommon.FactorConvertToDictionaryByList(tempDataList);

        var resultFactor = RWCommon.InteractFactor(metaKey, internalDictionary, externalDictionary);
        resultFactor = CheckFactorInfo(resultFactor);

        if (resultFactor == null)
        {
            return;
        }

        externalDataList.Add(resultFactor);

        isChanged = true;
        RecalculateAttributeList();
    }
    private RWFactorData CheckFactorInfo(RWFactorData input)
    {
        RWFactorData result = input.Duplicate();

        var factorInfo = RWGlobalFactorConfig.Config.GetGolbalFactorInfomation(input.factorTag);
        if (factorInfo != null)
        {
            foreach (var bindingData in factorInfo.bindingTagList)
            {
                if (resultDataDictionary.ContainsKey(bindingData.bindingTag) == true)
                {
                    var bindFactor = resultDataDictionary[bindingData.bindingTag];
                    var targetFactor = resultDataDictionary[input.factorTag];
                    float cal = 0;

                    switch (input.type)
                    {
                        case CalculateType.Add:
                            cal = targetFactor.Value + input.value;
                            break;
                        case CalculateType.Subtract:
                            cal = targetFactor.Value - input.value;
                            break;
                        case CalculateType.Multiply:
                            cal = targetFactor.Value * input.value;
                            break;
                        case CalculateType.Divide:
                            cal = targetFactor.Value / input.value;
                            break;
                        default:
                            break;
                    }

                    switch (bindingData.bindingType)
                    {
                        case BindingType.Max:
                            {
                                var max = bindFactor.Value - cal;
                                max *= -1f;
                                if (max == input.value)
                                {
                                    return null;
                                }
                                else if (max >= 0f)
                                {
                                    result.value = bindFactor.Value - targetFactor.Value;
                                }
                                else
                                {

                                }
                            }
                            break;
                        case BindingType.Minimun:
                            {
                                var min = cal - bindFactor.Value;
                                min *= -1f;
                                if (min == input.value)
                                {
                                    return null;
                                }
                                else if (min >= 0f)
                                {
                                    result.value = targetFactor.Value - bindFactor.Value;
                                }
                                else
                                {

                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (factorInfo.isUnsigned == true && cal < 0)
                    {
                        if (-cal == input.value)
                        {
                            return null;
                        }
                        else if (cal < 0f)
                        {
                            result.value = input.value + cal;
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        return result;
    }

    public void InteractWithExternalFactorDataSet(string metaKey, List<RWFactorData> externalFactorDataSet)
    {
        var internalDictionary = RWCommon.FactorConvertToDictionaryByList(internalDataList);
        var externalDictionary = RWCommon.FactorConvertToDictionaryByList(externalFactorDataSet);

        var resultFactor = RWCommon.InteractFactor(metaKey, internalDictionary, externalDictionary);
        externalDataList.Add(resultFactor);

        isChanged = true;
        RecalculateAttributeList();
    }

    public void InteractWithExternalFactorDictionary(string metaKey, Dictionary<string, float> externalDictionary)
    {
        var internalDictionary = RWCommon.FactorConvertToDictionaryByList(internalDataList);
        var resultFactor = RWCommon.InteractFactor(metaKey, internalDictionary, externalDictionary);
        externalDataList.Add(resultFactor);

        isChanged = true;
        RecalculateAttributeList();
    }
}
