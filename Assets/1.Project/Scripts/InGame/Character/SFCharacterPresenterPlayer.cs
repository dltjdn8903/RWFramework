using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EMoveType
{
    None,
    Left,
    Right,
    Up,
    Down,
}

public class PlayerTalk : Interactable
{
    public void Interaction()
    {
    }
}

//public class PlayerWalk : Moveable
//{
//    private SFCharacterBasePresenter<SFCharacterModelPlayer> presenter = null;

//    private PlayerWalk() { }

//    public PlayerWalk(SFCharacterBasePresenter<SFCharacterModelPlayer> presenter)
//    {
//        this.presenter = presenter;
//    }

//    public void Move()
//    {

//    }
//}

//public class SFCharacterModelPlayer: SFCharacterModelBase
//{
//    private int characterNumber = -1;
//    public Interactable interaction = null;
//    public Moveable move = null;
//}

public class SFCharacterPlayerInitData : SFCharacterBaseInitData
{
    public Vector3 initPosition;
}

[CanEditMultipleObjects]
public class SFCharacterPresenterPlayer : SFCharacterBasePresenter
{
    //public SFCharacterViewBase view = null;
    //private SFCharacterModelPlayer data = new SFCharacterModelPlayer();

    public SFAbilityComponent abilityComponent = null;

    public List<RWFactorDataSetBase> initFactorDataSetList = new List<RWFactorDataSetBase>();

    public static SFCharacterPresenterPlayer CreateCharacter(SFCharacterPlayerInitData data)
    {
        SFCharacterPresenterPlayer result = null;

        var resource = Resources.Load<SFCharacterPresenterPlayer>(data.characterTID);
        result = Instantiate(resource, data.initPosition, Quaternion.identity);
        result.InitCharacter(data);

        return result;
    }

    private void Awake()
    {
        AddSubscribes();
    }

    private void AddSubscribes()
    {
        var dispasible = DisposableObjectFactory.GetOrAdd(gameObject);

        abilityComponent.SubscribeFactor("HP", (prev, current) =>
        {
            //var maxHP = GetFactor("MaxHP");
            //if (current.Value > maxHP)
            //{
            //    current.Value = maxHP;
            //}

            //if (current.Value <= 0)
            //{
            //    current.Value = 0;
            //}

            Debug.Log($"prevValue: {prev}, currentValue: {current}");
        });
    }

    private void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        foreach (var item in initFactorDataSetList)
        {
            abilityComponent.AddDataSet(item);
        }
    }

    public void AdjustFactor(string metaKey, RWFactorData factor)
    {
        abilityComponent.InteractWithExternalFactorData(metaKey, factor);
    }

    public void InitCharacter(SFCharacterPlayerInitData initData)
    {
        data = new SFCharacterModelPlayer();

        var viewResourcePath = "Prefab/Character/CharacterPresenterPlayer";
        var resource = Resources.Load<GameObject>(viewResourcePath);
        var viewInstance = Instantiate(resource, initData.initPosition, Quaternion.identity);
        view.InitView(viewInstance);
        //result.InitCharacter(data);

        //SFCharacterViewBase.LoadPrefabByName("Prefab/Character/CharacterPresenterPlayer", viewInstance => 
        //{
        //    var aa = viewInstance as SFCharacterViewPlayer;
        //});

        //initData.interaction = new PlayerTalk();
        //initData.move = new PlayerWalk(this);


        //TanukiCharView.LoadByPrefabNameAsync(prefabName,
        //    (charView) =>
        //    {
        //        charView.transform.SetParent(m_trmCharView);
        //        MDCommon.NormalizeTransform(charView.transform);

        //        charView.AttachWeapon(weaponTid);
        //        charView.SetOutlineState(true);

        //        charView.SetupProvocation(m_tempSetupPlayerLink);

        //        m_charView = charView;
        //        m_charChangeFunc.SetupView(charView);
        //    }
        //);

    }

    private float GetFactor(string tag)
    {
        float result = abilityComponent.GetFactorValue(tag);
        return result;
    }

    //void Update()
    //{
    //    InputCheck();
    //}

    //private void InputCheck()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {

    //    }

    //    if (Input.GetKeyDown(KeyCode.D))
    //    {

    //    }

    //    if (Input.GetKeyDown(KeyCode.W))
    //    {

    //    }

    //    if (Input.GetKeyDown(KeyCode.S))
    //    {

    //    }
    //}

    //private void MoveHorizontal()
    //{
    //    initData.move.Move();
    //}

    [Header("Ability Test")]
    public string testMetaFactorKey = string.Empty;

    public void ApplyTestMetaFactorDamage()
    {
        var SkillTest = abilityComponent.CreateFactorByMeta(testMetaFactorKey);
        AdjustFactor("System_Damaged", SkillTest);
    }

    public void ApplyTestMetaFactorBenefit()
    {
        var SkillTest = abilityComponent.CreateFactorByMeta(testMetaFactorKey);
        AdjustFactor("System_Benefit", SkillTest);
    }

    public SFAttributeSetArmor armor = null;

    public void AddArmor()
    {
        abilityComponent.AddDataSet(armor);
    }

    public void RemoveArmor()
    {
        abilityComponent.RemoveAttributeSet(armor);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SFCharacterPresenterPlayer))]
public class SFCharacterPresenterPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(30);

        SFCharacterPresenterPlayer player = (SFCharacterPresenterPlayer)target;

        if (GUILayout.Button("ApplyTestMetaFactorDamage"))
        {
            player.ApplyTestMetaFactorDamage();
        }

        if (GUILayout.Button("ApplyTestMetaFactorBenefit"))
        {
            player.ApplyTestMetaFactorBenefit();
        }

        if (GUILayout.Button("AddArmor"))
        {
            player.AddArmor();
        }

        if (GUILayout.Button("RemoveArmor"))
        {
            player.RemoveArmor();
        }
    }
}
#endif