using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerSprintingState : SyncPlayerMovingState
    {
        public SyncPlayerSprintingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.SyncPlayer.AnimationData.SprintParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.SyncPlayer.AnimationData.SprintParameterHash);
        }
    }
}
