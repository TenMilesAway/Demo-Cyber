using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyber
{
    public class EnemyWanderState : EnemyMovementState
    {
        private NavMeshAgent agent;
        private Vector3 startPoint;
        private float wanderRadius;

        public EnemyWanderState(EnemyMovementStateMachine enemyMovementStateMachine) : base(enemyMovementStateMachine)
        {
            agent = enemyMovementStateMachine.Enemy.Agent;
            startPoint = enemyMovementStateMachine.Enemy.nowPosition.position;
            wanderRadius = 10f;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("¿ªÊ¼Ñ²Âß");
            StartAnimation(stateMachine.Enemy.AniamtionData.WalkParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Enemy.AniamtionData.WalkParameterHash);
        }

        public override void Update()
        {
            base.Update();

            if (HasReachedDestination())
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += startPoint;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                Vector3 finalPosition = hit.position;

                agent.SetDestination(finalPosition);
            }
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending
                && agent.remainingDistance <= agent.stoppingDistance
                && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        }
    }
}
