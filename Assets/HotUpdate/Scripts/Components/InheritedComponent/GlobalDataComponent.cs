using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class GlobalDataComponent : BaseComponent
    {
        public string PlayerID { get; set; }

        public GameObject PoolRoot { get; set; }

        public void SetDontDestroyOnLoad(GameObject GO)
        {
            DontDestroyOnLoad(GO);
        }
    }
}
