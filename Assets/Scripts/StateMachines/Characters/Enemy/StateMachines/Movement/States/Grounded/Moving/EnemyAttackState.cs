using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyAttackState : EnemyMovementState
    {
        public EnemyAttackState(EnemyMovementStateMachine enemyMovementStateMachine) : base(enemyMovementStateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.Enemy.transform.LookAt(targetPlayer);

            agent.velocity = Vector3.zero;
            agent.acceleration = 0;

            StartAnimation(stateMachine.Enemy.AniamtionData.AttackParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            agent.acceleration = 5;
            stateMachine.Enemy.BT.GetVariable("TargetPlayer").SetValue(null);

            StopAnimation(stateMachine.Enemy.AniamtionData.AttackParameterHash);
        }

        public override void OnAnimationEnterEvent()
        {
            
        }

        public override void OnAnimationExitEvent()
        {
            
        }
    }
}
