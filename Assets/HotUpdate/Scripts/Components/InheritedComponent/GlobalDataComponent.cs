using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class GlobalDataComponent : BaseComponent
    {
        [HideInInspector] public string PlayerID { get; set; }         // 玩家 ID
        [HideInInspector] public GameObject PoolRoot { get; set; }     // 对象池根节点
        [HideInInspector] public bool IsInit { get; set; } = false;    // 是否已初始化过部分全局数据
        [HideInInspector] public GameObject Player { get; set; }       // 玩家

        public void SetDontDestroyOnLoad(GameObject GO)
        {
            DontDestroyOnLoad(GO);
        }
    }
}
