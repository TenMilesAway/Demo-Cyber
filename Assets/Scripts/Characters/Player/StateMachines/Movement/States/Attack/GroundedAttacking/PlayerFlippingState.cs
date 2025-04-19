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

            if (!IsComboTime())
                return;

            if (EndComboCo != null)
            {
                MonoMgr.GetInstance().StopCoroutine(EndComboCo);

                hasExit = false;
            }

            // 第一次攻击时，开始连击，这里应该去判断连击间隔
            if (comboCounter == 0)
            {
                stateMachine.Player.Animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                
                StartAnimation(stateMachine.Player.AnimationData.AttackReadyParameterHash);

                stateMachine.Player.Animator.Play("Flip", 0, 0);

                lastClickedTime = Time.time;

                comboCounter++;

                return;
            }

            // 我可不可以缓存一下鼠标的点击次数，这样就不用判断间隔时间了，后续优化
            //if (Time.time - lastClickedTime >= stateMachine.Player.Data.AttackData.ComboDeterTime)
            if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f
                || stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("ReadyToExitAttack"))
            {
                stateMachine.Player.Animator.runtimeAnimatorController = combo[comboCounter].animatorOV;

                StartAnimation(stateMachine.Player.AnimationData.AttackReadyParameterHash);

                stateMachine.Player.Animator.Play("Flip", 0, 0);

                lastClickedTime = Time.time;

                comboCounter++;
            }

            comboCounter = comboCounter % combo.Count;

            if (comboCounter == 0)
                lastComboEnd = Time.time;
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
    }
}
