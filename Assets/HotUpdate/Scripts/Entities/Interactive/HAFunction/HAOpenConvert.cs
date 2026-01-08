using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class HAOpenConvert : MonoBehaviour, IFunction
    {
        [SerializeField] private int _convertID;
        [SerializeField] private string _functionName; // 功能名称

        private const string _interactionPrompt = "按<color=red> F </color>开启兑换界面";
        private bool _isInteractable;

        public string InteractionName { get { return _functionName; } }
        public string InteractionPrompt { get { return _interactionPrompt; } }
        public bool IsInteractable { get { return _isInteractable; } }

        /// <summary>
        /// 交互：打开地图面板
        /// </summary>
        public void Interact(object interactor = null)
        {
            ConvertParam param = new ConvertParam();
            param.id = _convertID;

            UIManager.GetInstance().OpenPanel(GlobalDefine.ConvertPanel, UILayer.Mid, param);

            InventoryParam param2 = new InventoryParam();
            param2.isWithConvertPanel = true;
            UIManager.GetInstance().OpenPanel(GlobalDefine.InventoryPanel, UILayer.Mid, param2);
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
