using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public partial class Service : BaseManager<Service>
    {
        // 这个方法只有上传，上传以后没做回传
        public void UploadPlayerTempInfo()
        {
            MsgUploadPlayerTempInfo msg = new MsgUploadPlayerTempInfo();

            msg.tempInfo = GameDataMgr.GetInstance().GetPlayerTempInfo();
            msg.mapInfo  = GameDataMgr.GetInstance().mapInfo;

            NetManager.Send(msg);
        }
    }
}
