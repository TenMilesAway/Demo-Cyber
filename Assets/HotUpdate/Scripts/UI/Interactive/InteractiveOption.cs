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
        /// 初始化交互选项文本
        /// </summary>
        private void InitTextInteractiveOption(string npcName, IInteractive obj)
        {
            UpdateTextInteractiveOption(npcName, obj);
        }

        /// <summary>
        /// 更新交互选项文本
        /// </summary>
        public void UpdateTextInteractiveOption(string npcName, IInteractive obj)
        {
            if (obj is IDialogue)
            {
                _txtInteractiveOption.text = string.Format("和<color=#28E1E5>{0}</color>对话", npcName);
            }
            else if (obj is ITreasure)
            {
                _txtInteractiveOption.text = string.Format("开启<color=#28E1E5>{0}</color>", npcName);
            }
            else if (obj is IFunction)
            { 
                if (obj is HAOpenMap)
                {
                    _txtInteractiveOption.text = string.Format("前往<color=#28E1E5>{0}</color>", npcName);
                }
                else if (obj is HAOpenConvert)
                {
                    _txtInteractiveOption.text = string.Format("和<color=#28E1E5>{0}</color>交易", npcName);
                }
            }
        }
        #endregion
    }
}
