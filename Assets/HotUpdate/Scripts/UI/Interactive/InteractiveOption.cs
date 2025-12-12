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

        public void Init(string npcName, Type type)
        {
            _imgSelect.gameObject.SetActive(false);

            InitTextInteractiveOption(npcName, type);
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
        public void UpdateTextInteractiveOption(string npcName, Type type)
        {
            if (type == typeof(IDialogue))
            {
                _txtInteractiveOption.text = string.Format("和<color=#28E1E5>{0}</color>对话", npcName);
            }
            else if (type == typeof(ITreasure))
            {
                _txtInteractiveOption.text = string.Format("开启<color=#28E1E5>{0}</color>", npcName);
            }
        }
        #endregion

        #region 辅助方法
        private void InitTextInteractiveOption(string npcName, Type type)
        {
            UpdateTextInteractiveOption(_imgSelect.name, type);
        }
        #endregion
    }
}
