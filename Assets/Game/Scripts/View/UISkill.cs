using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkill : MonoBehaviour, IResourceListener,IPointerEnterHandler,IPointerExitHandler
{
    #region 字段
    /// <summary>
    /// 技能信息
    /// </summary>
    public SkillModel Skill;
    /// <summary>
    /// 技能图标
    /// </summary>
    [SerializeField]
    private Image imgSkill;
    /// <summary>
    /// 遮罩
    /// </summary>
    [SerializeField]
    private Image imgMask;
    /// <summary>
    /// 升级按钮
    /// </summary>
    [SerializeField]
    private Button btnUp;
    /// <summary>
    /// 设置升级按钮是否可以点
    /// </summary>
    public bool UpInteractable
    {
        set { btnUp.interactable = value; }
    }
    /// <summary>
    /// 技能是否可以用
    /// </summary>
    private bool canUse;
    /// <summary>
    /// 技能是否可以用
    /// </summary>
    public bool CanUse
    {
        get { return canUse; }
    }

    /// <summary>
    /// 冷却时间
    /// </summary>
    private float cdTime;
    /// <summary>
    /// 当前时间
    /// </summary>
    private float curTime;
    #endregion

    void Update()
    {
        //技能不可以用时计算冷却
        if (!canUse)
        {
            curTime -= Time.deltaTime;
            if (curTime <= 0)
            {
                canUse = true;
                curTime = 0;
                cdTime = 0;
                imgMask.gameObject.SetActive(false);
            }
            else
            {    //赋值 角度0-1
                imgMask.fillAmount = curTime/cdTime;
            }
        }
    }

    public void Use(int cd)
    {
        if (!canUse)
        {
            return;
        }

        cdTime = cd;
        curTime = cd;
        //显示遮罩层
        imgMask.gameObject.SetActive(true);
        canUse = false;
    }
     /// <summary>
     /// 隐藏遮罩层
     /// </summary>
    public void Reset()
    {
        if (!canUse)
        {
            return;
        }
        if (Skill.Level > 0)
        {
            imgMask.gameObject.SetActive(false);
        }
    }



    public void OnLoaded(string assetName, object asset)
    {
       imgMask.sprite = asset as Sprite;
       imgSkill.sprite = asset as Sprite; 
    }
    /// <summary>
    /// 鼠标进入时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //显示技能的提示信息
    }
    /// <summary>
    /// 鼠标离开时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        //关闭技能的提示信息
    }
    /// <summary>
    /// 初始化技能
    /// </summary>
    /// <param name="skill"></param>
    public void Init(SkillModel skill)
    {
        //保存数据
        this.Skill = skill;
        //加载图片
        ResourceManager.Instance.Load(Paths.RES_SKILL + skill.Id,typeof(Sprite),this);
        //显示遮罩
        if (skill.Level <= 0)
        {
            imgMask.gameObject.SetActive(true);
        }

    }
    /// <summary>
    /// 点击升级按钮
    /// </summary>
    public void OnUpClick()
    {
        //想服务器发起升级该Id的技能
       PhotonManager.Instance.Request(OpCode.FightCode,OpFight.SkillUp,this.Skill.Id); 
    }
}
