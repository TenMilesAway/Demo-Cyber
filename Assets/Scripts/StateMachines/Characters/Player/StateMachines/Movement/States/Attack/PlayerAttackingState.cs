using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyber
{
    /// <summary>
    /// 攻击状态：攻击时无法移动，但会自带位移突进，写进 SO
    /// 连击：共有 4 段连击，每段攻击动画帧判断结束后的0.5s内判断是否有左键输入
    /// 重击：长按一段时候后释放，长按时应该进入蓄力动画，松开后触发
    /// 结束后收纳动作：0.5s判断结束以后（有僵直？），执行收纳动作，结束后过渡到 Idling
    /// </summary>
    public class PlayerAttackingState : PlayerMovementState
    {
        protected List<AttackSO> combo;

        protected int comboCounter = 0;

        protected float lastComboEnd;
        protected float lastClickedTime;

        protected bool hasExit = false;

        protected Coroutine EndComboCo;

        public PlayerAttackingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            lastComboEnd = 0;
        }

        public override void Enter()
        {
            stateMachine.ReusableData.MovementSpeedModifier = stateMachine.Player.Data.AttackData.SpeedModified;

            base.Enter();

            combo = stateMachine.Player.combo;

            ResetVelocity();

            StartAnimation(stateMachine.Player.AnimationData.AttackParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(stateMachine.Player.AnimationData.AttackParameterHash);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void PhysicsUpdate()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifier == 0f)
                ResetVelocity();

            base.PhysicsUpdate();

            Float();
        }

        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
        }

        public override void OnTriggerEnter(Collider collider)
        {
            base.OnTriggerEnter(collider);

            if (stateMachine.Player.LayerData.IsEnemyLayer(collider.gameObject.layer))
            {
                // 攻击逻辑
                Debug.LogWarning("击中敌人");

                MonoMgr.GetInstance().StartCoroutine(SimulateHitfeel());

                // 添加监听
            }
        }

        public override void OnAnimationEnterEvent()
        {
            stateMachine.Player.ResizableCapsuleCollider.TriggerColliderData.ShowAttackCheckCollider();
        }

        public override void OnAnimationExitEvent()
        {
            stateMachine.Player.ResizableCapsuleCollider.TriggerColliderData.HideAttackCheckCollider();
        }


        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, stateMachine.Player.ResizableCapsuleCollider.SlopeData.FloatRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.ColliderCenterInLocalSpace.y * stateMachine.Player.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * stateMachine.Player.ResizableCapsuleCollider.SlopeData.StepReachForce - GetPlayerVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = groundedData.SlopeSpeedAngles.Evaluate(angle);

            if (stateMachine.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

                UpdateCameraRecenteringState(stateMachine.ReusableData.MovementInput);
            }

            return slopeSpeedModifier;
        }

        protected void ExitAttack()
        {
            if (stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                // 传递给 MonoMgr 的协程函数只能是 public
                EndComboCo = MonoMgr.GetInstance().StartCoroutine(EndCombo());
        }

        public IEnumerator EndCombo()
        {
            hasExit = true;

            yield return new WaitForSeconds(stateMachine.Player.Data.AttackData.ComboWaitTime);

            comboCounter = 0;

            stateMachine.ChangeState(stateMachine.IdlingState);

            hasExit = false;
        }

        public bool IsComboTime()
        {
            return Time.time - lastComboEnd > stateMachine.Player.Data.AttackData.ComboWaitTime;
        }

        public IEnumerator SimulateHitfeel()
        {
            stateMachine.Player.Animator.speed = 0.01f;

            CameraController.GetInstance().SwitchToShake(0.5f);

            yield return new WaitForSeconds(0.1f);

            CameraController.GetInstance().ReturnToNoShake();

            stateMachine.Player.Animator.speed = 1f;
        }
        #endregion
    }
}
