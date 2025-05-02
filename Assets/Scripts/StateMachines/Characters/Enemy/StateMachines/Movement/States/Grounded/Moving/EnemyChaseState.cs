using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyChaseState : EnemyMovementState
    {
        public EnemyChaseState(EnemyMovementStateMachine enemyMovementStateMachine) : base(enemyMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            StartAnimation(stateMachine.Enemy.AniamtionData.RunParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            agent.ResetPath();

            StopAnimation(stateMachine.Enemy.AniamtionData.RunParameterHash);
        }

        public override void Update()
        {
            agent.SetDestination(stateMachine.Enemy.TargetPlayer.position);
        }
    }
}
