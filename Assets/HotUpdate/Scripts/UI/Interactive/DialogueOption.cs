using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class DialogueOption : MonoBehaviour
    {
        [SerializeField] private Button _btnDialogueOption;
        [SerializeField] private Text _txtDialogueOption;

        private int _dialogueIndex;

        /// <summary>
        /// 初始化 DialogueOption
        /// </summary>
        /// <param name="dialogueOptionText">选项文本</param>
        /// <param name="dialogueIndex">选项索引</param>
        public void Init(string dialogueOptionText, int dialogueIndex, Action callback)
        {
            _txtDialogueOption.text = dialogueOptionText;
            _dialogueIndex = dialogueIndex;
            _btnDialogueOption.onClick.AddListener(() => OnClickOptionButton(callback));
        }

        /// <summary>
        /// 获得对话选项的索引, 用于进入下一组对话
        /// </summary>
        public int GetDialogueOptionIndex()
        {
            return _dialogueIndex;
        }

        public void ClearAllListeners()
        {
            _btnDialogueOption.onClick.RemoveAllListeners();
        }

        #region 监听方法
        private void OnClickOptionButton(Action callback)
        {
            callback?.Invoke();
        }
        #endregion
    }
}
