using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class EquipmentTipPanelParam : OpenUIParam
    {
        public EquipCell _equipCell;
    }

    public class EquipmentTipPanel : UIBasePanel
    {
        [SerializeField] private Button _btnTakeOff;
        [SerializeField] private Button _btnClose;

        private EquipCell _equipCell;
        private int i = 0;

        public override string GetPanelName()
        {
            return GlobalDefine.EquipmentTipPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            EquipmentTipPanelParam equipmentTipPanelParam = (EquipmentTipPanelParam)param;

            _equipCell = equipmentTipPanelParam._equipCell;
            transform.position = _equipCell.transform.position;

            RemoveListeners();
            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        private void AddListeners()
        {
            i++;
            HADebug.LogFormat("目前添加{0}次监听", i);
            _btnTakeOff.onClick.AddListener(OnClickBtnTakeOff);
            _btnClose.onClick.AddListener(OnClickBtnClose);
        }

        public void RemoveListeners()
        {
            i--;
            HADebug.LogFormat("目前添加{0}次监听", i);
            _btnTakeOff.onClick.RemoveAllListeners();
            _btnClose.onClick.RemoveAllListeners();
        }

        #region 监听方法
        private void OnClickBtnTakeOff()
        {
            _equipCell.TakeOffEquipment();
            UIManager.GetInstance().ClosePanel(GetPanelName());
        }

        private void OnClickBtnClose()
        {
            UIManager.GetInstance().ClosePanel(GetPanelName());
        }
        #endregion
    }
}
