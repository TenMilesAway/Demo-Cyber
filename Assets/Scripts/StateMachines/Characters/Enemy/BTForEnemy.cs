using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace Cyber
{
    public class BTForEnemy : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }

        public bool isWandering;
        public bool isIdling;

        private Vector3 startPoint;
        private float wanderRadius;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();

            ResetAllBool();
            isWandering = true;

            startPoint = transform.position;
            wanderRadius = 10f;
        }

        private void Update()
        {
            // Èç¹ûÊÇÑ²Âß×´Ì¬
            if (isWandering)
            {
                if (HasReachedDestination())
                {
                    Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                    randomDirection += startPoint;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                    Agent.SetDestination(hit.position);

                    Animator.SetBool("isWalking", true);
                }
            }
        }

        #region Change Methods
        public void ChangeStateToWander()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            isWandering = true;
        }
        #endregion

        #region Main Methods
        private void ResetAllBool()
        {
            isWandering = false;
            isIdling = false;
        }

        private void ResetAllAnimatorBool()
        {
            Animator.SetBool("isWalking", false);
            Animator.SetBool("isIdling", false);
        }

        private bool HasReachedDestination()
        {
            return !Agent.pathPending
                && Agent.remainingDistance < Agent.stoppingDistance
                && (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f);
        }
        #endregion
    }
}
