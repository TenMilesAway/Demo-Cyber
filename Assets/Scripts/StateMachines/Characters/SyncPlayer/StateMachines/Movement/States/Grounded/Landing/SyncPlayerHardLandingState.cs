using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerHardLandingState : SyncPlayerLandingState
    {
        public SyncPlayerHardLandingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.SyncPlayer.AnimationData.HardLandParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.SyncPlayer.AnimationData.HardLandParameterHash);
        }
    }
}
