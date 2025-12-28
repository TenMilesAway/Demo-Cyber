using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class InteractiveDataManager : BaseManager<InteractiveDataManager>
    {
        private List<IInteractive> _interactives = new List<IInteractive>(); // 当前可交互物体

        public List<IInteractive> CurrentInteractives { get { return _interactives; } }

        public void Init()
        {
            GameManager.Event.AddListener(GameEventType.HasInteractiveObject, HasInteractiveObject);
            GameManager.Event.AddListener(GameEventType.NoneInteractiveObject, NoneInteractiveObject);
        }

        #region 主要方法
        /// <summary>
        /// 清除可交互物体
        /// </summary>
        public void ClearInteractives()
        {
            _interactives.Clear();
        }

        /// <summary>
        /// 获得可交互物体
        /// </summary>
        public List<IInteractive> GetInteractives()
        {
            return _interactives;
        }

        /// <summary>
        /// 添加可交互物体
        /// </summary>
        public void AddInteractive(IInteractive obj)
        {
            if (!_interactives.Contains(obj))
            {
                int beforeAddCount = _interactives.Count;

                _interactives.Add(obj);
                // 如果在添加前, 可交互物体为 0
                if (beforeAddCount == 0)
                {
                    // Pos: InteractiveDataManager
                    GameManager.Event.Broadcast(GameEventType.HasInteractiveObject);
                }
                else
                {
                    // Pos: InteractivePanel
                    GameManager.Event.Broadcast<List<IInteractive>>(GameEventType.UpdateInteractiveList, _interactives);
                }
                HADebug.LogFormat("添加一个可交互物体[{0}], 当前可交互物体总数[{1}]", obj.GetType().Name, _interactives.Count);
            }
        }

        /// <summary>
        /// 移除可交互物体
        /// </summary>
        public void RemoveInteractive(IInteractive obj)
        {
            if (_interactives.Contains(obj))
            {
                _interactives.Remove(obj);
                // 移除后, 可交互物体为 0
                if (_interactives.Count == 0)
                {
                    // Pos: InteractiveDataManager
                    GameManager.Event.Broadcast(GameEventType.NoneInteractiveObject);
                }
                else
                {
                    // Pos: InteractivePanel
                    GameManager.Event.Broadcast<List<IInteractive>>(GameEventType.UpdateInteractiveList, _interactives);
                }
                HADebug.LogFormat("删除一个可交互物体[{0}], 当前可交互物体总数[{1}]", obj.GetType().Name, _interactives.Count);
            }
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 当队列里有可交互物体时调用
        /// </summary>
        private void HasInteractiveObject()
        {
            UIManager.GetInstance().OpenPanel(GlobalDefine.InteractivePanel, UILayer.Top, null, () => {
                // Pos: InteractivePanel
                GameManager.Event.Broadcast<List<IInteractive>>(GameEventType.UpdateInteractiveList, _interactives);
            });
        }

        /// <summary>
        /// 当队列里无可交互物体时调用
        /// </summary>
        private void NoneInteractiveObject()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.InteractivePanel);
        }
        #endregion
    }
}
