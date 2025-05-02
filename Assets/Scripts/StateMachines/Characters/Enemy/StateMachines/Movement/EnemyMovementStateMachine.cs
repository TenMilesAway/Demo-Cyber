using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyMovementStateMachine : StateMachine
    {
        public Enemy Enemy { get; }

        public EnemyIdlingState IdleState { get; }

        public EnemyWanderState WanderState { get; }
        public EnemyChaseState ChaseState { get; }
        public EnemyAttackState AttackState { get; }
        public EnemyLockState LockState { get; }

        public EnemyMovementStateMachine(Enemy enemy)
        {
            Enemy = enemy;

            IdleState = new EnemyIdlingState(this);

            WanderState = new EnemyWanderState(this);
            ChaseState = new EnemyChaseState(this);
            AttackState = new EnemyAttackState(this);
            LockState = new EnemyLockState(this);
        }
    }
}
