using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyMovementState : IState
    {
        protected EnemyMovementStateMachine stateMachine;

        public EnemyMovementState(EnemyMovementStateMachine enemyMovementStateMachine)
        {
            stateMachine = enemyMovementStateMachine;
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

        public void HandleInput()
        {
            throw new System.NotImplementedException();
        }

        public void OnAnimationEnterEvent()
        {
            throw new System.NotImplementedException();
        }

        public void OnAnimationExitEvent()
        {
            throw new System.NotImplementedException();
        }

        public void OnAnimationTransitionEvent()
        {
            throw new System.NotImplementedException();
        }

        public void OnTriggerEnter(Collider collider)
        {
            throw new System.NotImplementedException();
        }

        public void OnTriggerExit(Collider collider)
        {
            throw new System.NotImplementedException();
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
