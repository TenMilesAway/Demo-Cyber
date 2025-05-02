using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyber
{
    public class EnemyMovementState : IState
    {
        protected NavMeshAgent agent;
        protected EnemyMovementStateMachine stateMachine;
        protected Transform targetPlayer;

        public EnemyMovementState(EnemyMovementStateMachine enemyMovementStateMachine)
        {
            stateMachine = enemyMovementStateMachine;
            agent = enemyMovementStateMachine.Enemy.Agent;
        }

        public virtual void Enter()
        {
            
        }

        public virtual void Exit()
        {
            
        }

        public virtual void Update()
        {

        }

        public virtual void PhysicsUpdate()
        {
            
        }

        public virtual void HandleInput()
        {
            
        }

        public virtual void OnAnimationEnterEvent()
        {
            
        }

        public virtual void OnAnimationExitEvent()
        {
            
        }

        public virtual void OnAnimationTransitionEvent()
        {
            
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            
        }

        public virtual void OnTriggerStay(Collider collider)
        {
            
        }

        protected void StartAnimation(int animationHash)
        {
            stateMachine.Enemy.Animator.SetBool(animationHash, true);
        }

        protected void StopAnimation(int animationHash)
        {
            stateMachine.Enemy.Animator.SetBool(animationHash, false);
        }
    }
}
