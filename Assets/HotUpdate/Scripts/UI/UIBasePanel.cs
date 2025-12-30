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
    public abstract class UIBasePanel : MonoBehaviour
    {
        public bool _isBlockingWindow = true;

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit(OpenUIParam param)
        {
            InitHandle(param);
        }

        protected virtual void InitHandle(OpenUIParam param)
        {
            
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void OnClose()
        {
            OnHide();
            CloseHandle();
        }

        protected virtual void CloseHandle()
        {
            bool hasBlockingWindow = UIManager.GetInstance().hasBlockingWindow();

            // 鼠标状态和输入监听
            if (!hasBlockingWindow && Cursor.visible) GameManager.Event.Broadcast(GameEventType.ToggleCursor);

            // 开启输入
            if (!hasBlockingWindow)
            {
                GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            }
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
            bool hasBlockingWindow = UIManager.GetInstance().hasBlockingWindow();

            // 鼠标状态和输入监听
            if (hasBlockingWindow && !Cursor.visible) GameManager.Event.Broadcast(GameEventType.ToggleCursor);

            // 禁用输入
            if (hasBlockingWindow)
            {
                GameManager.Event.Broadcast(GameEventType.DisablePlayerInput);
            }
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

        protected virtual void OnDestroy()
        {
            GameManager.Resource.Release(GetInstanceID().ToString());
        }

        public abstract string GetPanelName();
    }
}
