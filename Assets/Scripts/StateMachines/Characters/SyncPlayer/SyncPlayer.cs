using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayer : MonoBehaviour
    {
        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public SyncPlayerMovementStateMachine movementStateMachine;

        private void Awake()
        {
            AnimationData.Initialize();

            Rigidbody = GetComponent<Rigidbody>();

            Animator = GetComponentInChildren<Animator>();

            movementStateMachine = new SyncPlayerMovementStateMachine(this);
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);
        }
    }
}
