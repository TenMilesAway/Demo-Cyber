using Cyber;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class GoldRushPanelParam : OpenUIParam
    {
        public string title = "淘金时刻";
        public string subtitle = "翻出属于你的幸运奖励！";
        public string remainTime = "6天23小时";
        public string tips = "消耗货币进行翻牌，九选一获得奖励";
        public int ownedCurrency = 1250;
        public int flipCost = 100;
    }

    public class GoldRushPanel : UIBasePanel
    {
        [Header("Base")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnAddCurrency;
        [SerializeField] private Button _btnRewardPreview;
        [SerializeField] private Button _btnHistory;
        [SerializeField] private Button _btnFlip;

        [Header("Texts")]
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtSubtitle;
        [SerializeField] private Text _txtRemainTime;
        [SerializeField] private Text _txtTips;
        [SerializeField] private Text _txtRules;
        [SerializeField] private Text _txtTopCurrency;
        [SerializeField] private Text _txtBottomCurrency;
        [SerializeField] private Text _txtFlipCost;
        [SerializeField] private Text _txtSelectionHint;

        [Header("Cards")]
        [SerializeField] private Button[] _cardButtons;
        [SerializeField] private Image[] _cardImages;
        [SerializeField] private Text[] _cardTexts;

        private readonly List<string> _rewardPlaceholders = new List<string>
        {
            "金币箱",
            "稀有矿石",
            "强化石",
            "幸运钥匙",
            "祝福卷轴",
            "高级药剂",
            "藏宝图",
            "秘银碎片",
            "活动货币",
        };

        private GoldRushPanelParam _panelParam;
        private bool[] _openedCards;
        private int _selectedCardIndex = -1;
        private int _ownedCurrency;

        private static readonly Color CardNormalColor = new Color32(147, 96, 50, 255);
        private static readonly Color CardSelectedColor = new Color32(214, 159, 77, 255);
        private static readonly Color CardOpenedColor = new Color32(232, 203, 123, 255);

        public override string GetPanelName()
        {
            return GlobalDefine.GoldRushPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _panelParam = param as GoldRushPanelParam ?? new GoldRushPanelParam();
            _ownedCurrency = _panelParam.ownedCurrency;
            _openedCards = new bool[_cardButtons.Length];
            _selectedCardIndex = -1;

            InitStaticTexts();
            InitCards();
            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        private void AddListeners()
        {
            _btnClose.onClick.AddListener(OnClickClose);
            _btnAddCurrency.onClick.AddListener(() => ShowToast("活动货币入口预留中"));
            _btnRewardPreview.onClick.AddListener(() => ShowToast("奖励预览功能预留中"));
            _btnHistory.onClick.AddListener(() => ShowToast("历史记录功能预留中"));
            _btnFlip.onClick.AddListener(OnClickFlip);

            for (int i = 0; i < _cardButtons.Length; i++)
            {
                int index = i;
                _cardButtons[i].onClick.AddListener(() => OnClickCard(index));
            }
        }

        private void RemoveListeners()
        {
            _btnClose.onClick.RemoveAllListeners();
            _btnAddCurrency.onClick.RemoveAllListeners();
            _btnRewardPreview.onClick.RemoveAllListeners();
            _btnHistory.onClick.RemoveAllListeners();
            _btnFlip.onClick.RemoveAllListeners();

            foreach (Button button in _cardButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void InitStaticTexts()
        {
            _txtTitle.text = _panelParam.title;
            _txtSubtitle.text = _panelParam.subtitle;
            _txtRemainTime.text = $"活动剩余时间：{_panelParam.remainTime}";
            _txtTips.text = _panelParam.tips;
            _txtRules.text =
                "1. 消耗指定货币进行翻牌，九选一获得奖励；\n\n" +
                "2. 每次翻牌可获得一个奖励，奖励不会重复获得；\n\n" +
                "3. 全部奖励抽完后，界面将自动刷新，可继续参与活动；\n\n" +
                "4. 活动结束后，未使用的货币将自动回收，请及时使用。";
            _txtFlipCost.text = _panelParam.flipCost.ToString();
            RefreshCurrencyTexts();
            UpdateSelectionHint();
        }

        private void InitCards()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                _cardTexts[i].text = "待翻牌";
                _cardImages[i].color = CardNormalColor;
            }
        }

        private void OnClickClose()
        {
            UIManager.GetInstance().ClosePanel(GetPanelName());
        }

        private void OnClickCard(int index)
        {
            if (_openedCards[index])
            {
                ShowToast("该奖励已经翻开");
                return;
            }

            _selectedCardIndex = index;
            RefreshCardStates();
            UpdateSelectionHint();
        }

        private void OnClickFlip()
        {
            if (_selectedCardIndex < 0)
            {
                ShowToast("请先选择一张奖励牌");
                return;
            }

            if (_ownedCurrency < _panelParam.flipCost)
            {
                ShowToast("活动货币不足");
                return;
            }

            _ownedCurrency -= _panelParam.flipCost;
            _openedCards[_selectedCardIndex] = true;
            _cardTexts[_selectedCardIndex].text = _rewardPlaceholders[_selectedCardIndex % _rewardPlaceholders.Count];
            _selectedCardIndex = -1;

            RefreshCurrencyTexts();
            RefreshCardStates();
            UpdateSelectionHint();
        }

        private void RefreshCardStates()
        {
            for (int i = 0; i < _cardButtons.Length; i++)
            {
                if (_openedCards[i])
                {
                    _cardImages[i].color = CardOpenedColor;
                }
                else if (i == _selectedCardIndex)
                {
                    _cardImages[i].color = CardSelectedColor;
                }
                else
                {
                    _cardImages[i].color = CardNormalColor;
                }
            }
        }

        private void RefreshCurrencyTexts()
        {
            string currency = _ownedCurrency.ToString();
            _txtTopCurrency.text = currency;
            _txtBottomCurrency.text = currency;
        }

        private void UpdateSelectionHint()
        {
            if (_selectedCardIndex < 0)
            {
                _txtSelectionHint.text = "请选择一张奖励牌";
                return;
            }

            _txtSelectionHint.text = $"当前选择：第 {_selectedCardIndex + 1} 张奖励牌";
        }

        private void ShowToast(string content)
        {
            UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstanceID().ToString(), toast =>
            {
                ToastPanel component = toast.GetComponent<ToastPanel>();
                component.Init(content, true);
            });
        }
    }
}
