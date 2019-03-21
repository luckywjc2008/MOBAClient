using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;

public class KeyControl : MonoBehaviour
{
    [SerializeField]
    private KeyCode Skill_E = KeyCode.E;
    [SerializeField]
    private UISkill uiSkill_E;

    // Update is called once per frame
    void Update()
    {
        #region 当鼠标右键点击
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mouse = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            for (int i = hits.Length - 1; i >= 0 ; i--)
            {
                RaycastHit hit = hits[i];
                //如果点到了敌方单位就攻击
                if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
                {
                    attack(hit.collider.gameObject);
                    //攻击到了敌方目标，就不再响应其他操作了
                    break;
                }
                //如果点到了地面，那就要移动
                else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
                {
                    Move(hit.point);
                }
            }
        }
        #endregion

        #region 空格聚焦

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //聚焦到自己的英雄
            Camera.main.GetComponent<CameraControl>().FocusOn();
        }


        #endregion

        #region 技能释放

        if (Input.GetKeyDown(Skill_E) && uiSkill_E.CanUse && uiSkill_E.Skill.Level >= 1)
        {
            Vector2 mouse = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit))
            {
                //释放技能
                skill(3,hit.point);
            }
        }
        

        #endregion

    }
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetPos"></param>
    private void skill(int index, Vector3 targetPos)
    {
        HeroModel myHero = GameData.myControl.Model as HeroModel;
        int skillId = myHero.Skills[index - 1].Id;

        int myId = myHero.Id;
        PhotonManager.Instance.Request(OpCode.FightCode, OpFight.Skill, skillId, myId, -1,targetPos.x,targetPos.y,targetPos.z);
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="target"></param>
    private void attack(GameObject target)
    { 
        //向服务器发送攻击请求 参数：技能ID,谁攻击了谁
        int targetId = target.GetComponent<BaseControl>().Model.Id;
        int myId = GameData.myControl.Model.Id;
        PhotonManager.Instance.Request(OpCode.FightCode, OpFight.Skill, 1, myId, targetId,-1f,-1f,-1f);
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="point"></param>
    private void Move(Vector3 point)
    {
        //显示一个特效
        GameObject go = PoolManager.Instance.GetObject("ClickMove");
        go.transform.position = point + new Vector3(0,1f,0);

        //给服务器发一个请求 参数：点的坐标
        PhotonManager.Instance.Request(OpCode.FightCode,OpFight.Walk,point.x,point.y,point.z);
    }
}
