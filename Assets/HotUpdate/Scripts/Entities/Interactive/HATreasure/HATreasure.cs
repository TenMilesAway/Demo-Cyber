using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 可交互宝藏
    /// </summary>
    public class HATreasure : MonoBehaviour, ITreasure
    {
        // 宝藏 SO
        [SerializeField] private string _NPCName; // 宝藏名称
        // 内部变量
        private const string _interactionPrompt = "按 <color=red> F </color>开启宝藏";
        private bool _isInteractable;

        public string InteractionName { get { return _NPCName; } }
        public string InteractionPrompt { get { return _interactionPrompt; } }
        public bool IsInteractable { get { return _isInteractable; } }

        /// <summary>
        /// 交互：开启宝藏
        /// </summary>
        public void Interact(object interactor = null)
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.TreasurePanel);
            InventoryParam param = new InventoryParam();
            param.data = PlayerDataManager.GetInstance().GetPlayerInfo();

            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param);
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
