using System;
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

public class SFCharacterView : MonoBehaviour
{
    public Animator animator = null;

    private RuntimeAnimatorController currentController = null;

    private string combatForm = string.Empty;
    public string CombatForm 
    { 
        get => combatForm; 
        set => combatForm = value; 
    }

    private string state = string.Empty;
    public string State
    {
        get => state;
        set
        {
            if (value != state)
            {
                PlayAnim(value);
            }
        }
    }


    private void Awake()
    {
        CombatForm = CombatFormControllerPath.KickBoxing;
        State = AnimStateParameterName.Idle;
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

    public void PlayAnim(string animName)
    {
        animator.SetTrigger(animName);
    }
}
