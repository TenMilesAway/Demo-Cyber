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
        [SerializeField] private DSDialogueContainerSO _dialogueContainer; // 对话 Graph
        [SerializeField] private DSDialogueGroupSO _dialogueGroup;         // 对话 Group
        [SerializeField] private DSDialogueSO _dialogue;                   // 对话节点
        [SerializeField] private bool _groupedDialogues;                   // 是否仅使用分组对话
        [SerializeField] private bool _startingDialogueOnly;               // 是否仅使用开始对话节点
        [SerializeField] private int _selectedDialogueGroupIndex;          // 分组对话索引
        [SerializeField] private int _selectedDialogueIndex;               // 对话索引
        
        // 内部变量
        private const string _interactionPrompt = "按 F 进行对话"; // 交互提示语
        private bool _isInteractable;                             // 是否可交互

        public DSDialogueContainerSO DialogueContainer { get { return _dialogueContainer; } }
        public DSDialogueGroupSO DialogueGroup { get { return _dialogueGroup; } }
        public DSDialogueSO Dialogue { get { return _dialogue; } }

        /// 供外部调用
        [HideInInspector] public string InteractionPrompt => _interactionPrompt;
        [HideInInspector] public bool IsInteractable => _isInteractable;

        /// <summary>
        /// 交互：对话
        /// </summary>
        public void Interact(object interactor)
        {
            // 对话
            HADebug.LogFormat("开始对话 [{0}]", gameObject.name);
        }

        #region 接口预留字段
        public string DialogueID => throw new System.NotImplementedException();

        public Vector3 Position => throw new System.NotImplementedException();

        public void StartDialogue()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
