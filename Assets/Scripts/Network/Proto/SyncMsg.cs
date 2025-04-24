using System.Collections.Generic;
using Cyber;

public class MsgGetPlayerList : MsgBase
{
    public MsgGetPlayerList() { protoName = "MsgGetPlayerList"; }

    public List<PlayerInfo> list;
}

public class MsgUpdatePlayerEntities : MsgBase
{
    public MsgUpdatePlayerEntities() { protoName = "MsgUpdatePlayerEntities"; }

    public List<PlayerInfo> list;
}

public class MsgUploadPlayerTempInfo : MsgBase
{
    public MsgUploadPlayerTempInfo() { protoName = "MsgUploadPlayerTempInfo"; }

    public PlayerTempInfo tempInfo;

    public Maps mapInfo;
}

public class MsgUpdatePlayerTempInfo : MsgBase
{
    public MsgUpdatePlayerTempInfo() { protoName = "MsgUpdatePlayerTempInfo"; }

    public List<PlayerTempInfo> tempInfos;
}