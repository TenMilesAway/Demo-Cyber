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
            _btnTakeOff.onClick.AddListener(OnClickBtnTakeOff);
            _btnClose.onClick.AddListener(OnClickBtnClose);
        }

        public void RemoveListeners()
        {
            _btnTakeOff.onClick.RemoveAllListeners();
            _btnClose.onClick.RemoveAllListeners();
        }

        #region ¼àÌý·½·¨
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
