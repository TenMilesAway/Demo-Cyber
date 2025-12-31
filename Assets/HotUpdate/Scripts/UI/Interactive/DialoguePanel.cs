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
        private DSDialogueSO _startDialogueSO;
        private List<GameObject> _dialogueOptions = new List<GameObject>();
        private Sequence dialogueSequence;

        public override string GetPanelName()
        {
            return GlobalDefine.DialoguePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            // 读取 HADialogue 的信息, 用于初始化对话
            DialoguePanelParam dialoguePanelParam = param as DialoguePanelParam;
            _dialogue = dialoguePanelParam.data as HADialogue;
            _startDialogueSO = _dialogue.Dialogue;
            _currentDialogueSO = _dialogue.Dialogue;

            _btnDialogueCancel.onClick.AddListener(CancelDialogue);

            PlayNextDialogue();
        }

        protected override void ShowHandle()
        {
            base.ShowHandle();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            dialogueSequence.Kill();
            dialogueSequence = null;

            _btnDialogueCancel.onClick.RemoveAllListeners();
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

            dialogueSequence = DOTween.Sequence();
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
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.DialogueOption, GetInstanceID().ToString(), (GameObject dialogueOption) =>
                {
                    _dialogueOptions.Add(dialogueOption);
                    dialogueOption.transform.SetParent(_dialogueOptionContainer, false);
                    dialogueOption.GetComponent<DialogueOption>().Init("结束对话", 0, DialogueOver);

                    // 也许未来可以在这里加入对话的回调逻辑：任务、触发其它
                });

                return;
            }

            // 有后续选项
            for (int i = 0; i <  _currentDialogueSO.Choices.Count; i++)
            {
                int index = i;
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.DialogueOption, GetInstanceID().ToString(), (GameObject dialogueOption) =>
                {
                    _dialogueOptions.Add(dialogueOption);
                    dialogueOption.transform.SetParent(_dialogueOptionContainer, false);
                    // 记住，一定要排查闭包问题
                    dialogueOption.GetComponent<DialogueOption>().Init(_currentDialogueSO.Choices[index].Text, index, () =>
                    {
                        _nextDialogueSO = _currentDialogueSO.Choices[index].NextDialogue;
                        ClearCurrentOptions();
                        PlayNextDialogue();
                    });
                });
            }
        }

        /// <summary>
        /// 清除当前的选项
        /// </summary>
        private void ClearCurrentOptions()
        {
            foreach (GameObject option in _dialogueOptions)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.DialogueOption, option, () =>
                {
                    option.GetComponent<DialogueOption>().ClearAllListeners();
                });
            }

            _dialogueOptions.Clear();
        }
        #endregion

        #region 监听方法：UI
        /// <summary>
        /// 中止对话
        /// </summary>
        public void CancelDialogue()
        {
            UIManager.GetInstance().ClosePanelAndDestory(GetPanelName());
            ClearCurrentOptions();
            GameManager.Event.Broadcast(GameEventType.HasInteractiveObject);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        private void DialogueOver()
        {
            UIManager.GetInstance().ClosePanelAndDestory(GetPanelName());
            ClearCurrentOptions();
            GameManager.Event.Broadcast(GameEventType.HasInteractiveObject);
        }
        #endregion

        #region 辅助方法：重置
        /// <summary>
        /// 重置对话
        /// </summary>
        private void ResetDialogue()
        {
            _currentDialogueSO = _startDialogueSO;
            _txtDialogue.text = _currentDialogueSO.Text;
        }
        #endregion
    }
}
