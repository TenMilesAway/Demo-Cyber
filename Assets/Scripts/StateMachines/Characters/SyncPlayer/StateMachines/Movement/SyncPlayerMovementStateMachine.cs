using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class SyncPlayerMovementStateMachine : StateMachine
    {
        public SyncPlayer SyncPlayer { get; }

        public SyncPlayerIdlingState IdlingState { get; }
        public SyncPlayerDashingState DashingState { get; }

        public SyncPlayerWalkingState WalkingState { get; }
        public SyncPlayerRunningState RunningState { get; }
        public SyncPlayerSprintingState SprintingState { get; }

        public SyncPlayerLightStoppingState LightStoppingState { get; }
        public SyncPlayerMediumStoppingState MediumStoppingState { get; }
        public SyncPlayerHardStoppingState HardStoppingState { get; }

        public SyncPlayerLightLandingState LightLandingState { get; }
        public SyncPlayerRollingState RollingState { get; }
        public SyncPlayerHardLandingState HardLandingState { get; }

        public SyncPlayerJumpingState JumpingState { get; }
        public SyncPlayerFallingState FallingState { get; }

        public SyncPlayerFlippingState FlippingState { get; }

        public SyncPlayerMovementStateMachine(SyncPlayer syncPlayer)
        {
            SyncPlayer = syncPlayer;

            IdlingState = new SyncPlayerIdlingState(this);
            DashingState = new SyncPlayerDashingState(this);

            WalkingState = new SyncPlayerWalkingState(this);
            RunningState = new SyncPlayerRunningState(this);
            SprintingState = new SyncPlayerSprintingState(this);

            LightStoppingState = new SyncPlayerLightStoppingState(this);
            MediumStoppingState = new SyncPlayerMediumStoppingState(this);
            HardStoppingState = new SyncPlayerHardStoppingState(this);

            LightLandingState = new SyncPlayerLightLandingState(this);
            RollingState = new SyncPlayerRollingState(this);
            HardLandingState = new SyncPlayerHardLandingState(this);

            JumpingState = new SyncPlayerJumpingState(this);
            FallingState = new SyncPlayerFallingState(this);

            FlippingState = new SyncPlayerFlippingState(this);
        }
    }
}
