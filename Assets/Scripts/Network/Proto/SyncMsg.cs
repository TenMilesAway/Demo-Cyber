using System.Collections.Generic;
using Cyber;

/// <summary>
/// 获得当前玩家列表
/// </summary>
public class MsgGetPlayerList : MsgBase
{
    public MsgGetPlayerList() { protoName = "MsgGetPlayerList"; }

    public List<PlayerInfo> list;
}

/// <summary>
/// 更新当前地图玩家实体
/// </summary>
public class MsgUpdatePlayerEntities : MsgBase
{
    public MsgUpdatePlayerEntities() { protoName = "MsgUpdatePlayerEntities"; }

    public List<PlayerInfo> list;
}

/// <summary>
/// 上传当前玩家临时信息
/// </summary>
public class MsgUploadPlayerTempInfo : MsgBase
{
    public MsgUploadPlayerTempInfo() { protoName = "MsgUploadPlayerTempInfo"; }

    public PlayerTempInfo tempInfo;

    public Maps mapInfo;
}

/// <summary>
/// 更新当前地图同步玩家的临时信息
/// </summary>
public class MsgUpdatePlayerTempInfo : MsgBase
{
    public MsgUpdatePlayerTempInfo() { protoName = "MsgUpdatePlayerTempInfo"; }

    public List<PlayerTempInfo> tempInfos;
}

/// <summary>
/// 获得离线玩家 id
/// </summary>
public class MsgPlayerDisconnect : MsgBase
{
    public MsgPlayerDisconnect() { protoName = "MsgPlayerDisconnect"; }

    public string id;
}