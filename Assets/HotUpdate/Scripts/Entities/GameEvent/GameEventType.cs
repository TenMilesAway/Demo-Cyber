/// <summary>
/// 该枚举类主要管理事件号
/// </summary>
public enum GameEventType
{
    // 网络事件
    PacketIdBegin = 0,
    ConnectSucc = 1,      // 连接成功
    ConnectFail = 2,      // 连接失败
    Close = 3,            // 连接关闭
    PacketIdEnd = 10000,
    // 网络消息预留区间

    OneSecondEvent,       // 每秒触发事件
    PlayAudio,            // 播放音频
    EnterItemCell,        // 鼠标进入物品格子
    ExitItemCell,         // 鼠标离开物品格子
}
