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

        private List<IInteractive> _interactives;            // 可交互物体
        private List<InteractiveOption> _interactiveOptions; // 可交互物体 InteractiveOption
        private int _lastSelectIndex;                        // 刚才选中的选项
        private int _currentSelectIndex;                     // 现在选中的选项

        public override string GetPanelName()
        {
            return GlobalDefine.InteractivePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            _interactiveOptionContainer.gameObject.SetActive(false);

            // 初始化可交互队列
            _interactives = new List<IInteractive>();
            _interactiveOptions = new List<InteractiveOption>();

            // 初始化选择索引
            _currentSelectIndex = 0;
            _lastSelectIndex = 0;

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        #region 主要方法
        private void UpdateInteractives(List<IInteractive> interactives)
        {
            int needToDestroy = _interactives.Count - interactives.Count;

            // 如果当前 List 的数量小于外部更新的数量, 则需要去生成新的 prefab
            if (needToDestroy < 0)
            {
                int count = -needToDestroy;
                for (int i = 0; i < count; i++)
                {
                    UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.InteractiveOption, GetInstanceID().ToString(), (GameObject interactiveOption) =>
                    {
                        interactiveOption.transform.SetParent(_interactiveOptionContainer, false);
                        _interactiveOptions.Add(interactiveOption.GetComponent<InteractiveOption>());
                        if (!_interactiveOptionContainer.gameObject.activeSelf) _interactiveOptionContainer.gameObject.SetActive(true);

                        _interactives = new List<IInteractive>(interactives);
                        UpdateGOs();
                    });
                }
            }
            // 如果大于或等于, 则根据情况去销毁
            else
            {
                for (int i = 0; i < needToDestroy; i++)
                {
                    // 移除末尾
                    UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.InteractiveOption, _interactiveOptions[_interactiveOptions.Count - 1].gameObject);
                    _interactiveOptions.RemoveAt(_interactiveOptions.Count - 1);
                }

                _interactives = new List<IInteractive>(interactives);
                UpdateGOs();
            }
        }
        #endregion

        #region 监听方法
        private void AddListeners()
        {
            GameManager.Event.AddListener<List<IInteractive>>(GameEventType.UpdateInteractiveList, UpdateInteractives);
        }

        private void RemoveListeners()
        {
            GameManager.Event.RemoveListener<List<IInteractive>>(GameEventType.UpdateInteractiveList, UpdateInteractives);
        }
        #endregion

        #region 辅助方法
        private void UpdateGOs()
        {
            _currentSelectIndex = _currentSelectIndex >= _interactives.Count ? _interactives.Count - 1 : _currentSelectIndex;

            for (int i = 0; i < _interactives.Count; i++)
            {
                int index = i;
                _interactiveOptions[index].Init(_interactives[index].InteractionName, _interactives[index], index == _currentSelectIndex);
            }
        }
        #endregion
    }
}
