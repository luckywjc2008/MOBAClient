using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using MobaCommon.Code;
using UnityEngine;

/// <summary>
/// Photon管理
/// </summary>
public class PhotonManager : Singleton<PhotonManager>, IPhotonPeerListener
{
    #region Receiver
    //账号
    private AccountReceiver account;
    private PlayerReceiver player;
    private SelectReceiver select;
    private FightReceiver fight;

    public AccountReceiver Account
    {
        get
        {
            if (account == null)
            {
                account = FindObjectOfType<AccountReceiver>();
            }
            return account;
        }
    }

    public PlayerReceiver Player
    {
        get
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerReceiver>();
            }
            return player;
        }
    }
    /// <summary>
    /// 选人
    /// </summary>
    public SelectReceiver Select
    {
        get
        {
            if (select == null)
            {
                select = FindObjectOfType<SelectReceiver>();
            }
            return select;
        }
    }

    /// <summary>
    /// 战斗
    /// </summary>
    public FightReceiver Fight
    {
        get
        {
            if (fight == null)
            {
                fight = FindObjectOfType<FightReceiver>();
            }
            return fight;
        }
    }

    #endregion

    #region Photon接口
    /// <summary>
    /// 代表客户端
    /// </summary>
    private PhotonPeer peer;
    /// <summary>
    /// IP地址
    /// </summary>
    private string serverAddress = "127.0.0.1:5055";
    /// <summary>
    /// 名字
    /// </summary>
    private string applicationName = "MOBA";
    /// <summary>
    /// 协议
    /// </summary>
    private ConnectionProtocol protocol = ConnectionProtocol.Udp;
    /// <summary>
    /// 连接状态
    /// </summary>
    private bool isConnect = false;

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnEvent(EventData eventData)
    {

    }
    /// <summary>
    /// 接受服务器的响应
    /// </summary>
    /// <param name="operationResponse"></param>
    public void OnOperationResponse(OperationResponse response)
    {
        Log.Debug(response.ToStringFull());
        byte opCode = response.OperationCode;
        byte subCode = (byte)response[80];
        //转接
        switch (opCode)
        {
            case OpCode.AccountCode:
                Account.OnReceive(subCode,response);
                break;
            case OpCode.PlayerCode:
                Player.OnReceive(subCode,response);
                break;
            case OpCode.SelectCode:
                Select.OnReceive(subCode, response);
                break;
            case OpCode.FightCode:
                Fight.OnReceive(subCode, response);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 状态改变
    /// </summary>
    /// <param name="statusCode"></param>
    public void OnStatusChanged(StatusCode statusCode)
    {
        Log.Debug(statusCode.ToString());
        switch (statusCode)
        {
            case StatusCode.Connect:
                isConnect = true;
                break;
            case StatusCode.Disconnect:
                isConnect = false;
                break;
            default:
                break;
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        peer = new PhotonPeer(this, protocol);
        peer.Connect(serverAddress, applicationName);
        //设置自身物体是跨场景存在
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnect)
        {
            peer.Connect(serverAddress, applicationName);
        }
        peer.Service();
    }

    void OnApplicationQuit()
    {
        //断开连接
        peer.Disconnect();
    }
    /// <summary>
    /// 向服务器发请求
    /// </summary>
    /// <param name="OpCode">操作码</param>
    /// <param name="SubCode">子操作码</param>
    /// <param name="parameters">传递参数</param>
    public void Request(byte OpCode,byte SubCode,params object[] parameters)
    {
        //创建参数字典
        Dictionary<byte,object> dict = new Dictionary<byte, object>();
        //指定子操作码
        dict[80] = SubCode;
        for (int i = 0; i < parameters.Length; i++)
        {
            dict[(byte)i] = parameters[i];
        }
        peer.OpCustom(OpCode, dict, true);
    }
}
