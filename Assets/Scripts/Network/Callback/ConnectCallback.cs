using UnityEngine;

public class ConnectCallback
{
    public void ConnectSucc(string err)
    {
        Debug.Log("[客户端] 连接服务器成功");
    }

    public void ConnectFail(string err)
    {
        Debug.Log("[客户端] 连接服务器失败, " + err);
    }

    public void ConnectClose(string err)
    {
        Debug.Log("[客户端] 服务器关闭");
    }
}
