using System.Collections;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

public class SLCharacterManager : RWSingleton<SLCharacterManager>
{
    public List<SFCharacterBasePresenter> spawnedCharacterList = new List<SFCharacterBasePresenter>();

    public void CreateCharacter<T, V>(V initData) where T : SFCharacterBasePresenter where V : SFCharacterBaseInitData
    {
        if (initData.characterTID.IsNullOrEmpty() == true)
        {
            return;
        }

        var resource = Resources.Load<GameObject>(initData.characterTID);
        var instance = Instantiate(resource, initData.initPosition, Quaternion.identity);
        var result = instance?.GetComponent<T>();
        result.InitCharacter(initData);

        spawnedCharacterList.Add(result);
    }
}
