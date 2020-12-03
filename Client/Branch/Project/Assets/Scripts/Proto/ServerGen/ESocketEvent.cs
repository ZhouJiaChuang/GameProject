/// <summary>
/// 网络事件
/// </summary>
public enum ESocketEvent
{
    ConnectSuccess = 101,
    ConnectFailed = 102,

    /// <summary>
    /// 收到更新视野内玩家消息
    /// </summary>
    ResUpdateViewPlayerMessage,
    
}