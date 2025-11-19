using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public interface IObjectPool<T>
    {
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        void Put(T obj);

        /// <summary>
        /// 预热创建对象
        /// </summary>
        /// <param name="count"></param>
        void Prewarm(int count);

        /// <summary>
        /// 清理池对象
        /// </summary>
        void Clear();

        /// <summary>
        /// 激活对象数量
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// 未激活对象数量
        /// </summary>
        int InactiveCount { get; }
    }
}
