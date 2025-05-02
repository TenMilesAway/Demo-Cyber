using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class PlayerReactionState : PlayerGroundedState
    {
        public PlayerReactionState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.ReusableData.MovementSpeedModifier = stateMachine.Player.Data.GroundedData.ReactionData.SpeedModifier;

            StartAnimation(stateMachine.Player.AnimationData.ReactionParameterHash, true);

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void OnAnimationExitEvent()
        {
            base.OnAnimationExitEvent();

            stateMachine.ChangeState(stateMachine.IdlingState);
        }

        public override void PhysicsUpdate()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
                ResetVelocity();

            base.PhysicsUpdate();
        }
    }
}
