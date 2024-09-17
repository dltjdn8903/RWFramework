using UnityEngine;



public abstract class Interactable
{
    public abstract void Interaction();
}

public abstract class Moveable
{
    public abstract void Move(float speed, Vector3 direction);
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
    protected SFCharacterModelBase data = null;
    public SFCharacterView view = null;

    public virtual void InitCharacter<T>(T initData) where T : SFCharacterBaseInitData
    {
        var resource = Resources.Load<GameObject>(initData.characterTID);
        var instance = Instantiate(resource, initData.initPosition, Quaternion.identity);
    }
}
