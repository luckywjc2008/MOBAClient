using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using UnityEngine;
using UnityEngine.UI;

public class TalkView : MonoBehaviour
{
    [SerializeField]
    private Text txtContext;
    [SerializeField]
    private InputField inTalk;
    [SerializeField]
    private Scrollbar bar;
    /// <summary>
    /// 初始化聊天框
    /// </summary>
    public void OnShow()
    {
        txtContext.text = string.Empty;
    }

    public void OnSendClick()
    {
        AudioClip acSelect = ResourceManager.Instance.GetAsset(Paths.RES_SOUND_SELECT + "Click") as AudioClip;
        SoundManager.Instance.PlayEffectMusic(acSelect);
        string text = inTalk.text;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        //给服务器发送聊天请求
        PhotonManager.Instance.Request(OpCode.SelectCode,OpSelect.Talk,text);
        //清除上次输入
        inTalk.text = string.Empty;
    }

    public void TalkAppend(string context)
    {
        txtContext.text += "\n" + context;
        //每次聊天都显示最后一行
        bar.value = 0;
    }
}
