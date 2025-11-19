using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class PoolConfig
    {
        public GameObject prefab;                // 对象
        public int initialSize = 0;              // Prewarm 的对象数量
        public int maxSize = 100;                // 池最大拥有对象数量
        public float autoRecycleTime = 20f;      // 自动回收时间
    }

    public class ObjectPool<T> : IObjectPool<T>
    {

        public int ActiveCount => throw new System.NotImplementedException();

        public int InactiveCount => throw new System.NotImplementedException();

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public T Get()
        {
            throw new System.NotImplementedException();
        }

        public void Prewarm(int count)
        {
            throw new System.NotImplementedException();
        }

        public void Put(T obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
