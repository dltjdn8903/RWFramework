using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SLSkillPunchInitData : SLSkillBaseInitData
{
    public Transform moveCharacterTransform;
    public Vector3 moveDirection;
}

public class SLSkillPunch : SLSkillBase
{
    private Coroutine moveCorutine = null;

    public override void InitSkillData<T>(T data)
    {
        base.InitSkillData(data);

        SLSkillPunchInitData punchData = data as SLSkillPunchInitData;

        if (moveCorutine != null)
        {
            StopCoroutine(moveCorutine);
        }
        moveCorutine = StartCoroutine(MoveCharacter(punchData.moveCharacterTransform, punchData.moveDirection));
    }

    IEnumerator MoveCharacter(Transform moveTarget, Vector3 moveDirection)
    {
        yield return new WaitForSeconds(0.1f);

        var moveTime = 0.2f;
        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
            moveTarget.localPosition += moveDirection * 3f * Time.deltaTime;
            yield return null;
        }
    }
}
