using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyMovementStateMachine : StateMachine
    {
        public Enemy Enemy { get; }

        public EnemyWanderState WanderState { get; }

        public EnemyMovementStateMachine(Enemy enemy)
        {
            Enemy = enemy;

            WanderState = new EnemyWanderState(this);
        }
    }
}
