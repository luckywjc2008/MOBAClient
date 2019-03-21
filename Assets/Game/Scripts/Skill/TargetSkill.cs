using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using UnityEngine;
/// <summary>
/// 选择目标型的技能
/// </summary>
public class TargetSkill : MonoBehaviour
{
    //目标 
    private Transform target;
    /// <summary>
    /// 技能Id
    /// </summary>
    private int skillId;
    /// <summary>
    /// 攻击者Id
    /// </summary>
    private int attackId;
    /// <summary>
    /// 目标Id
    /// </summary>
    private int targetId;
    /// <summary>
    /// 是否需要发送
    /// </summary>
    private bool send;
    /// <summary>
    /// 移动速度
    /// </summary>
    private float speed = 10f;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="target"></param>
    /// <param name="skillId"></param>
    /// <param name="attackId"></param>
    /// <param name="targetId"></param>
    /// <param name="send">是否发送</param>
    public void Init(Transform target, int skillId, int attackId, int targetId, bool send)
    {
        this.target = target;
        this.skillId = skillId;
        this.attackId = attackId;
        this.targetId = targetId;
        this.send = send;
    }

    void Update()
    {
        //检测是否有目标
        if (target == null)
        {
            return;
        }
        //差值移动的效果 (先找到方向向量 在归1化然后计算飞行速度)
        transform.position = transform.position -((transform.position - target.position)/(Vector3.Distance(transform.position, target.position)) * speed * Time.deltaTime);
        float d = Vector3.Distance(transform.position, target.position);
        if (d > 1f)
        {
            return;
        }
        if (send)
        {
            //防止重复发送
            send = false;
            //发起伤害请求
            PhotonManager.Instance.Request(OpCode.FightCode,OpFight.Damage,attackId,skillId,new int[]
            {
                targetId,
            }
            );
        }
        //回收到资源池
        PoolManager.Instance.HideObjet(gameObject);
    }

}
