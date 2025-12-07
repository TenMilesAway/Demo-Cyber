using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 可交互物体接口
    /// </summary>
    public interface IInteractive
    {
        public void OnInteract();

        public bool IsInRange(Vector3 playerPosition);
    }
}
