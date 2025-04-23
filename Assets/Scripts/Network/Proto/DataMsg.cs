using Cyber;

public class MsgPlayerDataSave : MsgBase
{
    public MsgPlayerDataSave() { protoName = "MsgPlayerDataSave"; }

    public PlayerInfo playerInfo;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}

public class MsgPlayerDataLoad : MsgBase
{
    public MsgPlayerDataLoad() { protoName = "MsgPlayerDataLoad"; }

    public PlayerInfo playerInfo;

    // 回复 (0 - 成功，1 - 失败)
    public int result = 0;
}
