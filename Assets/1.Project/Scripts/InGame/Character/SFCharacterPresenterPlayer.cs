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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
            item.InitDataSet(gameObject.GetInstanceID());
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
            RotateView(Vector3.zero);
        });

        var skillInitData = new SLSkillInitData();
        skillInitData.ownerID = gameObject.GetInstanceID();
        skillInitData.skillKey = currentActionData.key;

        var addOneData = abilityComponent.CreateFactorByMeta("Skill_Punch");
        skillInitData.factorSet.Add(addOneData);

        var addTwoData = abilityComponent.CreateFactorByMeta("Skill_Slow");
        skillInitData.factorSet.Add(addTwoData);

        view.currentSkillObject.InitSkillData(skillInitData);
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
        ZoomCharacter();
        RotateCharacter();
        MoveCharacter();
        InputCheck();
    }

    public Transform camZoomer = null;
    public void ZoomCharacter()
    {
        var zoomValue = Input.GetAxis("Mouse ScrollWheel") * 1.0f;
        var result = camZoomer.localPosition.z + zoomValue;
        if (result < 0)
        {
            result = 0;
        }

        if (result > 1.5)
        {
            result = 1.5f;
        }

        camZoomer.localPosition = new Vector3(camZoomer.localPosition.x, camZoomer.localPosition.y, result);
    }

    public Transform camPivot = null;
    private void RotateCharacter()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0 || mouseY != 0)
        {
            var rotationY = mouseX == 0 ? camPivot.localRotation.eulerAngles.y : camPivot.localRotation.eulerAngles.y + mouseX;
            var rotationX = mouseY == 0 ? camPivot.localRotation.eulerAngles.x : camPivot.localRotation.eulerAngles.x - mouseY;
            var resultRotatin = new Vector3(rotationX, rotationY, camPivot.localRotation.z);
            camPivot.localRotation = Quaternion.Euler(resultRotatin);
        }
    }

    private Vector3 prevDirection = Vector3.zero;
    private Tween rotateTween = null;

    private void MoveCharacter()
    {
        float rad = camPivot.localRotation.eulerAngles.y * Mathf.Deg2Rad;
        float z = Mathf.Cos(rad);
        float x = Mathf.Sin(rad);
        Vector3 direction = new Vector3(x, 0, z);
        Vector3 vecticalDirection = Vector3.Cross(direction, Vector3.up);

        Vector3 movePosition = Vector3.zero;
        movePosition -= Input.GetKey(KeyCode.D) ? vecticalDirection : Vector3.zero;
        movePosition += Input.GetKey(KeyCode.A) ? vecticalDirection : Vector3.zero;
        movePosition += Input.GetKey(KeyCode.W) ? direction : Vector3.zero;
        movePosition -= Input.GetKey(KeyCode.S) ? direction : Vector3.zero;
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

        var speed = GetFactor("MoveSpeed");
        transform.localPosition += direction * speed * Time.deltaTime;

        if (CharacterState == ECharacterState.Walk)
        {
            if (prevDirection != direction)
            {
                RotateView(direction);
            }
        }

        prevDirection = direction;
    }

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
    }

    private void RotateView(Vector3 direction)
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

        //if (GUILayout.Button("ApplyTestMetaFactorDamage"))
        //{
        //    player.ApplyTestMetaFactorDamage();
        //}
    }
}
#endif