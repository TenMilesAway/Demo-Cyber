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
        [SerializeField] private Button _btnBag;
        [SerializeField] private Button _btnForge;
        [SerializeField] private Button _btnSkill;
        [SerializeField] private Button _btnSettings;

        [Header("右上: 地图切换按钮组")]
        [SerializeField] private Button _btnTransport;
        [SerializeField] private Button _btnChallenge;

        [Header("需要更新的信息")]
        [SerializeField] private Text _txtName;
        [SerializeField] private Text _txtCommonCurrency;
        [SerializeField] private Text _txtRareCurrency;
        [SerializeField] private Text _txtCurrentHP;
        [SerializeField] private Text _txtMaxHP;
        [SerializeField] private Text _txtCurrentMP;
        [SerializeField] private Text _txtMaxMP;
        [SerializeField] private Text _txtCurrentEXP;
        [SerializeField] private Text _txtMaxEXP;
        [SerializeField] private Image _imgHPBar;
        [SerializeField] private Image _imgMPBar;
        [SerializeField] private Image _imgEXPBar;

        private PlayerInfo _playerInfo;

        public override string GetPanelName()
        {
            return GlobalDefine.MainPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            MainPanelParam mainPanelParam = (MainPanelParam)param;
            _playerInfo = mainPanelParam.data as PlayerInfo;
            InitPlayerInfo(_playerInfo);

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        #region 主要方法
        private void InitPlayerInfo(PlayerInfo info)
        {
            UpdateUI(info);
        }
        #endregion

        #region 监听方法
        private void AddListeners()
        {
            GameManager.Event.AddListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            _btnBag.onClick.AddListener(OnClickBtnBag);
            _btnForge.onClick.AddListener(OnClickBtnForge);
        }

        private void RemoveListeners()
        {
            GameManager.Event.RemoveListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            _btnBag.onClick.RemoveAllListeners();
            _btnForge.onClick.RemoveAllListeners();
        }

        private void OnClickBtnBag()
        {
            InventoryParam param = new InventoryParam();
            param.data = _playerInfo;

            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param);
        }

        private void OnClickBtnForge()
        {
            
        }

        private void UpdateUI(PlayerInfo info)
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
            _imgHPBar.fillAmount    = (float)info._currentHP / info._maxHP;
            _imgMPBar.fillAmount    = (float)info._currentMP / info._maxMP;
            _imgEXPBar.fillAmount   = (float)info._currentEXP / info._maxEXP;
        }
        #endregion
    }
}
