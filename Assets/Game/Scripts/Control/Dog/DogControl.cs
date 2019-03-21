using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogControl : BaseControl
{
    public Transform camp1;
    public Transform camp2;

    public void SetCamp(Transform camp1, Transform camp2)
    {
        this.camp1 = camp1;
        this.camp2 = camp2;
    }

    public override void Move(Vector3 point)
    {
        point.y = transform.position.y;
        //寻路
        agent.ResetPath();
        agent.SetDestination(point);
        state = AnimState.WALK;
    }

    protected override void Update()
    {
        //检查寻路是否终止
        if (state == AnimState.WALK && !IsMoving)
        {
            state = AnimState.FREE;
            PoolManager.Instance.HideObjet(gameObject);
        }
    }

    public override void RequestAttack()
    {

    }
    /// <summary>
    /// 同步攻击动画(不计算伤害，就是显示动画的)
    /// </summary>
    /// <param name="target"></param>
    public override void AttackResponse(params Transform[] target)
    {

    }

    public override void DeathResponse()
    {
        PoolManager.Instance.HideObjet(gameObject);
    }
}
