using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyIdlingState : EnemyMovementState
    {
        public EnemyIdlingState(EnemyMovementStateMachine enemyMovementStateMachine) : base(enemyMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Enemy.AniamtionData.IdleParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Enemy.AniamtionData.IdleParameterHash);
        }
    }
}
