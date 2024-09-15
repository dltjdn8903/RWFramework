using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFCharacterViewBase : MonoBehaviour
{
    public Animator animator = null;

    private RuntimeAnimatorController currentController = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    public void InitView(GameObject viewInstance)
    {
        animator = viewInstance.GetComponent<Animator>();
        currentController = animator.runtimeAnimatorController;
    }

    public static void LoadPrefabByName(string prefabName, Action<SFCharacterViewBase> onLoadDone)
    {
        var resource = Resources.Load(prefabName);
        var result = Instantiate(resource) as SFCharacterViewBase;
        if (result != null) 
        {
            onLoadDone?.Invoke(result);
        }
    }



    public void PlayAnim(string animName)
    {
        animator.SetTrigger(animName);
    }
}
