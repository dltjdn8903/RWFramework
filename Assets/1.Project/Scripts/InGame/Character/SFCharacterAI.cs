using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;

public class SFCharacterAI : SFCharacterBasePresenter
{
    public SFAbilityComponent abilityComponent = null;

    public List<RWFactorDataSetBase> initFactorDataSetList = new List<RWFactorDataSetBase>();

    private void Awake()
    {
        AddSubscribes();
        InitPlayer();
    }

    private void InitPlayer()
    {
        foreach (var item in initFactorDataSetList)
        {
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
