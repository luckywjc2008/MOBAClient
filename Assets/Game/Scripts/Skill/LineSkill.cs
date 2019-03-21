using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using UnityEngine;

public class LineSkill : MonoBehaviour
{
    /// <summary>
    /// 技能使用者
    /// </summary>
    private Transform user = null;
    /// <summary>
    /// 距离
    /// </summary>
    private float distance;
    /// <summary>
    /// 已经移动的距离
    /// </summary>
    private float curDistance;
    /// <summary>
    /// 速度
    /// </summary>
    private float speed;

    /// <summary>
    /// 技能Id
    /// </summary>
    private int skillId;
    /// <summary>
    /// 攻击者Id
    /// </summary>
    private int attackId;

    /// <summary>
    /// 是否需要发送
    /// </summary>
    private bool send;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="user"></param>
    /// <param name="distance"></param>
    /// <param name="speed"></param>
    /// <param name="skillId"></param>
    /// <param name="attackId"></param>
    /// <param name="send"></param>
    public void Init(Transform user,float distance,float speed,int skillId,int attackId,bool send)
    {
        this.user = user;

        this.transform.position = user.position;
        this.transform.rotation = user.rotation;
        this.curDistance = 0;
        this.idList.Clear();

        this.distance = distance;
        this.speed = speed;
        this.skillId = skillId;
        this.attackId = attackId;
        this.send = send;
    }

    void Update()
    {
        if (user == null)
        {
            return;
        }
        Vector3 translation = Vector3.forward * speed * Time.deltaTime;
        transform.Translate(translation);
        curDistance += translation.z;
        //达到最大距离就回收到资源池
        if (curDistance >= distance)
        {
            PoolManager.Instance.HideObjet(gameObject);
            //发送伤害请求
            if (send)
            {
                send = false;
                PhotonManager.Instance.Request(OpCode.FightCode, OpFight.Damage, this.attackId, this.skillId, idList.ToArray());
            }
           
        }
    }

    private List<int> idList = new List<int>();
    void OnTriggerEnter(Collider other)
    {
        //计算伤害
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
        {
            idList.Add(other.GetComponent<BaseControl>().Model.Id);
        }
    }

}
