using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class HAOpenConvert : MonoBehaviour, IFunction
    {
        [SerializeField] private int _convertID;
        [SerializeField] private string _functionName; // 功能名称

        [Space(10)]
        [SerializeField] private Transform _canvas;
        [SerializeField] private bool _lockY = false;  // 是否只绕 Y 轴旋转
        [SerializeField] [Range(0f, 20f)] private float _smooth = 0f; // 旋转平滑系数
        [SerializeField] private float _visibleDistance = 30f;

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

        private void LateUpdate()
        {
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (distance > _visibleDistance && !_canvas.gameObject.activeSelf) return;
            else if (distance > _visibleDistance && _canvas.gameObject.activeSelf)
            {
                _canvas.gameObject.SetActive(false);
                return;
            }

            _canvas.gameObject.SetActive(true);

            Vector3 camPos = Camera.main.transform.position;
            Vector3 dir = _canvas.position - camPos;

            if (dir.sqrMagnitude <= Mathf.Epsilon) return;

            if (_lockY)
            {
                dir.y = 0f;
                if (dir.sqrMagnitude <= Mathf.Epsilon) return;
            }

            Quaternion targetRot = Quaternion.LookRotation(dir);
            if (_smooth > 0f)
            {
                _canvas.rotation = Quaternion.Slerp(_canvas.rotation, targetRot, Mathf.Clamp01(Time.deltaTime * _smooth));
            }
            else
            {
                _canvas.rotation = targetRot;
            }
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
