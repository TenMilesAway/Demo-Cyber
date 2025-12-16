using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyber;

namespace HA
{
    /// <summary>
    /// 此类区别于 DSDialogue, 隔离出来的原因是为了统一接口管理
    /// </summary>
    public class HADialogue : MonoBehaviour, IDialogue
    {
        // 对话 SO
        [SerializeField] private string _NPCName;                          // NPC 名称
        [SerializeField] private DSDialogueContainerSO _dialogueContainer; // 对话 Graph
        [SerializeField] private DSDialogueGroupSO _dialogueGroup;         // 对话 Group
        [SerializeField] private DSDialogueSO _dialogue;                   // 对话节点
        [SerializeField] private bool _groupedDialogues;                   // 是否仅使用分组对话
        [SerializeField] private bool _startingDialogueOnly;               // 是否仅使用开始对话节点
        [SerializeField] private int _selectedDialogueGroupIndex;          // 分组对话索引
        [SerializeField] private int _selectedDialogueIndex;               // 对话索引
        // 内部变量
        private const string _interactionPrompt = "按<color=red> F </color>进行对话";         // 交互提示语
        private bool _isInteractable;                                      // 是否可交互

        public string InteractionName { get { return _NPCName; } }
        public DSDialogueContainerSO DialogueContainer { get { return _dialogueContainer; } }
        public DSDialogueGroupSO DialogueGroup { get { return _dialogueGroup; } }
        public DSDialogueSO Dialogue { get { return _dialogue; } }
        public string InteractionPrompt { get { return _interactionPrompt; } }
        public bool IsInteractable { get { return _isInteractable; } }


        /// <summary>
        /// 交互：对话
        /// </summary>
        public void Interact(object interactor = null)
        {
            // 对话
            HADebug.LogFormat("开始对话 [{0}]", gameObject.name);
            DialoguePanelParam param = new DialoguePanelParam();
            param.data = this;
            UIManager.GetInstance().OpenPanel(GlobalDefine.DialoguePanel, UILayer.Mid, param);
        }

        #region 接口预留字段
        public Vector3 Position => throw new System.NotImplementedException();
        #endregion
    }
}
