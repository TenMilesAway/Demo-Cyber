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

        private PlayerInfo _playerInfo; // 需要等待玩家信息加载完成后才显示主面板，所以通过 param 传递

        public override string GetPanelName()
        {
            return GlobalDefine.MainPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _isBlockingWindow = false;

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

        private void AddListeners()
        {
            // 业务
            GameManager.Event.AddListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            // UI
            _btnBag.onClick.AddListener(OnClickBtnBag);
            _btnForge.onClick.AddListener(OnClickBtnForge);
        }

        private void RemoveListeners()
        {
            // 业务
            GameManager.Event.RemoveListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            // UI
            _btnBag.onClick.RemoveAllListeners();
            _btnForge.onClick.RemoveAllListeners();
        }

        #region 主要方法
        /// <summary>
        /// 初始化主要面板玩家信息
        /// </summary>
        private void InitPlayerInfo(PlayerInfo info)
        {
            UpdateUI(info);
        }
        #endregion

        #region 监听方法：UI
        private void OnClickBtnBag()
        {
            AudioClipData data = new AudioClipData
            {
                _type = AudioClipType.SFXOpenPanel,
                _content = "打开仓库面板",
            };
            GameManager.Event.Broadcast<AudioClipData>(GameEventType.PlayAudio, data);

            InventoryParam param = new InventoryParam();
            param.data = _playerInfo;
            param.isWithPropertyPanel = true;

            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param);
            UIManager.GetInstance().OpenPanel(GlobalDefine.PropertyPanel);

            UIManager.GetInstance().ClosePanel(GlobalDefine.InteractivePanel);
        }

        private void OnClickBtnForge()
        {
            List<ItemInfo> infos = new List<ItemInfo>
            {
                new ItemInfo { _id = 1000, _num = 1 },
                new ItemInfo { _id = 1001, _num = 2 },
                new ItemInfo { _id = 1002, _num = 3 },
            };

            InventoryDataManager.GetInstance().AddItemInfoToInventory(infos);
        }
        #endregion

        #region 监听方法：刷新 UI
        /// <summary>
        /// 刷新面板 UI
        /// </summary>
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
