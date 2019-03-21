using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using LitJson;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;

public class PlayerReceiver : MonoBehaviour, IReceiver
{
    [SerializeField]
    private MainView view;
    public void OnReceive(byte subCode, OperationResponse response)
    {
        PlayerDto dto;
        switch (subCode)
        {
            case OpPlayer.GetInfo:
                OnGetInfo(response.ReturnCode);
                break;
            case OpPlayer.Create:
                OnCreate(response.ReturnCode,response.DebugMessage);
                break;
            case OpPlayer.OnLine:
                Log.Debug(response[0].ToString());
                dto = JsonMapper.ToObject<PlayerDto>(response[0].ToString());
                OnLine(dto);
                break;
            case OpPlayer.RequestAdd:
                OnRequestAdd(response.DebugMessage);
                break;
            case OpPlayer.ToClientAdd:
                dto = JsonMapper.ToObject<PlayerDto>(response[0].ToString());
                OnToClientAdd(response,dto);
                break;
            case OpPlayer.FriendOnLine:
                OnFriendOnLine((int)response[0]);
                break;
            case OpPlayer.FriendOffLine:
                OnFriendOffLine((int) response[0]);
                break;
            case OpPlayer.MatchComplete:
                OnMatchComplete();
                break;
            default:
                break;
        }
    }

    private void OnMatchComplete()
    {
        view.OnStopMatch();
        SoundManager.Instance.PlayEffectMusic(view.acCountDown);
        MessageTip.Instance.Show("匹配成功，10s内进入房间！", () =>
        {
            //关闭主界面
            UIManager.Instance.HideUIPanel(UIDefind.UIMain);
            //打开选人界面
            UIManager.Instance.ShowUIPanel(UIDefind.UISelect);

            SoundManager.Instance.StopEffectMusic();
        },10f);
    }

    /// <summary>
     /// 好友下线
     /// </summary>
     /// <param name="friendId"></param>
    private void OnFriendOffLine(int friendId)
    {
       view.UpdateFreindView(friendId,false);
    }
    /// <summary>
    /// 好友上线
    /// </summary>
    /// <param name="friendId"></param>
    private void OnFriendOnLine(int friendId)
    {
        view.UpdateFreindView(friendId, true);
    }

    private void OnRequestAdd(string mess)
    {
        MessageTip.Instance.Show(mess);
    }
    /// <summary>
    /// 收到添加好友请求
    /// </summary>
    /// <param name="retCode"></param>
    /// <param name="dto"></param>
    private void OnToClientAdd(OperationResponse response, PlayerDto dto)
    { 
        switch (response.ReturnCode)
        {
            case 0:
                view.ShowToClientAddPanel(dto);
                break;
            case 1:
                 view.UpdateView(dto);
                break;
            case -1:
                MessageTip.Instance.Show(response.DebugMessage);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 上线
    /// </summary>
    private void OnLine(PlayerDto dto)
    {
        //保存数据
        GameData.Player = dto;
        view.UpdateView(dto);
    }

    /// <summary>
    /// 获取信息
    /// </summary>
    /// <param name="retCode"></param>
    private void OnGetInfo(int retCode)
    {
        switch (retCode)
        {
            case -1:
                //非法登录
                break;
            case -2:
                //没有角色创建角色
                view.CreatePanelActive = true;
                break;
            case 0:
                //有角色
                playerOnLine();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 创建角色成功
    /// </summary>
    private void OnCreate(int retCode,string mess)
    {
        if (retCode == 0)
        {
            view.CreatePanelActive = false;
            //创建成功后发起上线
            playerOnLine();
        }
        else if (retCode == -1)
        {
            MessageTip.Instance.Show(mess);
            view.CreatePanelActive = true;
        }

    }
    /// <summary>
    /// 角色上线
    /// </summary>
    private void playerOnLine()
    {
        PhotonManager.Instance.Request(OpCode.PlayerCode,OpPlayer.OnLine);
    }
}
