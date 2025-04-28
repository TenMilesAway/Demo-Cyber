using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerJumpingState : SyncPlayerAirborneState
    {
        public SyncPlayerJumpingState(SyncPlayerMovementStateMachine syncPlayerMovementStateMachine) : base(syncPlayerMovementStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
