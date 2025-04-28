using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    /// <summary>
    /// 禁用输入的基础面板
    /// </summary>
    public class DisableBasePanel : BasePanel
    {

        public override void ShowMe()
        {
            base.ShowMe();

            GameDataMgr.GetInstance().GetPlayer().Input.OnDisable();
        }

        public override void HideMe()
        {
            base.HideMe();

            GameDataMgr.GetInstance().GetPlayer().Input.OnEnable();
        }
    }
}
