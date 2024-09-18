using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class CombatFormControllerPath
{
    public static string KickBoxing => "Characters/Controller/KickBoxing";
}

public class AnimStateParameterName
{
    public static string Idle => "Idle";
    public static string Move => "Move";
}

[Serializable]
public class FormColliderData
{
    public string formKey;
    public List<BoxCollider> colliders;
}

public class SFCharacterView : MonoBehaviour
{
    public Animator animator = null;

    private RuntimeAnimatorController currentController = null;

    public List<FormColliderData> formColliderList = new List<FormColliderData>();

    public SLSkillBase currentSkillObject = null;

    private string combatForm = string.Empty;
    public string CombatForm 
    { 
        get => combatForm; 
        set => combatForm = value; 
    }

    private void Awake()
    {
        CombatForm = CombatFormControllerPath.KickBoxing;
    }

    private void Start()
    {

    }

    public void InitView(GameObject viewInstance)
    {
        animator = viewInstance.GetComponent<Animator>();
        currentController = animator.runtimeAnimatorController;
    }

    public static void LoadPrefabByName(string prefabName, Action<SFCharacterView> onLoadDone)
    {
        var resource = Resources.Load<GameObject>(prefabName);
        var result = Instantiate(resource).GetComponent<SFCharacterView>();
        if (result != null)
        {
            onLoadDone?.Invoke(result);
        }
    }

    public void ChangeContorller(string controller)
    {

    }

    Coroutine endEventCorutine = null;

    IEnumerator IEStartEndEvent(string animName, Action onEndEvent)
    {
        bool isStart = false;
        while (!isStart)
        {
            yield return null;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(animName) == true)
            {
                isStart = true;
            }
        }

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(currentState.length / currentState.speed);
        onEndEvent?.Invoke();
    }

    public void PlayAnim(string animName, Action onEndEvent = null)
    {
        animator.SetTrigger(animName);

        if (endEventCorutine != null)
        {
            StopCoroutine(endEventCorutine);
        }

        endEventCorutine = StartCoroutine(IEStartEndEvent(animName, onEndEvent));
    }

    public bool isSelfCollider(Collider collider)
    {
        bool result = false;

        var data = formColliderList.Find(value => value.colliders.Contains(collider) == false);
        result = data != null;
        return result;
    }

    public void StartSkill()
    {

    }

    public void EndSkill()
    {

    }
}
