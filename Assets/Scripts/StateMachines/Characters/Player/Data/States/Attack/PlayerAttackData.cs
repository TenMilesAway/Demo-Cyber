using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class PlayerAttackData
    {
        [field: SerializeField] [field: Range(0f, 0.5f)] public float ComboDeterTime { get; private set; } = 0.3f;
        [field: SerializeField] [field: Range(0.5f, 1f)] public float ComboWaitTime { get; private set; } = 0.5f;

        [field: SerializeField] [field: Range(0.1f, 0.5f)] public float SpeedModified { get; private set; } = 0.3f;
    }
}
