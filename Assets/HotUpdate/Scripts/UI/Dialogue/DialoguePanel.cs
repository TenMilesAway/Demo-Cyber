using Cyber;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace HA
{
    public class DialoguePanelParam : OpenUIParam
    {

    }

    public class DialoguePanel : UIBasePanel
    {
        [SerializeField] private Transform _dialogueOptionContainer;
        [SerializeField] private Image _imgHead;
        [SerializeField] private Text _txtDialogue;
        [SerializeField] private Button _btnDialogueCancel;

        private HADialogue _dialogue;
        // SO 用的还是 DialogueSystem 的原脚本
        private DSDialogueSO _currentDialogueSO;
        private DSDialogueSO _nextDialogueSO;
        private List<GameObject> _dialogueOptions = new List<GameObject>();
        private GameObject _optionPrefab;

        public override string GetPanelName()
        {
            return GlobalDefine.DialoguePanel;
        }

        protected override async void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            // 读取 HADialogue 的信息, 用于初始化对话
            DialoguePanelParam dialoguePanelParam = param as DialoguePanelParam;
            _dialogue = dialoguePanelParam.data as HADialogue;
            _currentDialogueSO = _dialogue.Dialogue;

            AsyncOperationHandle optionHandle = Addressables.LoadAssetAsync<GameObject>(GlobalDefine.DialogueOption);
            await optionHandle.Task;
            _optionPrefab = optionHandle.Task.Result as GameObject;

            _btnDialogueCancel.onClick.AddListener(CancelDialogue);

            PlayNextDialogue();
        }

        #region 主要方法
        /// <summary>
        /// 播放下一个对话
        /// </summary>
        public void PlayNextDialogue()
        {
            if (_nextDialogueSO != null)
            {
                _currentDialogueSO = _nextDialogueSO;
            }

            Sequence dialogueSequence = DOTween.Sequence();
            dialogueSequence.Append(_txtDialogue.DOText(_currentDialogueSO.Text, 2, true, ScrambleMode.All));
            dialogueSequence.OnComplete(() =>
            {
                InitOptions();
            });
        }

        /// <summary>
        /// 初始化对话选项
        /// </summary>
        public void InitOptions()
        {
            // 如果没有后续对话 (已经到达[结束对话])
            if (_currentDialogueSO.Choices.Count == 1 && _currentDialogueSO.Choices[0].NextDialogue == null)
            {
                GameObject option = Instantiate(_optionPrefab);

                _dialogueOptions.Add(option);

                option.transform.SetParent(_dialogueOptionContainer, false);

                option.GetComponent<DialogueOption>().Init("结束对话", 0, DialogueOver);

                return;
            }

            // 有后续选项
            for (int i = 0; i <  _currentDialogueSO.Choices.Count; i++)
            {
                GameObject option = Instantiate(_optionPrefab);

                _dialogueOptions.Add(option);

                option.transform.SetParent(_dialogueOptionContainer, false);

                // 防止闭包
                int index = i;

                option.GetComponent<DialogueOption>().Init(_currentDialogueSO.Choices[i].Text, index, () =>
                {
                    _nextDialogueSO = _currentDialogueSO.Choices[index].NextDialogue;

                    ClearCurrentOptions();

                    PlayNextDialogue();
                });
            }
        }

        private void ClearCurrentOptions()
        {
            foreach (GameObject option in _dialogueOptions)
            {
                Destroy(option);
            }

            _dialogueOptions.Clear();
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 中止对话
        /// </summary>
        public void CancelDialogue()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.DialoguePanel);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        private void DialogueOver()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.DialoguePanel);
        }
        #endregion
    }
}
