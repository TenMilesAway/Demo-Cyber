using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
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
        private Cyber.PlayerInput _playerInput;              // 用户输入组件
        private int _lastSelectIndex;                        // 刚才选中的选项
        private int _currentSelectIndex;                     // 现在选中的选项
        private bool _isFirstShow;                           // 是否为第一次显示 (用于对话提示更新)

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
            _playerInput = PlayerDataManager.GetInstance().GetPlayerInput();

            // 初始化选择索引
            _currentSelectIndex = 0;
            _lastSelectIndex = 0;

            _isFirstShow = true;

            // 初始化一些监听
            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        #region 主要方法
        /// <summary>
        /// 刷新可交互物体列表
        /// </summary>
        private void UpdateInteractives(List<IInteractive> interactives)
        {
            int needToDestroy = _interactives.Count - interactives.Count;

            if (_isFirstShow)
            {
                _txtPrompt.text = interactives[0].InteractionPrompt;
                _isFirstShow = false;
            }

            // 如果当前 List 的数量小于外部更新的数量, 则需要去生成新的 prefab
            if (needToDestroy < 0)
            {
                int count = -needToDestroy;
                for (int i = 0; i < count; i++)
                {
                    int index = i;
                    UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.InteractiveOption, GetInstanceID().ToString(), (GameObject interactiveOption) =>
                    {
                        interactiveOption.transform.SetParent(_interactiveOptionContainer, false);
                        _interactiveOptions.Add(interactiveOption.GetComponent<InteractiveOption>());
                        if (index == count - 1)
                        {
                            _interactives = new List<IInteractive>(interactives);
                            UpdateGOs();
                            if (!_interactiveOptionContainer.gameObject.activeSelf) _interactiveOptionContainer.gameObject.SetActive(true);
                        }
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

        private void OnInteractiveStarted(InputAction.CallbackContext context)
        {
            string controlName = context.control.name;

            if (controlName == "upArrow")
            {
                // 上一个选项
                _interactiveOptions[_lastSelectIndex].Select(false);
                _currentSelectIndex = _currentSelectIndex >= _interactives.Count - 1 ? _currentSelectIndex : _currentSelectIndex + 1;
                _interactiveOptions[_currentSelectIndex].Select(true);
                _lastSelectIndex = _currentSelectIndex;
                _txtPrompt.text = _interactives[_currentSelectIndex].InteractionPrompt;
            }
            else if (controlName == "downArrow")
            {
                // 下一个选项
                _interactiveOptions[_lastSelectIndex].Select(false);
                _currentSelectIndex = _currentSelectIndex <= 0 ? _currentSelectIndex : _currentSelectIndex - 1;
                _interactiveOptions[_currentSelectIndex].Select(true);
                _lastSelectIndex = _currentSelectIndex;
                _txtPrompt.text = _interactives[_currentSelectIndex].InteractionPrompt;
            }
        }

        private void StartInteractive(InputAction.CallbackContext context)
        {
            GameManager.Event.Broadcast(GameEventType.DisablePlayerInput);
            
            // 开始对话
            if (_interactives[_currentSelectIndex] is IDialogue)
            {
                //DialoguePanelParam param = new DialoguePanelParam();
                //param.data = _interactives[_currentSelectIndex] as HADialogue;
                //UIManager.GetInstance().OpenPanel(GlobalDefine.DialoguePanel, UILayer.Mid, param);
                (_interactives[_currentSelectIndex] as HADialogue).Interact();
            }
            else if (_interactives[_currentSelectIndex] is ITreasure)
            {
                (_interactives[_currentSelectIndex] as HATreasure).Interact();
            }

            UIManager.GetInstance().ClosePanel(GetPanelName());
        }
        #endregion

        #region 监听方法
        private void AddListeners()
        {
            GameManager.Event.AddListener<List<IInteractive>>(GameEventType.UpdateInteractiveList, UpdateInteractives);

            GameManager.Event.Broadcast(GameEventType.EnableInteractiveInput);

            _playerInput.PlayerActions.InteractiveOption.started += OnInteractiveStarted;
            _playerInput.PlayerActions.Interaction.started += StartInteractive;
        }

        private void RemoveListeners()
        {
            GameManager.Event.RemoveListener<List<IInteractive>>(GameEventType.UpdateInteractiveList, UpdateInteractives);

            GameManager.Event.Broadcast(GameEventType.DisableInteractiveInput);

            _playerInput.PlayerActions.InteractiveOption.started -= OnInteractiveStarted;
            _playerInput.PlayerActions.Interaction.started -= StartInteractive;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 更新 InteractiveOption 的数据
        /// </summary>
        private void UpdateGOs()
        {
            _currentSelectIndex = _currentSelectIndex >= _interactives.Count ? _interactives.Count - 1 : _currentSelectIndex;
            _lastSelectIndex = _lastSelectIndex >= _interactives.Count ? _interactives.Count - 1 : _lastSelectIndex;
            _txtPrompt.text = _interactives[_currentSelectIndex].InteractionPrompt;

            for (int i = 0; i < _interactives.Count; i++)
            {
                int index = i;
                _interactiveOptions[index].Init(_interactives[index].InteractionName, _interactives[index], index == _currentSelectIndex);
            }
        }
        #endregion
    }
}
