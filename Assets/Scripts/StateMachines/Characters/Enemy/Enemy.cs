using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyber
{
    public class Enemy : MonoBehaviour
    {
        private EnemyMovementStateMachine movementStateMachine;

        [field: SerializeField] public EnemyAniamtionData AniamtionData { get; private set; }

        public Animator Animator { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        public Transform nowPosition;

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            Agent = GetComponent<NavMeshAgent>();

            AniamtionData.Initialize();

            movementStateMachine = new EnemyMovementStateMachine(this);
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.WanderState);
        }

        private void Update()
        {
            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }
    }
}
