using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class MainPanelParam : OpenUIParam
    {

    }

    public class MainPanel : UIBasePanel
    {
        [Header("右下: 功能按钮组")]
        [SerializeField]
        private Button _btnBag;
        [SerializeField]
        private Button _btnForge;
        [SerializeField]
        private Button _btnSkill;
        [SerializeField]
        private Button _btnSettings;

        [Header("右上: 地图切换按钮组")]
        [SerializeField]
        private Button _btnTransport;
        [SerializeField]
        private Button _btnChallenge;

        [Header("需要更新的信息")]
        [SerializeField]
        private Text _txtName;
        [SerializeField]
        private Text _txtCommonCurrency;
        [SerializeField]
        private Text _txtRareCurrency;
        [SerializeField]
        private Text _txtCurrentHP;
        [SerializeField]
        private Text _txtMaxHP;
        [SerializeField]
        private Text _txtCurrentMP;
        [SerializeField]
        private Text _txtMaxMP;
        [SerializeField]
        private Text _txtCurrentEXP;
        [SerializeField]
        private Text _txtMaxEXP;

        public override string GetPanelName()
        {
            return GlobalDefine.MainPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            MainPanelParam mainPanelParam = (MainPanelParam)param;
            PlayerInfo info = mainPanelParam.data as PlayerInfo;
            InitPlayerInfo(info);

            _btnBag.onClick.AddListener(OnClickBtnBag);
        }

        #region 主要方法
        private void InitPlayerInfo(PlayerInfo info)
        {
            if (info == default) return;

            _txtName.text           = info._name;
            _txtCommonCurrency.text = info._commonCurrency.ToString();
            _txtRareCurrency.text   = info._rareCurrency.ToString();
            _txtCurrentHP.text      = info._currentHP.ToString();
            _txtMaxHP.text          = info._maxHP.ToString();
            _txtCurrentMP.text      = info._currentMP.ToString();
            _txtMaxMP.text          = info._maxMP.ToString();
            _txtCurrentEXP.text     = info._currentEXP.ToString();
            _txtMaxEXP.text         = info._maxEXP.ToString();
        }
        #endregion

        #region 监听方法
        private void OnClickBtnBag()
        {
            // 测试切换状态机
            // GameManager.Fsm.StartFsmState(GlobalDefine.FsmStateForestMap);

            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel);
        }
        #endregion
    }
}
