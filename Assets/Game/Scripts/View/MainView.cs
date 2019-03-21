using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.UI;

public class MainView : UIBase,IResourceListener
{
    private AudioClip clickClip;
    [HideInInspector]
    public AudioClip acCountDown;
    
    #region UIBase
    public override string UIName()
    {
        return UIDefind.UIMain;
    }

    public override void Init()
    {
        clickClip =  ResourceManager.Instance.GetAsset(Paths.GetSoundFullName("Click")) as AudioClip;
        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "CountDown", typeof(AudioClip), this);
        //添加按钮点击
        btnCreate.onClick.AddListener(OnCreateClick);
        //向服务器获取角色信息
        PhotonManager.Instance.Request(OpCode.PlayerCode, OpPlayer.GetInfo);
    }

    public void OnLoaded(string assetName, object asset)
    {
        if (assetName == Paths.RES_SOUND_UI + "CountDown")
        {
            acCountDown = asset as AudioClip;
        }
    }

    public override void OnDestroy()
    {

    }
    #endregion

    #region 创建模块

    [Header("创建模块")]
    [SerializeField]
    private InputField inName;

    [SerializeField]
    private Button btnCreate;

    [SerializeField]
    private GameObject createPanel;
    /// <summary>
    /// 创建按钮的可点击状态
    /// </summary>
    public bool CreateInteractable
    {
        set { btnCreate.interactable = value; }
    }

    public void OnCreateClick()
    {   
        //播放点击声音
        SoundManager.Instance.PlayEffectMusic(clickClip);
        //输入检查
        if (string.IsNullOrEmpty(inName.text))
        {
           return;
        }
        //发起创建请求
        PhotonManager.Instance.Request(OpCode.PlayerCode,OpPlayer.Create,inName.text);
        inName.text = string.Empty;
        CreateInteractable = false;
    }
    /// <summary>
    /// 创建面板可见
    /// </summary>
    public bool CreatePanelActive
    {
        set
        {
            createPanel.SetActive(value);
        }
    }

    #endregion

    [Header("角色信息")]
    [SerializeField]
    private Text txtName;
    [SerializeField]
    private Slider barExp;

    /// <summary>
    /// 更新显示
    /// </summary>
    public void UpdateView(PlayerDto player)
    {
        txtName.text = player.name;
        barExp.value = (float)player.exp/(player.lv*100);

        Friend[] friends = player.friends;
        friendList.Clear();
        for (int i = 0; i < friendTran.childCount; i++)
        {
            GameObject children = friendTran.GetChild(i).gameObject;
            GameObject.Destroy(children);
        }
        for (int i = 0; i < friends.Length; i++)
        {
            Friend friend = friends[i];
            GameObject go = Instantiate(UIFriend);
            go.transform.SetParent(friendTran,false);
            FriendView fv = go.GetComponent<FriendView>();
            fv.InitView(friend.Id, friend.Name, friend.IsOnLine);
            friendList.Add(fv);
        }

    }

    #region 好友模块
    [SerializeField]
    private Transform friendTran;

    [Header("好友信息的预设")]
    [SerializeField]
    private GameObject UIFriend;

    [SerializeField]
    private InputField inAddName;

    private List<FriendView> friendList = new List<FriendView>();

    /// <summary>
    /// 点击添加好友按钮
    /// </summary>
    public void OnAddClick()
    {
        //播放点击声音
        SoundManager.Instance.PlayEffectMusic(clickClip);
        if (string.IsNullOrEmpty(inAddName.text))
        {
            return;
        }
        //添加好友请求
        PhotonManager.Instance.Request(OpCode.PlayerCode,OpPlayer.RequestAdd,inAddName.text);

    }
    [SerializeField]
    private ToClientAddView toClientAddPanel;
    /// <summary>
    /// 显示好友请求添加面板
    /// </summary>
    public void ShowToClientAddPanel(PlayerDto dto)
    {
       toClientAddPanel.gameObject.SetActive(true);
        toClientAddPanel.UpdateView(dto);

    }
     /// <summary>
     /// 天骄结果点击事件
     /// </summary>
     /// <param name="result"></param>
    public void OnResClick(bool result)
     {
         int id = toClientAddPanel.Id;
        PhotonManager.Instance.Request(OpCode.PlayerCode,OpPlayer.ToClientAdd,result, id);
        toClientAddPanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新好友界面
    /// </summary>
    /// <param name="friendId"></param>
    /// <param name="isOnLine"></param>
    public void UpdateFreindView(int friendId,bool isOnLine)
    {
        for (int i = 0; i < friendList.Count; i++)
        {
            if (friendList[i].Id == friendId)
            {
                friendList[i].UpdateView(isOnLine);
            }
        }
    }

    /// <summary>
    /// 显示好友列表
    /// </summary>
    public void ShowFriendList()
    {
        friendTran.parent.gameObject.SetActive(!friendTran.parent.gameObject.activeSelf);
    }

    #endregion

    #region 匹配模块
    [Header("匹配模块")]
    [SerializeField]
    private Button btnOne;
    [SerializeField]
    private Button btnTwo;
    [SerializeField]
    private MatchView matchView;

    /// <summary>
    /// 开始单人匹配
    /// </summary>
    public void OnOneMatch()
    {
        //播放点击声音
        SoundManager.Instance.PlayEffectMusic(clickClip);
        int id = GameData.Player.id;
        PhotonManager.Instance.Request(OpCode.PlayerCode,OpPlayer.StartMatch, id);

        matchView.StartMatch();

        btnOne.interactable = false;
        btnTwo.interactable = false;
    }
    /// <summary>
    /// 开始多人匹配
    /// </summary>
    public void OnTwoMatch(int[] ids)
    {
        //播放点击声音
        SoundManager.Instance.PlayEffectMusic(clickClip);

        PhotonManager.Instance.Request(OpCode.PlayerCode, OpPlayer.StartMatch, ids);
        matchView.StartMatch();
        btnOne.interactable = false;
        btnTwo.interactable = false;
    }

    /// <summary>
    /// 停止匹配
    /// </summary>
    public void OnStopMatch()
    {
        //播放点击声音
        SoundManager.Instance.PlayEffectMusic(clickClip);
        int id = GameData.Player.id;
        PhotonManager.Instance.Request(OpCode.PlayerCode, OpPlayer.StopMatch, id);

        matchView.StopMatch();
        btnOne.interactable = true;
        btnTwo.interactable = true;
    }

    #endregion

}
