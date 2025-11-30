using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HA
{
    public class UnityObjectPool : IObjectPool<Object>, IDisposable
    {
        public Object ItemPrefab;

        private readonly Queue<Object> _objects;
        private readonly Func<Object> _objectFactory;
        private Action<Object> _enterQueueHandle;
        private Action<Object> _dequeueHandle;
        private const float WaitDestroyTime = 20f;
        private string _poolName;
        private bool _disposed = false;

        private static Transform s_PoolRoot;
        private static readonly object locker = new object();
        public static Transform PoolRoot
        {
            get
            {
                if (s_PoolRoot == null)
                {
                    lock (locker)
                    {
                        if (s_PoolRoot == null)
                        {
                            s_PoolRoot = new GameObject("PoolRoot").transform;
                            //s_PoolRoot.gameObject.AddComponent<Pool>
                        }
                    }
                }

                return s_PoolRoot;
            }
        }

        public UnityObjectPool(Object itemPrefab, string poolName, Func<Object> objectFactory, 
            Action<Object> enterQueueHandle = null, 
            Action<Object> deQueueHandle = null)
        {
            ItemPrefab        = itemPrefab;
            _poolName         = poolName;
            _objectFactory    = objectFactory;
            _enterQueueHandle = enterQueueHandle;
            _dequeueHandle    = deQueueHandle;
            _objects          = new Queue<Object>();
            _disposed         = false;
        }

        public Object Get(Vector3 vec = default)
        {
            Object item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();

            if (item != null) DequeueHandle(item, vec);

            _dequeueHandle?.Invoke(item);

            return item;
        }

        public void Put(Object item)
        {
            if (item == null) return;

            if (!_objects.Contains(item))
            {
                _enterQueueHandle?.Invoke(item);

                EnqueueHandle(item);

                _objects.Enqueue(item);
            }
        }

        public void EnqueueHandle(Object item)
        {
            if (item is GameObject obj)
            {
                obj.SetActive(false);
                obj.transform.SetParent(PoolRoot, false);
                // GameEntry.Timer
            }
        }

        public void DequeueHandle(Object item)
        {
            if (item is GameObject obj)
            {
                obj.SetActive(true);
                // GameEntry.Timer
            }
        }

        public void DequeueHandle(Object item, Vector3 vec)
        {
            if (item is GameObject obj)
            {
                if (vec != default)
                {
                    obj.transform.position = vec;
                }
                obj.SetActive(true);
                // GameEntry.Timer
            }
        }

        public void Clear(Func<Object, bool> shouldClear)
        {
            int count = _objects.Count;

            for (int i = 0; i < count; i++)
            {
                var obj = _objects.Dequeue();

                if (shouldClear(obj))
                {
                    Object.Destroy(obj);
                }
                else
                {
                    _objects.Enqueue(obj);
                }
            }
        }

        #region 主要方法
        public Queue<Object> GetPoolObject()
        {
            return _objects;
        }

        protected Object CreateObject()
        {
            var newObject = _objectFactory != null
                ? _objectFactory()
                : GameObject.Instantiate(ItemPrefab);

            _enterQueueHandle?.Invoke(newObject);

            EnqueueHandle(newObject);

            return newObject;
        }
        #endregion

        #region 资源释放
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnityObjectPool()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            // 已经释放
            if (_disposed) return;
            // 释放托管资源
            if (disposing)
            {

            }
            // 释放非托管资源
            // ...
            _disposed = true;
        }
        #endregion
    }
}
