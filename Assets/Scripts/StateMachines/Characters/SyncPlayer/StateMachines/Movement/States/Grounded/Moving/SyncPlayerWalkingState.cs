using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerWalkingState : SyncPlayerMovingState
    {
        public SyncPlayerWalkingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.SyncPlayer.AnimationData.WalkParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.SyncPlayer.AnimationData.WalkParameterHash);
        }
    }
}
