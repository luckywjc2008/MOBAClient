using System;
using System.Collections;
using System.Collections.Generic;
using MobaCommon.Config;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour,IResourceListener
{

    private Text txtName;
    private Image imgBg;
    private Text txtState;
    private Image imgHead;

    void Start()
    {
        txtName = transform.Find("txtName").GetComponent<Text>();
        imgBg = transform.GetComponent<Image>();
        txtState = transform.Find("Text").GetComponent<Text>();
        imgHead = transform.Find("imgHead").GetComponent<Image>();
    }

    public void UpdateView(SelectModel model)
    {
        txtName.text = model.playerName;
        imgBg.color = Color.white;
        //判断玩家是否进入
        if (!model.isEnter) 
        {  
            ResourceManager.Instance.Load(Paths.RES_HEAD + "no-Connect", typeof(Sprite), this);
            return;
        }

        //进入之后
        if (model.heroId != -1)
        {
            string path = Paths.RES_HEAD + HeroData.GetHeroData(model.heroId).Name;
            ResourceManager.Instance.Load(path, typeof(Sprite), this);
        }
        else
        {
            ResourceManager.Instance.Load(Paths.RES_HEAD + "no-Select", typeof(Sprite), this);
        }

        if (model.isReady)
        {
            imgBg.color = Color.green;
            txtState.text = "已选择";
        }
        else
        {
            imgBg.color = Color.white;
            txtState.text = "正在选择...";
        }
    }

    public void OnLoaded(string assetName, object asset)
    {
       this.imgHead.sprite = asset as Sprite;
    }
}
