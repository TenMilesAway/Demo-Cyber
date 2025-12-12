using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class InteractiveDataManager : BaseManager<InteractiveDataManager>
    {
        // 当前可交互物体
        private List<IInteractive> _interactives = new List<IInteractive>();

        public List<IInteractive> CurrentInteractives { get { return _interactives; } }

        #region 主要方法
        /// <summary>
        /// Update：是否有可交互物体
        /// </summary>
        public void UpdateForInteractives()
        {
            if (_interactives.Count == 0) return;
        }

        public void ClearInteractives()
        {
            _interactives.Clear();
        }

        /// <summary>
        /// 添加可交互物体
        /// </summary>
        public void AddInteractive(IInteractive obj)
        {
            if (!_interactives.Contains(obj))
            {
                _interactives.Add(obj);
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
                HADebug.LogFormat("删除一个可交互物体[{0}], 当前可交互物体总数[{1}]", obj.GetType().Name, _interactives.Count);
            }
        }
        #endregion
    }
}
