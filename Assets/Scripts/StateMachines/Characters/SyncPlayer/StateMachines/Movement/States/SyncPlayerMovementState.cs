using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerMovementState : IState
    {
        protected SyncPlayerMovementStateMachine stateMachine;

        public SyncPlayerMovementState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine)
        {
            stateMachine = syncPlayerMovementStateMachine;
        }

        public virtual void Enter()
        {
            Debug.Log(GetType().Name);
        }

        public virtual void Exit()
        {
            
        }

        public virtual void HandleInput()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void PhysicsUpdate()
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
            stateMachine.SyncPlayer.Animator.SetBool(animationHash, true);
        }

        protected void StopAnimation(int animationHash)
        {
            stateMachine.SyncPlayer.Animator.SetBool(animationHash, false);
        }
    }
}
