using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class HATreasureEntity
    {
        public int _treasureID;
        public int _treasureNum;
        public int _treasureLevel;
        public float _treasureDuration;
    }

    /// <summary>
    /// 可交互宝藏
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class HATreasure : MonoBehaviour, ITreasure
    {
        // 宝藏 SO
        [SerializeField] private string _treasureName; // 宝藏名称
        [SerializeField] private int _treasureID;      // 宝藏唯一 ID，用于初始化宝藏物品
        // 内部变量
        private List<HATreasureEntity> _treasureEntities = new List<HATreasureEntity>();
        private const string _interactionPrompt = "按<color=red> F </color>开启宝藏";
        private bool _isInteractable; // 在这里用来判断宝藏是否被打开过
        private int _instanceID;

        public string InteractionName { get { return _treasureName; } }
        public string InteractionPrompt { get { return _interactionPrompt; } }
        public bool IsInteractable { get { return _isInteractable; } }

        /// <summary>
        /// 交互：开启宝藏
        /// </summary>
        public void Interact(object interactor = null)
        {
            TreasurePanelParam treasurePanelParam = new TreasurePanelParam();
            treasurePanelParam._isInteractable = _isInteractable;
            // 未被打开过, 则初始化宝藏 Entities
            if (!_isInteractable)
            {
                _instanceID = GetInstanceID();
                _treasureEntities = HATreasureDataManager.GetInstance().InitHATreasure(_treasureID);
                _isInteractable = true;
                HATreasureDataManager.GetInstance().AddHATreasureListToDic(_instanceID, _treasureEntities);
            }
            treasurePanelParam._parentInstanceID = _instanceID;
            treasurePanelParam._treasureEntities = _treasureEntities;
            UIManager.GetInstance().OpenPanel(GlobalDefine.TreasurePanel, UILayer.Mid, treasurePanelParam);

            InventoryParam param = new InventoryParam();
            param.data = PlayerDataManager.GetInstance().GetPlayerInfo();
            param.isWithTreasurePanel = true;
            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param);
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
