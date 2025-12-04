using HA;

/// <summary>
/// 协议：向服务器请求获取玩家信息
/// </summary>
public class HAMsgPlayerInfoLoad : MsgBase
{ 
    public HAMsgPlayerInfoLoad() { protoName = "HAMsgPlayerInfoLoad"; }

    public PlayerInfo playerInfo;

    /// <summary>
    /// 回复: 0成功 1失败
    /// </summary>
    public int result = 0;
}

/// <summary>
/// 协议：向服务器上传玩家信息以保存
/// </summary>
public class HAMsgPlayerInfoUpload : MsgBase
{
    public HAMsgPlayerInfoUpload() { protoName = "HAMsgPlayerInfoUpload"; }

    public PlayerInfo playerInfo;

    /// <summary>
    /// 回复: 0成功 1失败
    /// </summary>
    public int result = 0;
}

