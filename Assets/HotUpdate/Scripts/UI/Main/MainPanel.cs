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
        [Header("右下")]
        [SerializeField] private Button _btnBag;
        [SerializeField] private Button _btnStore;
        [SerializeField] private Button _btnGoldRush;
        [SerializeField] private Button _btnForge;
        [SerializeField] private Button _btnSkill;
        [SerializeField] private Button _btnSettings;

        [Header("右上")]
        [SerializeField] private Button _btnTransport;
        [SerializeField] private Button _btnChallenge;

        [Header("左上")]
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
            GameManager.Event.AddListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            // UI
            _btnBag.onClick.AddListener(OnClickBtnBag);
            _btnForge.onClick.AddListener(OnClickBtnForge);
            _btnTransport.onClick.AddListener(OnClickBtnTransport);
            _btnStore.onClick.AddListener(OnClickBtnShop);
            _btnGoldRush.onClick.AddListener(OnClickBtnGoldRush);
        }

        private void RemoveListeners()
        {
            GameManager.Event.RemoveListener<PlayerInfo>(GameEventType.UpdateMainPanelUI, UpdateUI);

            // UI
            _btnBag.onClick.RemoveAllListeners();
            _btnForge.onClick.RemoveAllListeners();
            _btnTransport.onClick.RemoveAllListeners();
            _btnStore.onClick.RemoveAllListeners();
            _btnGoldRush.onClick.RemoveAllListeners();
        }

        #region 初始化玩家信息
        /// <summary>
        /// 初始化玩家信息
        /// </summary>
        private void InitPlayerInfo(PlayerInfo info)
        {
            UpdateUI(info);
        }
        #endregion

        #region UI 监听 
        /// <summary>
        /// 点击背包按钮
        /// </summary>
        private void OnClickBtnBag()
        {
            AudioClipData data = new AudioClipData
            {
                _type = AudioClipType.SFXOpenPanel,
                _content = "打开背包",
            };
            GameManager.Event.Broadcast<AudioClipData>(GameEventType.PlayAudio, data);

            InventoryParam param = new InventoryParam();
            param.data = _playerInfo;
            param.isWithPropertyPanel = true;

            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param);
            UIManager.GetInstance().OpenPanel(GlobalDefine.PropertyPanel);

            UIManager.GetInstance().ClosePanel(GlobalDefine.InteractivePanel);
        }

        /// <summary>
        /// 点击锻造按钮 (暂时添加测试物品到背包)
        /// </summary>
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

        private void OnClickBtnTransport()
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.MapPanel);
        }

        private void OnClickBtnShop()
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.StorePanel);
        }

        private void OnClickBtnGoldRush()
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.GoldRushPanel);
        }
        #endregion

        #region 更新玩家信息 UI
        /// <summary>
        /// 更新玩家信息 UI
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
