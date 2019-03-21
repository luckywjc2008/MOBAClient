using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using MobaCommon.Code;
using MobaCommon.Config;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.UI;

public class SelectView : UIBase,IResourceListener
{
    [SerializeField]
    private UIPlayer[] team1;
    [SerializeField]
    private UIPlayer[] team2;
    [SerializeField]
    private Button btnReady;

    private AudioClip acClick;
    private AudioClip acReady;


    #region UIBase
    public override void Init()
    {
        acClick = ResourceManager.Instance.GetAsset(Paths.RES_SOUND_UI + "Click") as AudioClip;
        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "Ready",typeof(AudioClip),this);
    }

    public void OnLoaded(string assetName, object asset)
    {
        if (assetName == Paths.RES_SOUND_UI + "Ready")
        {
            acReady = asset as AudioClip;
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        //发起进入请求
        PhotonManager.Instance.Request(OpCode.SelectCode, OpSelect.Enter);
        //初始化玩家拥有英雄列表
        this.InitSelectHeroPanel(GameData.Player.heroIds);
        talkView.OnShow();
    }

    public override void OnDestroy()
    {

    }

    public override string UIName()
    {
        return UIDefind.UISelect;
    }
    #endregion
    /// <summary>
    /// 更新显示视图
    /// </summary>
    /// <param name="myTeam">自身队伍</param>
    /// <param name="team1"></param>
    /// <param name="team2"></param>
    public void UpdateView(int myTeam,SelectModel[] team1,SelectModel[] team2)
    {
        int myPlayerId = GameData.Player.id;
        SelectModel myModel = null;
        List<int> selectHero = new List<int>();
        if (myTeam == 1)
        {
            for (int i = 0; i < team1.Length; i++)
            {
                this.team1[i].UpdateView(team1[i]);

            }
            for (int i = 0; i < team2.Length; i++)
            {
                this.team2[i].UpdateView(team2[i]);
            }
            //禁用一个队伍已经选择了的英雄
            foreach (var item in team1)
            {
                if (item.heroId != -1)
                {
                    selectHero.Add(item.heroId);
                }
                //处理选择确定了英雄，就不能再选择了
                if (item.playerId == myPlayerId)
                {
                    myModel = item;
                }
            }
        }else if (myTeam == 2)
        {
            for (int i = 0; i < team2.Length; i++)
            {
                this.team1[i].UpdateView(team2[i]);
            }
            for (int i = 0; i < team1.Length; i++)
            {
                this.team2[i].UpdateView(team1[i]);
            }
            //禁用一个队伍已经选择了的英雄
            foreach (var item in team2)
            {
                if (item.heroId != -1)
                {
                    selectHero.Add(item.heroId);
                }
                //处理选择确定了英雄，就不能再选择了
                if (item.playerId == myPlayerId)
                {
                    myModel = item;
                }
            }
        }

        //禁用英雄
        foreach (UIHero item in idHeroDict.Values)
        {
            //如果当前这个英雄已经被选择了 或者玩家已准备
            if (selectHero.Contains(item.HeroId) || myModel.isReady == true)
            {
                item.Interactable = false;
            }
            else
            {
                item.Interactable = true;
            }
        }

        if (myModel.isReady == true)
        {
            btnReady.interactable = false;
        }
        else
        {
            btnReady.interactable = true;
        }

    }
    [Header("英雄选择预设")]
    [SerializeField]
    private GameObject UIHero;

    [SerializeField]
    private Transform heroParent;

    /// <summary>
    /// 已经加载的英雄组件
    /// </summary>
    private Dictionary<int,UIHero> idHeroDict = new Dictionary<int, UIHero>();
    /// <summary>
    /// 初始化英雄选择面板
    /// </summary>
    /// <param name="heroIds"></param>
    public void InitSelectHeroPanel(int[] heroIds)
    {
        GameObject go;
        for (int i = 0; i < heroIds.Length; i++)
        {
            if (idHeroDict.ContainsKey(heroIds[i]))
            {
                continue;
            }
            go = Instantiate(UIHero);
            UIHero hero = go.GetComponent<UIHero>();
            hero.InitView(HeroData.GetHeroData(heroIds[i]));
            go.transform.SetParent(heroParent,false);
            go.transform.localScale = Vector3.one;
            idHeroDict.Add(heroIds[i], hero);
        }
    }
     /// <summary>
     /// 确认选择
     /// </summary>
    public void ReadySelect()
    {
        SoundManager.Instance.PlayEffectMusic(acReady);
        btnReady.interactable = false;
        //给服务器发送确认选择
        PhotonManager.Instance.Request(OpCode.SelectCode,OpSelect.Ready);
    }

    #region 聊天
    [Header("聊天")]
    [SerializeField]
    private TalkView talkView;

    public void TalkAppend(string context)
    {
        talkView.TalkAppend(context);
    }

    #endregion

}
