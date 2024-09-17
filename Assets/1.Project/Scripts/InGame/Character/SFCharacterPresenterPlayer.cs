using DG.Tweening;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;



public enum ECharacterState
{
    None,
    Init,
    Idle,
    Walk,
    Run,
    Action,
    Interaction,
    Die,
}

public class PlayerTalk : Interactable
{
    private SFCharacterPresenterPlayer presenter = null;

    private PlayerTalk() { }

    public PlayerTalk(SFCharacterPresenterPlayer presenter)
    {
        this.presenter = presenter;
    }
    public override void Interaction()
    {

    }
}

public class PlayerWalk : Moveable
{
    private SFCharacterPresenterPlayer presenter = null;

    private PlayerWalk() { }

    public PlayerWalk(SFCharacterPresenterPlayer presenter)
    {
        this.presenter = presenter;
    }

    public override void Move(float speed, Vector3 direction)
    {
        Vector3 movePosition = direction.normalized * speed * Time.deltaTime;
        presenter.transform.position += movePosition;
    }
}

public class SFCharacterPlayerInitData : SFCharacterBaseInitData
{
    public Vector3 initPosition;
}

[CanEditMultipleObjects]
public class SFCharacterPresenterPlayer : SFCharacterBasePresenter
{
    public static List<SFCharacterPresenterPlayer> playerCharacterList = new List<SFCharacterPresenterPlayer>();

    public SFAbilityComponent abilityComponent = null;

    public List<RWFactorDataSetBase> initFactorDataSetList = new List<RWFactorDataSetBase>();

    public Interactable interaction = null;
    public Moveable move = null;

    private ECharacterState characterState = ECharacterState.None;

    public ECharacterState CharacterState
    {
        get => characterState;
        private set => ChangeState(value);
    }

    public static SFCharacterPresenterPlayer CreateCharacter(SFCharacterPlayerInitData data)
    {
        SFCharacterPresenterPlayer result = null;

        var resource = Resources.Load<SFCharacterPresenterPlayer>(data.characterTID);
        result = Instantiate(resource, data.initPosition, Quaternion.identity);
        result.InitCharacter(data);

        playerCharacterList.Add(result);

        return result;
    }

    private void Awake()
    {
        if (view != null)
        {
            Destroy(view.gameObject);
            CharacterState = ECharacterState.Init;
        }

        AddSubscribes();
    }

    private void AddSubscribes()
    {
        var dispasible = DisposableObjectFactory.GetOrAdd(gameObject);

        abilityComponent.SubscribeFactor("HP", (prev, current) =>
        {
            Debug.Log($"prevValue: {prev}, currentValue: {current}");
        });
    }

    private void Start()
    {
        //currentState.Value = ECharacterState.Init;
        //move.Move(GetFactor("MoveSpeed"), Vector3.zero);
    }

    private void InitPlayer()
    {
        foreach (var item in initFactorDataSetList)
        {
            abilityComponent.AddDataSet(item);
        }
    }

    private void ChangeState(ECharacterState currentState)
    {
        if (currentState == characterState)
        {
            return;
        }

        characterState = currentState;
        switch (currentState)
        {
            case ECharacterState.Init:
                {
                    StartInit();
                }
                break;
            case ECharacterState.Idle:
                {
                    StartIdle();
                }
                break;
            case ECharacterState.Walk:
                {
                    StartMove();
                }
                break;
            case ECharacterState.Run:
                {
                    StartMove();
                }
                break;
            case ECharacterState.Action:
                {
                    StartAction();
                }
                break;
            case ECharacterState.Interaction:
                {
                    StartInteraction();
                }
                break;
            case ECharacterState.Die:
                {
                    StartDie();
                }
                break;
            default:
                break;
        }
    }

    public void AdjustFactor(string metaKey, RWFactorData factor)
    {
        abilityComponent.InteractWithExternalFactorData(metaKey, factor);
    }

    public void InitCharacter(SFCharacterPlayerInitData initData)
    {
        data = new SFCharacterModelPlayer();

        interaction = new PlayerTalk(this);
        move = new PlayerWalk(this);

        SFCharacterView.LoadPrefabByName("Prefab/Character/View/CharacterViewPrimrose", viewInstance =>
        {
            view = viewInstance;
            viewInstance.transform.SetParent(transform);
            viewInstance.transform.localPosition = Vector3.zero;
            viewInstance.transform.localScale = Vector3.one;

            var dispasible = DisposableObjectFactory.GetOrAdd(gameObject);
            dispasible.SubscribeEventOnToggle(view.OnTriggerEnterAsObservable(), collider =>
            {
                if (view.isSelfCollider(collider) == true)
                {
                    return;
                }

                Debug.Log($"collider: {collider.gameObject.name}");
            });
        });

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

    private void StartInit()
    {
        InitPlayer();

        //임시 초기화
        {
            SFCharacterPlayerInitData initData = new SFCharacterPlayerInitData();
            initData.initPosition = Vector3.zero;

            InitCharacter(initData);
            playerCharacterList.Add(this);
        }

        CharacterState = ECharacterState.Idle;
    }

    private void StartIdle()
    {
        view.PlayAnim(AnimStateParameterName.Idle);
    }

    private void StartMove()
    {
        view.PlayAnim(AnimStateParameterName.Move);
    }

    private SkillData currentActionData = null;

    private void StartAction()
    {
        var skillData = RWTableDataSkill.Config.GetSkillTableData(currentActionData.key);
        view.PlayAnim(skillData.animStateParameter, () =>
        {
            CharacterState = prevDirection == Vector3.zero ? ECharacterState.Idle : ECharacterState.Walk;
            RotateCharacter(prevDirection);
        });
    }

    private void StartInteraction()
    {

    }

    private void StartHit()
    {

    }

    private void StartDie()
    {

    }

    void Update()
    {
        InputCheck();
    }

    private Vector3 prevDirection = Vector3.zero;
    private Tween rotateTween = null;

    private void InputCheck()
    {
        if (CharacterState == ECharacterState.Idle || CharacterState == ECharacterState.Walk)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                currentActionData = RWTableDataSkill.Config.GetSkillTableData("Skill_Punch");
                CharacterState = ECharacterState.Action;
            }
        }

        Vector3 movePosition = Vector3.zero;
        Vector3 direction = Vector3.zero;
        movePosition.x += Input.GetKey(KeyCode.D) ? 1 : 0;
        movePosition.x -= Input.GetKey(KeyCode.A) ? 1 : 0;
        movePosition.z += Input.GetKey(KeyCode.W) ? 1 : 0;
        movePosition.z -= Input.GetKey(KeyCode.S) ? 1 : 0;

        direction = movePosition.normalized;

        if (CharacterState == ECharacterState.Action)
        {
            prevDirection = direction;
            return;
        }

        if (movePosition.magnitude != 0)
        {
            CharacterState = ECharacterState.Walk;
        }
        else
        {
            if (CharacterState == ECharacterState.Walk)
            {
                CharacterState = ECharacterState.Idle;
            }
            return;
        }

        direction = movePosition.normalized;
        var speed = GetFactor("WalkSpeed");

        move.Move(speed, direction);

        if (CharacterState == ECharacterState.Walk)
        {
            if (prevDirection != direction)
            {
                RotateCharacter(direction);
            }
        }

        prevDirection = direction;
    }

    private void RotateCharacter(Vector3 direction)
    {
        rotateTween.Kill();

        Vector3 resultDirection = Vector3.zero;
        rotateTween = DOTween.To(() => resultDirection,
                                 value =>
                                 {
                                     resultDirection = value;
                                     resultDirection += transform.position;
                                     view.transform.LookAt(resultDirection);
                                 },
                                 direction,
                                 0.15f)
                                 .From(prevDirection)
                                 .SetEase(Ease.OutQuad);
    }

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

    public void ChangeState()
    {
        CharacterState = ECharacterState.Init;
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

        if (GUILayout.Button("ChangeState"))
        {
            player.ChangeState();
        }
    }
}
#endif