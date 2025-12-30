using Cyber;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class EquipCell : MonoBehaviour
    {
        [SerializeField] private Image _bg;
        [SerializeField] private Image _iconEquip;
        [SerializeField] private Text _txtEquip;

        public ItemInfo _itemInfo = null;

        public void Init(ItemInfo info)
        {
            _itemInfo = info;

            if (info == null || info._id == 0)
            {
                _txtEquip.gameObject.SetActive(true);
                _iconEquip.gameObject.SetActive(false);
                return;
            }

            TBItemData data = ItemDataManager.GetInstance().GetData(info._id);
            GameManager.Resource.LoadResourceAsync<Sprite>(data.icon, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _iconEquip.sprite = obj as Sprite;
            });
            _iconEquip.gameObject.SetActive(true);
            _txtEquip.gameObject.SetActive(false);

            AddListeners();
        }

        private void AddListeners()
        {
            UIManager.GetInstance().AddCustomEventListener(_bg, EventTriggerType.PointerClick, ClickEquipCell);
        }

        public void RemoveListeners()
        {
            UIManager.GetInstance().RemoveCustomEventListener(_bg, EventTriggerType.PointerClick, ClickEquipCell);
        }

        #region 主要方法：卸下装备
        public void TakeOffEquipment()
        {
            if (_itemInfo == null || _itemInfo._id == 0) return;

            int idInParent = -1;
            PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();

            for (int i = 0; i < playerInfo._allItems.Count; i++)
            {
                if (playerInfo._allItems[i]._id == 0 || playerInfo._allItems[i] == null)
                {
                    idInParent = i;
                    break;
                }
            }

            if (idInParent == -1)
            {
                // 背包满了
                return;
            }

            // 从列表中移除
            playerInfo._nowEquips.RemoveAll(item => item._id == _itemInfo._id);
            _txtEquip.gameObject.SetActive(true);
            _iconEquip.gameObject.SetActive(false);
            RemoveListeners();

            // 放入背包中
            playerInfo._allItems[idInParent] = _itemInfo;
            GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);

            // 减去属性
            TBItemData dataTakeOff = ItemDataManager.GetInstance().GetData(_itemInfo._id);
            playerInfo._pAttack -= dataTakeOff.attack;
            playerInfo._pArmorPenetration -= dataTakeOff.armorPenetration;
            playerInfo._pDefense -= dataTakeOff.defense;
            playerInfo._pDamageAvoidance -= dataTakeOff.damageAvoidance;
            playerInfo._maxHP -= dataTakeOff.hp;
            playerInfo._currentHP -= dataTakeOff.hp;
            playerInfo._maxMP -= dataTakeOff.mp;
            playerInfo._currentMP -= dataTakeOff.mp;
            playerInfo._pCriticalProbability -= dataTakeOff.cp;
            playerInfo._pCriticalMultiplier -= dataTakeOff.cm;
            playerInfo._pSuckProbability -= dataTakeOff.sp;
            playerInfo._pSuckMultiplier -= dataTakeOff.sm;
            PlayerDataManager.GetInstance().SetPlayerInfo(playerInfo);
            GameManager.Event.Broadcast(GameEventType.UpdatePropertyPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerStatsSave);
        }
        #endregion

        #region 监听方法：打开装备提示面板
        private void ClickEquipCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<EquipCell>(GameEventType.ClickEquipCell, this);
        }
        #endregion
    }
}
