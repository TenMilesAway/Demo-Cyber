using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class SingleConvertLine : MonoBehaviour
    {
        [SerializeField] private Transform _leftNeedItemListContainer;
        [SerializeField] private Transform _rightGiveItemContainer;
        [SerializeField] private Button _btnConvert;

        private List<ItemInfo> _needItemInfos; // 兑换所需物品
        private List<ItemInfo> _convertItems;  // 兑换得到的物体

        public void Init(TBConvertData convertDatas)
        {
            _needItemInfos = new List<ItemInfo>();
            string[] needItemList = convertDatas.needItemList.Split("|");
            foreach (string item in needItemList)
            {
                string[] param = item.Split(",");
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ConvertCell, GetInstanceID().ToString(), (convertCell) =>
                {
                    convertCell.transform.SetParent(_leftNeedItemListContainer, false);
                    ItemInfo info = new ItemInfo
                    {
                        _id = int.Parse(param[0]),
                        _num = int.Parse(param[1])
                    };
                    convertCell.GetComponent<ConvertCell>().Init(info);
                    _needItemInfos.Add(info);
                });
            }

            // 目前均只有 1 个物品
            _convertItems = new List<ItemInfo>();
            string[] convertItem = convertDatas.convertItem.Split("|");
            foreach (string item in convertItem)
            {
                string[] param = item.Split(",");
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ConvertCell, GetInstanceID().ToString(), (convertCell) =>
                {
                    convertCell.transform.SetParent(_rightGiveItemContainer, false);
                    ItemInfo info = new ItemInfo
                    {
                        _id = int.Parse(param[0]),
                        _num = int.Parse(param[1])
                    };
                    convertCell.GetComponent<ConvertCell>().Init(info);
                    _convertItems.Add(info);
                });
            }

            // 按钮监听
            _btnConvert.onClick.AddListener(OnClickBtnConvert);
        }

        /// <summary>
        /// 放回对象池需要做的事情
        /// </summary>
        public void PutBackToPool()
        {
            _btnConvert.onClick.RemoveAllListeners();

            for (int i = _leftNeedItemListContainer.childCount - 1; i >= 0; i--)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ConvertCell, _leftNeedItemListContainer.GetChild(i).gameObject);
            }

            for (int i = _rightGiveItemContainer.childCount - 1; i >= 0; i--)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ConvertCell, _rightGiveItemContainer.GetChild(i).gameObject);
            }
        }

        #region 监听方法：UI
        /// <summary>
        /// 点击兑换
        /// </summary>
        private void OnClickBtnConvert()
        {
            // 查看仓库物品是否足够

            // 获得物品
            InventoryDataManager.GetInstance().AddItemInfoToInventory(_convertItems);

            UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstanceID().ToString(), (toast) =>
            {
                ToastPanel component = toast.GetComponent<ToastPanel>();
                component?.Init(string.Format("兑换成功啦!请猎兽者大人前往仓库查看"), true);
            });
        }
        #endregion
    }
}
