using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerRollingState : SyncPlayerLandingState
    {
        public SyncPlayerRollingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.SyncPlayer.AnimationData.RollParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.SyncPlayer.AnimationData.RollParameterHash);
        }
    }
}
