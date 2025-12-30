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
    ReqPlayerInfoLoad,          // 请求：获得玩家所有信息
    ReqPlayerInfoSave,          // 请求：保存玩家所有信息
    ReqPlayerBaseLoad,          // 请求：获得玩家基础信息
    ReqPlayerBaseSave,          // 请求：保存玩家基础信息
    ReqPlayerStatsLoad,         // 请求：获得玩家状态信息
    ReqPlayerStatsSave,         // 请求：保存玩家状态信息
    ReqPlayerInventoryLoad,     // 请求：获得玩家背包信息
    ReqPlayerInventorySave,     // 请求：保存玩家背包信息
    HAMsgPlayerInfoLoad,        // 响应：获得玩家所有信息
    HAMsgPlayerInfoSave,        // 响应：保存玩家所有信息
    MsgPlayerBaseLoad,          // 响应：获得玩家基础信息
    MsgPlayerBaseSave,          // 响应：保存玩家基础信息
    MsgPlayerStatsLoad,         // 响应：获得玩家状态信息
    MsgPlayerStatsSave,         // 响应：保存玩家状态信息
    MsgPlayerInventoryLoad,     // 响应：获得玩家背包信息
    MsgPlayerInventorySave,     // 响应：保存玩家背包信息
    
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
    ClickItemCell,              // 鼠标点击物品格子
    BeginDragItemCell,          // 鼠标开始拖动物品格子
    DragingItemCell,            // 鼠标正在拖动物品格子
    EndDragItemCell,            // 鼠标结束拖动物品格子
    UpdateSelectedItemDetail,   // 选择物品后更新详情面板
    HasInteractiveObject,       // 有可交互物体
    NoneInteractiveObject,      // 无可交互物体
    UpdateInteractiveList,      // 更新交互物体队列
    UpdateInventoryItemList,    // 更新仓库显示物品序列
    UpdateEntityInfoAfterSpawn, // 刷怪操作完成后更新信息
    UpdateMainPanelUI,          // 刷新主面板 UI
    UpdateInventoryPanelUI,     // 刷新背包面板 UI
    GetEXPFromEnemy,            // 怪物死亡后，玩家获得经验
    PlayerReaction,             // 玩家受伤
    ToggleCursor,               // 切换鼠标状态
}
