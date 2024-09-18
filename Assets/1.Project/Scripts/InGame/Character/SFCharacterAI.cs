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
        dispasible.SubscribeEventOnToggle(view.OnTriggerEnterAsObservable(), collider =>
        {
            if (view.isSelfCollider(collider) == true)
            {
                return;
            }

            var skill = collider.GetComponent<SLSkillBase>();
            if (skill != null)
            {
                var skillData = skill.GetSkillData();
                foreach (var factor in skillData.factorSet)
                {
                    abilityComponent.AddFactorData(factor);
                    Debug.Log($"SFCharacterAI OnTrigger collider: {collider.gameObject.name}");
                }

                //abilityComponent.InteractWithExternalFactorDataSet("System_Damaged", skillData.factorSet);
            }

        });

        abilityComponent.SubscribeFactor("HP", (prev, current) =>
        {
            Debug.Log($"prevValue: {prev}, currentValue: {current}");
        });
    }
}
