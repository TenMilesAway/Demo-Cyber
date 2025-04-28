using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerHardStoppingState : SyncPlayerStoppingState
    {
        public SyncPlayerHardStoppingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.SyncPlayer.AnimationData.HardStopParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.SyncPlayer.AnimationData.HardStopParameterHash);
        }
    }
}
