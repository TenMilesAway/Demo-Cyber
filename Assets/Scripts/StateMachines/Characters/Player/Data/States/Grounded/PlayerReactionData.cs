using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class PlayerReactionData
    {
        [field: SerializeField] [field: Range(0.01f, 0.5f)] public float SpeedModifier { get; private set; } = 0.1f;
    }
}
