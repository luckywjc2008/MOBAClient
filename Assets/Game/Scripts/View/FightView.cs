using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightView : MonoBehaviour ,IResourceListener
{
    #region 字段
    /// <summary>
    /// 头像
    /// </summary>
    [SerializeField]
    private Image imgHead;
    /// <summary>
    /// 经验条
    /// </summary>
    [SerializeField]
    private Slider barExp;
    /// <summary>
    /// 等级
    /// </summary>
    [SerializeField]
    private Text txtLv;
    /// <summary>
    /// 血条
    /// </summary>
    [SerializeField]
    private Slider barHp;
    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField]
    private Text txtHp;
    /// <summary>
    /// 蓝条
    /// </summary>
    [SerializeField]
    private Slider barMp;
    /// <summary>
    /// 蓝量
    /// </summary>
    [SerializeField]
    private Text txtMp;
    /// <summary>
    /// 统计面板
    /// </summary>
    [SerializeField]
    private Text txtKDA;
    /// <summary>
    /// 钱
    /// </summary>
    [SerializeField]
    private Text txtMoney;
    /// <summary>
    /// 攻击力
    /// </summary>
    [SerializeField]
    private Text txtAttack;
    /// <summary>
    /// 防御力
    /// </summary>
    [SerializeField]
    private Text txtDefense;
    /// <summary>
    /// 技能
    /// </summary>
    [SerializeField]
    private UISkill[] skills;
    #endregion

    void Start()
    {
        //释放不必要的资源
        ResourceManager.Instance.ReleaseAll();
        //加载战斗场景背景音乐
        ResourceManager.Instance.Load(Paths.RES_SOUND_FIGHT + "FightBGM",typeof(AudioClip),this);
        //向服务器发起已经进入请求
        PhotonManager.Instance.Request(OpCode.FightCode, OpFight.Enter, GameData.Player.id);
    }

    #region 更新视图
    /// <summary>
    /// 初始化视图显示
    /// </summary>
    public void InitView(HeroModel hero)
    {
        //头像
        ResourceManager.Instance.Load(Paths.RES_HEAD + hero.Name, typeof(Image), this);
        //血条
        barHp.value = (float)hero.CurrHp / hero.MaxHp;
        txtHp.text = string.Format("{0}/{1}", hero.CurrHp, hero.MaxHp);
        //蓝条
        barMp.value = (float)hero.CurMp / hero.MaxMp;
        txtMp.text = string.Format("{0}/{1}", hero.CurMp, hero.MaxMp);
        //经验
        barExp.value = (float)hero.Exp / (hero.Level * 100);
        txtLv.text = string.Format("Lv.{0}", hero.Level);
        //统计
        txtKDA.text = string.Format("Kill:{0}     Dead:{1}", hero.Kill, hero.Dead);
        txtMoney.text = hero.Money.ToString();
        //属性
        txtAttack.text = hero.Attack.ToString();
        txtDefense.text = hero.Defense.ToString();

        //更新技能列表
        for (int i = 0; i < skills.Length; i++)
        {
            UISkill skill = skills[i];
            skill.Init(hero.Skills[i]);
        }
    }

    /// <summary>
    /// 更新显示
    /// </summary>
    /// <param name="hero"></param>
    public void UpdateView(HeroModel hero)
    {
        //血条
        barHp.value = (float)hero.CurrHp / hero.MaxHp;
        txtHp.text = string.Format("{0}/{1}", hero.CurrHp, hero.MaxHp);
        //死亡检查
        if (hero.CurrHp == 0)
        {
            //显示灰白屏幕
            //TODO
        }

        //蓝条
        barMp.value = (float)hero.CurMp / hero.MaxMp;
        txtMp.text = string.Format("{0}/{1}", hero.CurMp, hero.MaxMp);
        //经验
        barExp.value = (float)hero.Exp / (hero.Level * 100);
        txtLv.text = string.Format("Lv.{0}", hero.Level);
        //统计
        txtKDA.text = string.Format("Kill:{0}     Dead:{1}", hero.Kill, hero.Dead);
        txtMoney.text = hero.Money.ToString();
        //属性
        txtAttack.text = hero.Attack.ToString();
        txtDefense.text = hero.Defense.ToString();

        //更新技能列表
        for (int i = 0; i < skills.Length; i++)
        {
            UISkill skill = skills[i];
            skill.Init(hero.Skills[i]);
        }
    }
     /// <summary>
     /// 更新技能
     /// </summary>
     /// <param name="model"></param>
    public void UpdateSkills(HeroModel model)
    {
        //更新技能列表
        for (int i = 0; i < model.Skills.Length; i++)
        {
            SkillModel skill = model.Skills[i];
            if (model.Level >= skill.LearnLevel)
            {
                //要显示升级按钮
                if (model.SkillPoints > 0)
                {
                    skills[i].UpInteractable = true;
                }
                else
                {
                    skills[i].UpInteractable = false;
                }
            }
            else
            {
                skills[i].UpInteractable = false;
            }
            //保存技能的信息
            skills[i].Skill = skill;
            //隐藏遮罩
            skills[i].Reset();
        }
    }
    /// <summary>
    /// 刷新技能冷却
    /// </summary>
    public void UpdateCoolDown(int skillId)
    {
        foreach (var item in this.skills)
        {
            if (item.Skill.Id == skillId)
            {
                item.Use(item.Skill.CoolDown);
                break;
            }
        }
    }

    #endregion  

    public void OnLoaded(string assetName, object asset)
    {
        if ( assetName == Paths.RES_SOUND_FIGHT + "FightBGM")
        {
            SoundManager.Instance.PlayBgMusic(asset as AudioClip);
            SoundManager.Instance.BGVolume = 1f;
        }
        else
        {
            imgHead.sprite = asset as Sprite;
        }
    }

    #region 商店
   [SerializeField]
    private Image ItemPanel;

    public void OnItemPanelClick()
    {
        bool active = ItemPanel.gameObject.activeSelf;
        ItemPanel.gameObject.SetActive(!active);
    }

    #endregion

    #region 游戏结束
    [SerializeField]
    private Image WinPanel;
    [SerializeField]
    private Image LosePanel;

    public void GameOver(bool isWin)
    {
        if (isWin)
        {
            WinPanel.gameObject.SetActive(true);            
        }
        else
        {
            LosePanel.gameObject.SetActive(true);
        }
    }

    public void OnExitClick()
    {
        SceneManager.LoadScene(0);
    }

    #endregion
}
