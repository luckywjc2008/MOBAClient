using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Config;
using MobaCommon.Dto;
using UnityEngine;

public class Soldier : BaseControl,IResourceListener
{

    protected override void Start()
    {
        base.Start();
        ResourceManager.Instance.Load(Paths.RES_SOUND_FIGHT + "SoldierAttack",typeof(AudioClip),this);
        ResourceManager.Instance.Load(Paths.RES_SOUND_FIGHT + "SoldierDeath", typeof(AudioClip), this);
    }

    public void OnLoaded(string assetName, object asset)
    {
        if (assetName == Paths.RES_SOUND_FIGHT + "SoldierAttack")
        {
            nameClipDict.Add("Attack",asset as AudioClip);    
        }
        else if (assetName == Paths.RES_SOUND_FIGHT + "SoldierDeath")
        {
            nameClipDict.Add("Death", asset as AudioClip);
        }
    }

    //动画播放完毕 计算伤害
    public override void RequestAttack()
    {
        if (state == AnimState.DEATH)
        {
            return;
        }
        //如果不是自身发起的攻击 那么return
        if (this != GameData.myControl)
        {
            return;
        }

        PlayAudio("Attack");

        //获取目标Id
        int targetId = target.GetComponent<BaseControl>().Model.Id;
        int myId = GameData.myControl.Model.Id;

        //发起计算伤害的请求 参数1技能Id，目标id
        PhotonManager.Instance.Request(OpCode.FightCode,OpFight.Damage, myId, 1,new int[] {targetId});
    }
    /// <summary>
    /// 同步攻击动画(不计算伤害，就是显示动画的)
    /// </summary>
    /// <param name="target"></param>
    public override void AttackResponse(params Transform[] target)
    {
        if (state == AnimState.DEATH)
        {
            return;
        }
        //保存目标
        this.target = target[0].GetComponent<BaseControl>();
        //要面向攻击目标
        transform.LookAt(target[0]);
        //设置攻击状态
        this.state = AnimState.ATTACK;
    }

    /// <summary>
    /// 同步技能动画(不计算伤害，就是显示动画的)
    /// </summary>
    /// <param name="target"></param>
    public override void SkillResponse(int skillId,Transform target,Vector3 targetPos)
    {
        switch (skillId)
        {
            case 1001:
                break;
            case 1002:
                break;
            case 1003:
                targetPos.y = transform.position.y;
                transform.LookAt(targetPos);
                //加载技能攻击特效
                GameObject go = PoolManager.Instance.GetObject("E");
                SkillModel model = ((HeroModel) this.Model).Skills[2];
                go.GetComponent<LineSkill>().Init(transform,(float)model.Distance,8f,skillId,this.Model.Id, this == GameData.myControl);
                break;
            case 1004:
                break;
            default:
                break;     
        }
    }

    protected override void Update()
    {
        base.Update();

        if (target != null && this.state == AnimState.ATTACK)
        {
            //先检查攻击范围
            float distance = Vector3.Distance(transform.position, target.transform.position);
            //超过攻击范围，走到最近的一个点，然后攻击
            if (distance > Model.AttackDistance)
            {  
                //agent.ResetPath();
                //向目标走动
                agent.SetDestination(target.transform.position);
                //播放动画
                animControl.Walk();
            }
            else//在攻击的范围内 直接攻击
            {
                //停止寻路
                agent.isStopped = true;
                //播放动画
                animControl.Attack();
                //改变状态
                this.state = AnimState.FREE;
            }
        }
    }

    public override void DeathResponse()
    {
        //停止寻路
        agent.isStopped = true;
        //播放动画
        animControl.Death();
        //改变状态
        this.state = AnimState.DEATH;

        PlayAudio("Death");
    }

    public override void ResurgeResponse()
    {
        this.state = AnimState.FREE;
        this.OnHpChange();
    }

}
