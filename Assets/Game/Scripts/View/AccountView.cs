using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class AccountView : UIBase , IResourceListener
{
    private AudioClip bgClip;
    private AudioClip enterClip;
    private AudioClip clickClip;

    private void LoadAudio()
    {
        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "英雄", typeof(AudioClip), this);
        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "EnterGame", typeof(AudioClip), this);
        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "Click", typeof(AudioClip), this);
    }

    public void OnLoaded(string assetName, object asset)
    {
        AudioClip clip = asset as AudioClip;
        switch (assetName)
        {
            case Paths.RES_SOUND_UI + "英雄":
                bgClip = clip;
                SoundManager.Instance.PlayBgMusic(bgClip);
                break;
            case Paths.RES_SOUND_UI + "EnterGame":
                enterClip = clip;
                break;
            case Paths.RES_SOUND_UI + "Click":
                clickClip = clip;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 播放点击声音
    /// </summary>
    public void PlayClickSound()
    {
        SoundManager.Instance.PlayEffectMusic(clickClip);
    }

    #region  注册模块
    [Header("注册模块")]
    [SerializeField]
    private InputField inAcc4Register;
    [SerializeField]
    private InputField inPwd4Register;
    [SerializeField]
    private InputField inPwd4Repeat;

    public void OnResetPanel()
    {
        inAcc4Register.text = string.Empty;
        inPwd4Register.text = string.Empty;
        inPwd4Repeat.text = string.Empty;
    }

    /// <summary>
    /// 注册点击事件
    /// </summary>
    public void OnRegisterClick()
    {
        //判断账号和密码是否是空
        if (string.IsNullOrEmpty(inAcc4Register.text) || string.IsNullOrEmpty(inPwd4Register.text) || !inPwd4Repeat.text.Equals(inPwd4Register.text))
        {
            //告诉客户他非法输入了
            return;
        }

        //播放声音
        SoundManager.Instance.PlayEffectMusic(clickClip);

        AccountDto dto = new AccountDto()
        {
            Account = inAcc4Register.text,
            Password = inPwd4Register.text,
        };
        PhotonManager.Instance.Request(OpCode.AccountCode,OpAccount.Register,dto.Account,dto.Password);
    }

    #endregion

    //ctrl+k+s
    #region 登录模块
    [Header("登录模块")]
    [SerializeField]
    private InputField inAcc4Login;
    [SerializeField]
    private InputField inPwd4Login;
    [SerializeField]
    private Button btnEnter;
    /// <summary>
    /// 进入按钮是否可以点击
    /// </summary>
    public bool EnterInteractable
    {
        set
        {
            btnEnter.interactable = value;
            
        }
    }

    /// <summary>
    /// 进入游戏点击事件
    /// </summary>
    public void OnEnterClick()
    {
        if (string.IsNullOrEmpty(inAcc4Login.text) || string.IsNullOrEmpty(inPwd4Login.text) )
        {
            //提示
            return;
        }

        //播放声音
        SoundManager.Instance.PlayEffectMusic(enterClip);
        //创建传输模型
        AccountDto dto = new AccountDto()
        {
            Account = inAcc4Login.text,
            Password = inPwd4Login.text

        };
        PhotonManager.Instance.Request(OpCode.AccountCode,OpAccount.Login,JsonMapper.ToJson(dto));
        EnterInteractable = false;
    }
    /// <summary>
    /// 重置登录面板的输入
    /// </summary>
    public void ResetLoginInput()
    {
        inAcc4Login.text = String.Empty;
        inPwd4Login.text = String.Empty;
    }

    #endregion

    #region UIBase

    public override string UIName()
    {
        return UIDefind.UIAccount;
    }

    public override void Init()
    {
        LoadAudio();
    }

    public override void OnShow()
    {
        base.OnShow();
        //角色已经登录，不用再登录了
        if (GameData.Player != null )
        {
            UIManager.Instance.HideUIPanel(UIDefind.UIAccount);
            UIManager.Instance.ShowUIPanel(UIDefind.UIMain);
        }
    }

    public override void OnDestroy()
    {
        bgClip = null;
        enterClip = null;
        clickClip = null;
    }

    #endregion

}
