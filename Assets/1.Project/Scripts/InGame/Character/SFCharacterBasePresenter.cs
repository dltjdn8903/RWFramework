using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public interface Interactable
{
    public abstract void Interaction();
}

public interface Moveable
{
    public void Move();
}

public class SFCharacterBaseInitData
{
    public string characterTID = string.Empty;
    public Vector3 initPosition = Vector3.zero;
}

public class SFCharacterModelBase
{
    public int characterNumber = -1;

}

public abstract class SFCharacterBasePresenter : MonoBehaviour 
{
    protected SFCharacterModelBase data = default;
    public SFCharacterViewBase view = null;

    //private SFCharacterBaseInitData data = null;
    //private SFCharacterModelBase model = null;
    //public SFCharacterViewBase view = null;

    //private void Awake()
    //{
    //    CharacterNPCData initData = new CharacterNPCData();
    //    initData.talk = new NPCTalk();
    //    initData.walk = new NPCWalk();

    //    CharacterNPC npc = new CharacterNPC();
    //    npc.InitCharacter(initData);

    //    npc.TalkWithPlayer();
    //    npc.MoveNPC();
    //}

    //private void Start()
    //{

    //}

    public virtual void InitCharacter<T>(T initData) where T : SFCharacterBaseInitData
    {
        var resource = Resources.Load<GameObject>(initData.characterTID);

        var instance = Instantiate(resource, initData.initPosition, Quaternion.identity);
    }
}
