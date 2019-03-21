using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using LitJson;
using MobaCommon.Code;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectReceiver : MonoBehaviour, IReceiver
{
    [SerializeField]
    private SelectView view;

    private SelectModel[] team1;
    private SelectModel[] team2;
    private int myTeam;
    public void OnReceive(byte subCode, OperationResponse response)
    {
        switch (subCode)
        {
            case OpSelect.GetInfo:
                //先保存数据再更新显示
                team1 = JsonMapper.ToObject<SelectModel[]>(response[0].ToString());
                team2 = JsonMapper.ToObject<SelectModel[]>(response[1].ToString());
                myTeam = GetTeam(team1, team2);
                OnUpdateView();
                break;
            case OpSelect.Enter:
                int playerId = (int) response[0];
                OnEnter(playerId);
                break;
            case OpSelect.Destroy:
                OnRoomDestroy();
                break;
            case OpSelect.Select:
                int retCode = response.ReturnCode;
                OnSelect(retCode,response);
                break;
            case OpSelect.Ready:
                OnReady(response.ReturnCode, response);
                break;
            case OpSelect.Talk:
                OnTalk(response[0].ToString());
                break;
            case OpSelect.StartFight:
                OnStartFight();
                break;
            default:
                break;
        }
    }

    #region 帮助方法

    private void OnStartFight()
    {
       SceneManager.LoadScene("Fight");
    }

    /// <summary>
    /// 聊天
    /// </summary>
    /// <param name="context"></param>
    private void OnTalk(string context)
    {
        view.TalkAppend(context);
    }

    /// <summary>
    /// 玩家确认选择
    /// </summary>
    /// <param name="playerId"></param>
    private void OnReady(int retCode,OperationResponse response)
    {
        if (retCode == -1)
        {
            MessageTip.Instance.Show(response.DebugMessage);

        }else if (retCode == 0)
        {
            int playerId = (int) response[0];
            foreach (SelectModel item in team1)
            {
                if (item.playerId == playerId)
                {
                    item.isReady = true;
                    OnUpdateView();
                    return;
                }
            }
            foreach (SelectModel item in team2)
            {
                if (item.playerId == playerId)
                {
                    item.isReady = true;
                    OnUpdateView();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 有玩家选择了英雄
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="heroId"></param>
    private void OnSelect(int retCode, OperationResponse response)
    {
        if (retCode == 0)
        {
            int playerId = (int) response[0];
            int heroId = (int) response[1];

            foreach (SelectModel item in team1)
            {
                if (item.playerId == playerId)
                {
                    item.heroId = heroId;
                    OnUpdateView();
                    return;
                }
            }
            foreach (SelectModel item in team2)
            {
                if (item.playerId == playerId)
                {
                    item.heroId = heroId;
                    OnUpdateView();
                    return;
                }
            }

        }
        else if (retCode == -1)
        {
            MessageTip.Instance.Show(response.DebugMessage);
        }
    }

    private void OnRoomDestroy()
    {
        //关闭选人界面
        UIManager.Instance.HideUIPanel(UIDefind.UISelect);
        //打开主界面
        UIManager.Instance.ShowUIPanel(UIDefind.UIMain);

        MessageTip.Instance.Show("有人退出!");
    }

    /// <summary>
    /// 有其他玩家进入
    /// </summary>
    /// <param name="playerId"></param>
    private void OnEnter(int playerId)
    {
        foreach (SelectModel item in team1)
        {
            if (item.playerId == playerId)
            {
                item.isEnter = true;
                OnUpdateView();
                return;
            }
        }
        foreach (SelectModel item in team2)
        {
            if (item.playerId == playerId)
            {
                item.isEnter = true;
                OnUpdateView();
                return;
            }
        }
    }

    /// <summary>
    /// 获取房间数据
    /// </summary>
    private void OnUpdateView()
    {
        //更新显示
        Log.Debug("myTeam = " + myTeam + "team1 count = " + team1.Length + "team2 count =" + team2.Length);
        view.UpdateView(myTeam,team1,team2);
    }

    /// <summary>
    /// 获取队伍
    /// </summary>
    /// <param name="team1"></param>
    /// <param name="team2"></param>
    /// <returns></returns>
    private int GetTeam(SelectModel[] team1, SelectModel[] team2)
    {
        int playerId = GameData.Player.id;
        for (int i = 0; i < team1.Length; i++)
        {
            if (team1[i].playerId == playerId)
            {
                return 1;
            }
        }
        for (int i = 0; i < team2.Length; i++)
        {
            if (team2[i].playerId == playerId)
            {
                return 2;
            }
        }
        return -1;
    }
    #endregion
}
