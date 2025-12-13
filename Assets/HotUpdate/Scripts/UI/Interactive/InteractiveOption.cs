using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class InteractiveOption : MonoBehaviour
    {
        [SerializeField] private Image _imgSelect;
        [SerializeField] private Text _txtInteractiveOption;

        public void Init(string npcName, IInteractive obj, bool isShowImgSelect = false)
        {
            _imgSelect.gameObject.SetActive(isShowImgSelect);

            InitTextInteractiveOption(npcName, obj);
        }

        #region 主要方法
        /// <summary>
        /// 是否选中当前选项
        /// </summary>
        public void Select(bool isSelect = true)
        {
            _imgSelect.gameObject.SetActive(isSelect);
        }

        /// <summary>
        /// 更新交互选项文本
        /// </summary>
        public void UpdateTextInteractiveOption(string npcName, IInteractive obj)
        {
            if (obj is IDialogue)
            {
                HADebug.Log("这是一个对话可交互物体");
                _txtInteractiveOption.text = string.Format("和<color=#28E1E5>{0}</color>对话", npcName);
            }
            else if (obj is ITreasure)
            {
                HADebug.Log("这是一个宝箱可交互物体");
                _txtInteractiveOption.text = string.Format("开启<color=#28E1E5>{0}</color>", npcName);
            }
        }
        #endregion

        #region 辅助方法
        private void InitTextInteractiveOption(string npcName, IInteractive obj)
        {
            UpdateTextInteractiveOption(npcName, obj);
        }
        #endregion
    }
}
