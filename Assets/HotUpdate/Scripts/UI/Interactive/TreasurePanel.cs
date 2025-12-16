using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class TreasurePanelParam : OpenUIParam
    {
        public bool isInteractable;
        public List<HATreasureEntity> treasureEntities;
    }

    public class TreasurePanel : UIBasePanel
    {
        public override string GetPanelName()
        {
            return GlobalDefine.TreasurePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);
        }
    }
}
