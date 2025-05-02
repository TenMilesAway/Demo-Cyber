using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyLockState : EnemyMovementState
    {
        private float angle;

        public EnemyLockState(EnemyMovementStateMachine enemyMovementStateMachine) : base(enemyMovementStateMachine)
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

        public override void Update()
        {
            base.Update();

            angle = Vector3.Angle(stateMachine.Enemy.transform.position, stateMachine.Enemy.TargetPlayer.position);

            if (angle > 60f)
            {
                stateMachine.Enemy.transform.LookAt(stateMachine.Enemy.TargetPlayer);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }
}
