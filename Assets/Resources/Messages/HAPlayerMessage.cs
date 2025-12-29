using HA;

/// <summary>
/// 协议：向服务器请求获取玩家信息
/// </summary>
public class HAMsgPlayerInfoLoad : MsgBase
{ 
    public HAMsgPlayerInfoLoad() { protoName = "HAMsgPlayerInfoLoad"; }

    public PlayerInfo playerInfo;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

/// <summary>
/// 协议：向服务器上传玩家信息以保存
/// </summary>
public class HAMsgPlayerInfoUpload : MsgBase
{
    public HAMsgPlayerInfoUpload() { protoName = "HAMsgPlayerInfoUpload"; }

    public PlayerInfo playerInfo;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerBaseSave : MsgBase
{
    public MsgPlayerBaseSave() { protoName = "MsgPlayerBaseSave"; }

    public PlayerBaseEntity playerBaseEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerBaseLoad : MsgBase
{
    public MsgPlayerBaseLoad() { protoName = "MsgPlayerBaseLoad"; }

    public PlayerBaseEntity playerBaseEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerStatsSave : MsgBase
{
    public MsgPlayerStatsSave() { protoName = "MsgPlayerStatsSave"; }

    public PlayerStatsEntity playerStatsEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerStatsLoad : MsgBase
{
    public MsgPlayerStatsLoad() { protoName = "MsgPlayerStatsLoad"; }

    public PlayerStatsEntity playerStatsEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerInventorySave : MsgBase
{
    public MsgPlayerInventorySave() { protoName = "MsgPlayerInventorySave"; }

    public PlayerInventoryEntity playerInventoryEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerInventoryLoad : MsgBase
{
    public MsgPlayerInventoryLoad() { protoName = "MsgPlayerInventoryLoad"; }

    public PlayerInventoryEntity playerInventoryEntity;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}