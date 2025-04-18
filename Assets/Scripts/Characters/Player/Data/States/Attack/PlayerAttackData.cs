using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class PlayerAttackData
    {
        [field: SerializeField] [field: Range(0f, 2.0f)] public float ComboDuration { get; private set; } = 0.5f;
        [field: SerializeField] [field: Range(0f, 0.5f)] public float ComboDeterTime { get; private set; } = 0.3f;
    }
}
