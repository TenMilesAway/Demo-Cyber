using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class InteractiveDataManager : BaseManager<InteractiveDataManager>
    {
        // 当前可交互物体
        private List<IInteractive> _interactives = new List<IInteractive>();

        /// <summary>
        /// 当前可交互物体 (只读)
        /// </summary>
        public IReadOnlyList<IInteractive> CurrentInteractives => _interactives;

        #region 主要方法
        public void AddInteractive(IInteractive obj)
        {
            if (!_interactives.Contains(obj))
            {
                _interactives.Add(obj);
                HADebug.LogFormat("添加一个可交互物体[{0}], 当前可交互物体总数[{1}]", obj.GetType().Name, _interactives.Count);
            }
        }

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
