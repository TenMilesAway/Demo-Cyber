using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class DSChoiceSaveData
    {
        [field: SerializeField]public string Text { get; set; }
        [field: SerializeField]public string NodeID { get; set; }

    }
}
