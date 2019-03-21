using ExitGames.Client.Photon;

/// <summary>
/// 收到服务器响应的接受接口
/// </summary>
public interface IReceiver
{
    /// <summary>
    /// 收到服务器响应的子操作
    /// </summary>
    /// <param name="subCode"></param>
    /// <param name="response"></param>
    void OnReceive(byte subCode, OperationResponse response);
}
