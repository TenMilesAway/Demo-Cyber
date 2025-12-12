using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace HA
{
    public class InteractivePanelParam : OpenUIParam
    {

    }

    public class InteractivePanel : UIBasePanel
    {
        [SerializeField] private Text _txtPrompt;
        [SerializeField] private Transform _interactiveOptionContainer;

        private List<IInteractive> _interactives; // 可交互物体
        private List<GameObject> _interactiveGOs; // 可交互物体 GameObject
        private InteractiveOption _lastSelect;    // 刚才选中的选项
        private InteractiveOption _currentSelect; // 现在选中的选项

        private GameObject _interactiveOptionPrefab;

        public override string GetPanelName()
        {
            return GlobalDefine.InteractivePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            GameManager.Resource.LoadResourceAsync<GameObject>(GlobalDefine.InteractiveOption, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _interactiveOptionPrefab = obj as GameObject;
            });

            // 初始化可交互队列
            InteractivePanelParam interactivePanelParam = param as InteractivePanelParam;
            _interactives = interactivePanelParam.data as List<IInteractive>;
        }

        #region 主要方法
        public void UpdateInteractives(List<IInteractive> interactives)
        {
            int needToDestroy = _interactives.Count - interactives.Count;

            // 这里后续走对象池 API

            // 如果当前 List 的数量小于外部更新的数量, 则需要去生成新的 prefab
            if (needToDestroy < 0)
            {
                
            }
            // 如果大于或等于, 则根据情况去销毁
            else
            {
                for (int i = 0; i < needToDestroy; i++)
                {

                }

                _interactives.Clear();
                _interactives = interactives;
            }

            
        }
        #endregion
    }
}
