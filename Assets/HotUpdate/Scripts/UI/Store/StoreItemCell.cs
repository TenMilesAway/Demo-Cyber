using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class StoreItemCell : MonoBehaviour
    {
        [SerializeField] private Image _imgIcon;
        [SerializeField] private Image _imgCurrency;
        [SerializeField] private Text _txtName;
        [SerializeField] private Text _txtNum;
        [SerializeField] private Text _txtPrice;

        [Space(10)]
        [SerializeField] private Button _btnSub;
        [SerializeField] private Button _btnAdd;
        [SerializeField] private Button _btnBuy;

        private int _itemID = 0;      // 物品 ID
        private int _currentNum = 1;  // 当前购买数量
        private int _unitPrice = 0;   // 单价
        private int _totalPrice = 0;  // 总价
        private int _currencyType;    // 0 普通货币，1 稀有货币

        public void Init(TBStoreData storeData)
        {
            _btnBuy.enabled = false;

            TBItemData data = ItemDataManager.GetInstance().GetData(storeData.id);
            _itemID = storeData.id;
            _unitPrice = storeData.unitPrice;
            _currencyType = storeData.currencyType;

            GameManager.Resource.LoadResourceAsync<Sprite>(data.icon, "Store", (obj, result) =>
            {
                _imgIcon.sprite = obj as Sprite;
            });

            string currencyPath = _currencyType == 0 ? "Assets/UI/Items/Items.spriteatlas[SpriteItem0]" : "Assets/UI/Items/Items.spriteatlas[SpriteItem1]";

            GameManager.Resource.LoadResourceAsync<Sprite>(currencyPath, "Store", (obj, result) =>
            {
                _imgCurrency.sprite = obj as Sprite;
            });

            UpdateUI();
            RemoveListeners();
            AddListeners();
        }

        private void AddListeners()
        {
            _btnSub.onClick.AddListener(OnClickBtnSub);
            _btnAdd.onClick.AddListener(OnClickBtnAdd);
            _btnBuy.onClick.AddListener(OnClickBtnBuy);
        }

        public void RemoveListeners()
        {
            _btnSub.onClick.RemoveAllListeners();
            _btnAdd.onClick.RemoveAllListeners();
            _btnBuy.onClick.RemoveAllListeners();
        }

        #region 辅助方法
        /// <summary>
        /// 计算总价格
        /// </summary>
        private void CalculateTotalPrice()
        {
            _totalPrice = _unitPrice * _currentNum;
        }
        #endregion

        #region 监听方法：UI
        /// <summary>
        /// 刷新 UI
        /// </summary>
        private void UpdateUI()
        {
            CalculateTotalPrice();
            _txtNum.text = _currentNum.ToString();
            _txtPrice.text = _totalPrice.ToString();
            _btnBuy.enabled = true;
        }

        /// <summary>
        /// 点击按钮：增加数量
        /// </summary>
        private void OnClickBtnAdd()
        {
            _btnBuy.enabled = false;

            _currentNum++;
            _currentNum = Mathf.Clamp(_currentNum, 1, 999);

            UpdateUI();
        }

        /// <summary>
        /// 点击按钮：减少数量
        /// </summary>
        private void OnClickBtnSub()
        {
            _btnBuy.enabled = false;

            _currentNum--;
            _currentNum = Mathf.Clamp(_currentNum, 1, 999);

            UpdateUI();
        }

        /// <summary>
        /// 点击购买按钮
        /// </summary>
        private void OnClickBtnBuy()
        {
            _btnBuy.enabled = false;

            // 检查货币是否足够
            if (!PlayerDataManager.GetInstance().CheckCurrencyIsEnough(_totalPrice, _currencyType))
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, "Toast", (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component.Init(string.Format("货币不足"), true);
                });
                return;
            }

            // 发送购买请求
            ReqStoreBuyItem();
        }
        #endregion

        #region 监听方法：请求
        /// <summary>
        /// 购买物品
        /// </summary>
        private void ReqStoreBuyItem()
        {
            HAMsgStoreBuyItem msg = new HAMsgStoreBuyItem();

            msg.playerID = GameManager.GlobalData.PlayerID;
            msg.price = _totalPrice;
            msg.currencyType = _currencyType;
            msg.itemID = _itemID;
            msg.itemNum = _currentNum;

            NetManager.Send(msg);
        }
        #endregion
    }
}
