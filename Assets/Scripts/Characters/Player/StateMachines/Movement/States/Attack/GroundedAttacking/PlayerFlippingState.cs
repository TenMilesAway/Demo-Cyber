using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyber
{
    public class PlayerFlippingState : PlayerAttackingState
    {
        public PlayerFlippingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();

            comboCounter++;

            if (EndComboCo != null)
            {
                MonoMgr.GetInstance().StopCoroutine(EndComboCo);
                hasExit = false;
            }

            //if (!IsComboTime())
            //    return;

            // 第一次攻击时，开始连击
            if (comboCounter == 1)
            {
                stateMachine.Player.Animator.runtimeAnimatorController = combo[comboCounter - 1].animatorOV;
                stateMachine.Player.Animator.Play("Flip", 0, 0);
                StartAnimation(stateMachine.Player.AnimationData.AttackReadyParameterHash);
                lastClickedTime = Time.time;
                Debug.LogWarning(comboCounter);
                return;
            }

            // 我可不可以缓存一下鼠标的点击次数，这样就不用判断间隔时间了，后续优化
            if (Time.time - lastClickedTime >= stateMachine.Player.Data.AttackData.ComboDeterTime)
            {
                stateMachine.Player.Animator.runtimeAnimatorController = combo[comboCounter - 1].animatorOV;
                stateMachine.Player.Animator.Play("Flip", 0, 0);
                StartAnimation(stateMachine.Player.AnimationData.AttackReadyParameterHash);
                lastClickedTime = Time.time;
            }
            comboCounter = comboCounter % combo.Count;

            Debug.LogWarning(comboCounter);
         }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.AttackReadyParameterHash);
        }

        public override void Update()
        {
            base.Update();

            if (!hasExit)
                ExitAttack();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
