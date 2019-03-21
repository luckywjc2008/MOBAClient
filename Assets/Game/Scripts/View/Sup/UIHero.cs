using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using MobaCommon.Config;
using UnityEngine;
using UnityEngine.UI;

public class UIHero : MonoBehaviour,IResourceListener
{
    [SerializeField]
    private Image imgHead;
    [SerializeField]
    private Button btnHead;
    private int heroId;
    public int HeroId
    {
        get { return heroId; }
        set { heroId = value; }
    }
    private string heroName;
    private AudioClip ac;
    private AudioClip acSelect;

    /// <summary>
    /// 初始化视图
    /// </summary>
    public void InitView(HeroDataModel hero)
    {
        //保存Id
        this.heroId = hero.TypeId;
        //加载音效
        this.heroName = hero.Name;
        string soundPath = Paths.RES_SOUND_SELECT + heroName;
        ResourceManager.Instance.Load(soundPath,typeof(AudioClip),this);
        //更新头像
        string path = Paths.RES_HEAD + heroName;
        ResourceManager.Instance.Load(path, typeof(Sprite), this);

        ResourceManager.Instance.Load(Paths.RES_SOUND_UI + "Select", typeof(AudioClip), this);
    }

    public void OnLoaded(string assetName, object asset)
    {
        if (assetName == Paths.RES_HEAD + heroName)
        {
            this.imgHead.sprite = asset as Sprite;
        }
        else if(assetName == Paths.RES_SOUND_SELECT + heroName)
        {
            this.ac = asset as AudioClip;
        }else if (assetName == Paths.RES_SOUND_SELECT + "Select")
        {
            this.acSelect = asset as AudioClip;
        }
    }
    /// <summary>
    /// 英雄是否可以选择
    /// </summary>
    public bool Interactable
    {
        get { return btnHead.interactable; }
        set { btnHead.interactable = value; }
    }

    /// <summary>
    /// 选择英雄事件
    /// </summary>
    public void OnClick()
    {
        SoundManager.Instance.PlayEffectMusic(acSelect);
        //播放音效
        SoundManager.Instance.PlayEffectMusic(ac);
        //发起选人的请求
        PhotonManager.Instance.Request(OpCode.SelectCode,OpSelect.Select,heroId);
    }
}
