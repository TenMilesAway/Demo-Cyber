using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class TreasurePanelParam : OpenUIParam
    {
        public int _parentInstanceID;                    // 父类唯一 ID
        public bool _isInteractable;                     // 宝藏箱是否已被打开过
        public List<HATreasureEntity> _treasureEntities; // 宝藏箱内所有物品
    }

    public class TreasurePanel : UIBasePanel
    {
        [SerializeField] private Transform _treasureContainer; // 宝藏 Container
        [SerializeField] private Transform _safeboxContainer;  // 安全行囊 Container
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Image _imgTitle;

        private List<HATreasureEntity> _treasureEntities;             // 宝藏箱内所有物品
        private List<GameObject> _itemCells = new List<GameObject>(); // 所有显示的 ItemCell GO
        private bool _isInteractable;                                 // 宝藏箱是否已被打开过
        private int _parentInstanceID;                                // 父类唯一 ID
        private Coroutine _searchCo;                                  // 搜索动画协程变量

        public override string GetPanelName()
        {
            return GlobalDefine.TreasurePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            TreasurePanelParam treasurePanelParam = new TreasurePanelParam();
            treasurePanelParam = param as TreasurePanelParam;
            _isInteractable = treasurePanelParam._isInteractable;
            _treasureEntities = treasurePanelParam._treasureEntities;
            _parentInstanceID = treasurePanelParam._parentInstanceID;

            // 初始化 15 个槽位
            InitItemCells();
            // 初始化安全行囊
            InitSafeBox();

            if (!_isInteractable)
            {
                _searchCo = StartCoroutine(StartSearch());
            }
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            foreach (GameObject cell in _itemCells)
            {
                cell.GetComponent<ItemCell>().RemoveListeners();
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, cell);
            }
            _itemCells.Clear();

            StopAllCoroutines();
            _searchCo = null;
        }

        #region 主要方法：初始化
        private void InitItemCells()
        {
            // 初始化宝藏格子
            for (int i = 0; i < _treasureEntities.Count; i++)
            {
                int index = i;
                ItemInfo item = new ItemInfo
                {
                    _id = _treasureEntities[index]._treasureID,
                    _num = _treasureEntities[index]._treasureNum,
                };

                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemCell, GetInstanceID().ToString(), (GameObject itemCell) =>
                {
                    _itemCells.Add(itemCell);
                    itemCell.transform.SetParent(_treasureContainer, false);
                    if (!_isInteractable)
                    {
                        itemCell.GetComponent<ItemCell>().Init(item, true, _treasureEntities[index], ItemCellParent.Treasure, index, _parentInstanceID);
                    }
                    else
                    {
                        itemCell.GetComponent<ItemCell>().Init(item, true, null, ItemCellParent.Treasure, index, _parentInstanceID);
                    }
                });
            }

            // 初始化非宝藏格子
            int leftCount = 15 - _treasureEntities.Count;

            for (int i = 0; i < leftCount; i++)
            {
                int index = i;
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemCell, GetInstanceID().ToString(), (GameObject itemCell) =>
                {
                    _itemCells.Add(itemCell);
                    itemCell.transform.SetParent(_treasureContainer, false);
                    itemCell.GetComponent<ItemCell>().Init(null, true, null, ItemCellParent.Treasure, index, _parentInstanceID);
                });
            }
        }

        private void InitSafeBox()
        {
            // 后期通过数据单例类获取 safebox 的数量再初始化 ItemCell
        }
        #endregion

        #region 主要方法：宝藏搜索
        private IEnumerator StartSearch()
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < _treasureEntities.Count; i++)
            {
                ItemCell temp = _itemCells[i].GetComponent<ItemCell>();

                // 这里还需要修改,
                if (temp._itemInfo._id == 0) break;
                temp.StartSearch();

                yield return new WaitForSeconds(_treasureEntities[i]._treasureDuration);
            }
        }
        #endregion
    }
}
