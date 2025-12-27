using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class HAOpenMap : MonoBehaviour, IFunction
    {
        [SerializeField] private string _functionName; // 功能名称

        private const string _interactionPrompt = "按<color=red> F </color>打开地图";
        private bool _isInteractable;

        public string InteractionName { get { return _functionName; } }
        public string InteractionPrompt { get { return _interactionPrompt; } }
        public bool IsInteractable { get { return _isInteractable; } }

        /// <summary>
        /// 交互：打开地图面板
        /// </summary>
        public void Interact(object interactor = null)
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.MapPanel);
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
