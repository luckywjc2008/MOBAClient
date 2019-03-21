﻿using System.Collections;
using System.Collections.Generic;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 所有战斗模型的控制器基类
/// </summary>
public class BaseControl : MonoBehaviour
{
    /// <summary>
    /// 物体数据模型
    /// </summary>
    public DogModel Model { get; set; }
    /// <summary>
    /// 目标
    /// </summary>
    [SerializeField]
    protected BaseControl target;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="model"></param>
    /// <param name="friend"></param>
    public virtual void Init(DogModel model, bool friend)
    {
        this.Model = model;
        hpControl.setColor(friend);
        OnHpChange();
        //根据 布尔变量来设置其层级
        string layer = friend ? "Friend" : "Enemy";
        gameObject.layer = LayerMask.NameToLayer(layer);

    }

    #region 动画
    [SerializeField]
    protected AnimControl animControl;

    /// <summary>
    /// 当前的动画状态
    /// </summary>
    protected AnimState state = AnimState.FREE;



    #endregion

    #region 血量
    [SerializeField]
    protected HpControl hpControl;
    /// <summary>
    /// 血量改变
    /// </summary>
    public void OnHpChange()
    {
       hpControl.SetHp((float)Model.CurrHp/Model.MaxHp);
    }

    #endregion

    #region 移动控制
    [SerializeField]
    protected NavMeshAgent agent;
    /// <summary>
    /// 是否正在移动
    /// </summary>
    protected bool IsMoving
    {
        get {return agent.pathPending 
                || agent.remainingDistance > agent.stoppingDistance 
                || agent.velocity != Vector3.zero 
                || agent.pathStatus != NavMeshPathStatus.PathComplete; }
    }

    /// <summary>
    /// 移动
    /// </summary>
    public virtual void Move(Vector3 point)
    {
        if (state == AnimState.DEATH)
        {
            return;
        }

        point.y = transform.position.y;
        //寻路
        agent.ResetPath();
        agent.SetDestination(point);
        //播放动画
        animControl.Walk();
        state = AnimState.WALK;
    }

    protected virtual void Update()
    {
        //检查寻路是否终止
        if(state == AnimState.WALK && !IsMoving)
        {

            animControl.Free();
            state  = AnimState.FREE;     
        }
    }

    #endregion

    #region 攻击控制
    
    //选择人物 直接攻击 计算伤害
    
    //选择人物 给服务器发送一个要攻击的ID，先同步攻击动画，在播放到关键帧的时候在服务器计算伤害，然后同步伤害给每个客户端

    /// <summary>
    /// 请求攻击
    /// </summary>
    public virtual void RequestAttack()
    {

    }

    /// <summary>
    /// 攻击响应
    /// </summary>
    public virtual void AttackResponse(params Transform[] target)
    {

    }

    /// <summary>
    /// 技能响应
    /// </summary>
    public virtual void SkillResponse(int skillId,Transform target,Vector3 targetPos)
    {

    }

    #endregion


    #region 音效
    [SerializeField]
    protected AudioSource audioSource;
    /// <summary>
    /// 音效名称和音效文件
    /// </summary>
    protected Dictionary<string,AudioClip> nameClipDict = new Dictionary<string, AudioClip>();
    protected virtual void Start()
    {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }
    /// <summary>
    /// 根据状态播放音效
    /// </summary>
    protected void PlayAudio(string name )
    {
        switch (name)
        {
            case "Attack":
                audioSource.clip = nameClipDict["Attack"];
                audioSource.Play();
                break;
            case "Death":
                audioSource.clip = nameClipDict["Death"];
                audioSource.Play();
                break;
            default:
                break;
        }

    }

    #endregion

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void DeathResponse()
    {

    }
    /// <summary>
    /// 复活
    /// </summary>
    public virtual void ResurgeResponse()
    {
    }


}
