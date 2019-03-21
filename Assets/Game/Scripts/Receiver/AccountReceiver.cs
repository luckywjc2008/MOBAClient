using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MobaCommon.Code;
using UnityEngine;

public class AccountReceiver : MonoBehaviour, IReceiver
{
    [SerializeField]
    private AccountView view;
    public void OnReceive(byte subCode, OperationResponse response)
    {
        switch (subCode)
        {
            case OpAccount.Login:
                OnLogin(response.ReturnCode,response.DebugMessage);
                break;
            case OpAccount.Register:
                OnRegister(response.ReturnCode, response.DebugMessage);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 登录的处理
    /// </summary>
    /// <param name="retCode"></param>
    private void OnLogin(short retCode,string mess)
    {
        switch (retCode)
        {
            case 0:
                //成功
                UIManager.Instance.HideUIPanel(UIDefind.UIAccount);
                UIManager.Instance.ShowUIPanel(UIDefind.UIMain);
                break;
            case -1:
                //失败 玩家在线
                MessageTip.Instance.Show(mess);
                view.EnterInteractable = true;
                view.ResetLoginInput();
                break;
            case -2:
                //账号密码错误
                MessageTip.Instance.Show(mess);
                view.EnterInteractable = true;
                view.ResetLoginInput();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 注册的处理
    /// </summary>
    /// <param name="retCode"></param>
    private void OnRegister(short retCode, string mess)
    {
        switch (retCode)
        {
            case 0:
                //成功
                MessageTip.Instance.Show(mess);
                break;
            case -1:
                //失败 账号重复
                MessageTip.Instance.Show(mess);
                break;
            default:
                break;
        }
    }
}

