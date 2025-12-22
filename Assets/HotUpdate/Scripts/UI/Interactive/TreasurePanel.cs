using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class TreasurePanelParam : OpenUIParam
    {
        public bool isInteractable;
        public List<HATreasureEntity> treasureEntities;
    }

    public class TreasurePanel : UIBasePanel
    {
        [SerializeField] private Transform _treasureContainer;
        [SerializeField] private Transform _safeboxContainer;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Image _imgTitle;

        private List<HATreasureEntity> _treasureEntities;
        private List<GameObject> _itemCells = new List<GameObject>();
        private bool _isInteractable;
        private Coroutine _searchCo;
        public override string GetPanelName()
        {
            return GlobalDefine.TreasurePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            TreasurePanelParam treasurePanelParam = new TreasurePanelParam();
            treasurePanelParam = param as TreasurePanelParam;
            _isInteractable = treasurePanelParam.isInteractable;
            _treasureEntities = treasurePanelParam.treasureEntities;

            // 先初始化 15 个槽位
            // 将指定槽位数初始化为 Treasure
            // 再将剩下的槽位初始化为空
            InitItemCells();
            InitSafeBox();

            if (!_isInteractable)
            {
                _searchCo = StartCoroutine(StartSearch());
            }
        }

        protected override void HideHandle()
        {
            base.HideHandle();

            foreach (GameObject cell in _itemCells)
            {
                cell.GetComponent<ItemCell>().RemoveAllListeners();
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, cell);
            }
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            StopAllCoroutines();
            _searchCo = null;
        }

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
                        itemCell.GetComponent<ItemCell>().Init(item, true, _treasureEntities[index], ItemCellParent.Treasure);
                    }
                    else
                    {
                        itemCell.GetComponent<ItemCell>().Init(item, true, null, ItemCellParent.Treasure);
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
                    itemCell.GetComponent<ItemCell>().Init(null, true, null, ItemCellParent.Treasure);
                });
            }
        }

        private void InitSafeBox()
        {
            // 后期通过数据单例类获取 safebox 的数量再初始化 ItemCell
        }

        private IEnumerator StartSearch()
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < _treasureEntities.Count; i++)
            {
                _itemCells[i].GetComponent<ItemCell>().StartSearch();

                yield return new WaitForSeconds(_treasureEntities[i]._treasureDuration);
            }
        }
    }
}
