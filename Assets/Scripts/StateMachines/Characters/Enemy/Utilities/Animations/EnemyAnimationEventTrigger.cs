using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class EnemyAnimationEventTrigger : MonoBehaviour
    {
        private Enemy enemy;

        private void Awake()
        {
            enemy = transform.parent.GetComponent<Enemy>();
        }

        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            enemy.OnMovementStateAnimationEnterEvent();
        }

        public void TriggerOnMovementStateAnimationExitEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            enemy.OnMovementStateAnimationExitEvent();
        }

        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            enemy.OnMovementStateAnimationTransitionEvent();
        }

        private bool IsInAnimationTransition(int layerIndex = 0)
        {
            return enemy.Animator.IsInTransition(layerIndex);
        }
    }
}
