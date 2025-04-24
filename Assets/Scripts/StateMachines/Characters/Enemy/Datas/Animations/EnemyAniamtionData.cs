using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class EnemyAniamtionData
    {
        [SerializeField] private string idleParameterName = "isIdling";
        [SerializeField] private string walkParameterName = "isWalking";

        public int IdleParameterHash { get; private set; }
        public int WalkParameterHash { get; private set; }

        public void Initialize()
        {
            IdleParameterHash = Animator.StringToHash(idleParameterName);
            WalkParameterHash = Animator.StringToHash(walkParameterName);
        }
    }
}
