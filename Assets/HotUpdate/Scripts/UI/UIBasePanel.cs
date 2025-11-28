using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 打开 UI 时的参数
    /// </summary>
    public class OpenUIParam
    {
        public object data;
        public Action callback;
    }

    /// <summary>
    /// UI 基类
    /// </summary>
    public class UIBasePanel : MonoBehaviour
    {

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit(OpenUIParam param)
        {
            InitHandle(param);
        }

        protected virtual void InitHandle(OpenUIParam param)
        {
            // 音效播放


        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void OnClose()
        {

        }

        protected virtual void CloseHandle()
        {
            // 音效播放

        }

        /// <summary>
        /// 显示
        /// </summary>
        public void OnShow()
        {
            gameObject.SetActive(true);
            ShowHandle();
        }

        protected virtual void ShowHandle()
        {

        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void OnHide()
        {
            gameObject.SetActive(false);
            HideHandle();
        }

        protected virtual void HideHandle()
        {

        }
    }
}
