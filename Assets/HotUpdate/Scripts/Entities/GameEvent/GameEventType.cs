/// <summary>
/// 该枚举类主要管理事件号
/// </summary>
public enum GameEventType
{
    /** 网络事件 **/
    PacketIdBegin = 0,
    ConnectSucc = 1,            // 连接成功
    ConnectFail = 2,            // 连接失败
    Close = 3,                  // 连接关闭
    PacketIdEnd = 10000,
    /** 网络消息预留区间 **/
    
    /** 玩家输入 **/
    DisablePlayerInput,         // 禁用玩家所有输入
    EnablePlayerInput,          // 启用玩家所有输入
    DisablePlayerFlipInput,     // 禁用玩家攻击
    EnablePlayerFlipInput,      // 启用玩家攻击
    DisableInteractiveInput,    // 禁用玩家交互
    EnableInteractiveInput,     // 启用玩家交互

    /** 业务事件 **/
    OneSecondEvent,             // 每秒触发事件
    PlayAudio,                  // 播放音频
    EnterItemCell,              // 鼠标进入物品格子
    ExitItemCell,               // 鼠标离开物品格子
    BeginDragItemCell,          // 鼠标开始拖动物品格子
    DragingItemCell,            // 鼠标正在拖动物品格子
    EndDragItemCell,            // 鼠标结束拖动物品格子
    HasInteractiveObject,       // 有可交互物体
    NoneInteractiveObject,      // 无可交互物体
    UpdateInteractiveList,      // 更新交互物体队列
}
