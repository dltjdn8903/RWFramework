using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

public class SFCharacterAI : SFCharacterBasePresenter
{
    public SFAbilityComponent abilityComponent = null;

    public List<RWFactorDataSetBase> initFactorDataSetList = new List<RWFactorDataSetBase>();

    private static string viewPath = "Prefab/Character/View/";

    private void Awake()
    {
        InitAbility();
        AddSubscribes();
    }

    private void InitAbility()
    {
        foreach (var item in initFactorDataSetList)
        {
            item.InitDataSet(gameObject.GetInstanceID());
            abilityComponent.AddDataSet(item);
        }
    }

    private void AddSubscribes()
    {
        var dispasible = DisposableObjectFactory.GetOrAdd(gameObject);
        dispasible.SubscribeEventOnToggle(view.OnTriggerEnterAsObservable(), HitEvent);

        abilityComponent.SubscribeFactor("HP", (prev, current) =>
        {
            Debug.Log($"prevValue: {prev}, currentValue: {current}");
        });
    }

    public void InitCharacter(string viewName)
    {
        SFCharacterView.LoadPrefabByName($"{viewPath}{viewName}", viewInstance =>
        {
            view = viewInstance;
            viewInstance.transform.SetParent(transform);
            viewInstance.transform.localPosition = Vector3.zero;
            viewInstance.transform.localScale = Vector3.one;
        });
    }

    private void Update()
    {
        RotateCharacter();
        MoveCharacter();
    }

    private void RotateCharacter()
    {
    }

    private void MoveCharacter()
    {
    }

    private void HitEvent(Collider collider)
    {
        if (view.isSelfCollider(collider) == true)
        {
            return;
        }

        var skill = collider.GetComponent<SLSkillBase>();
        if (skill != null)
        {
            var skillInteractionData = skill.GetSkillInteractionData();
            foreach (var factorMeta in skillInteractionData.skillFactorMetaList)
            {
                var factorData = skillInteractionData.factorSet.Find(value => value.factorTag == factorMeta.factorTag);
                abilityComponent.InteractWithExternalFactorData(factorMeta.metaKey, factorData);
            }
        }
    }
}
