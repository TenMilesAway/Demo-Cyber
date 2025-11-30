using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class UnityObjectPoolFactory : BaseManager<UnityObjectPoolFactory>, IDisposable
    {
        public delegate T LoadFunc<out T>(string path);

        private readonly Dictionary<string, UnityObjectPool> _pools = new Dictionary<string, UnityObjectPool>();
        private bool _disposed;


        #region 资源释放
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnityObjectPoolFactory()
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
