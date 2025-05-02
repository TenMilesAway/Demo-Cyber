using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Cyber
{
    public class Enemy : MonoBehaviour
    {
        public Animator Animator { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public BehaviorTree BT { get; private set; }
        public Transform TargetPlayer { get; private set; }
        [field: SerializeField] public Transform NowPosition { get; private set; }
        [field: SerializeField] public EnemyAniamtionData AniamtionData { get; private set; }

        private EnemyMovementStateMachine movementStateMachine;

        [Header(" Ù–‘")]
        public float hp = 200;
        public float defense = 20;
        public float attack = 50;

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            Agent = GetComponent<NavMeshAgent>();
            BT = GetComponent<BehaviorTree>();

            AniamtionData.Initialize();

            movementStateMachine = new EnemyMovementStateMachine(this);
            Debug.Log(movementStateMachine);
        }

        private void Update()
        {
            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }

        public void OnMovementStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        public void OnMovementStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }

        #region State Change Methods
        public void OnWanderState()
        {
            movementStateMachine.ChangeState(movementStateMachine.WanderState);
        }

        public void OnIdleState()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdleState);
        }

        public void OnChaseState()
        {
            TargetPlayer = BT.GetVariable("TargetPlayer").GetValue() as Transform;

            movementStateMachine.ChangeState(movementStateMachine.ChaseState);
        }

        public void OnAttackState()
        {
            movementStateMachine.ChangeState(movementStateMachine.AttackState);
        }

        public void OnLockState()
        {
            movementStateMachine.ChangeState(movementStateMachine.LockState);
        }
        #endregion
    }

    #region BT Methods
    public class EnemyDetectPlayerConditional : Conditional
    {
        public BehaviorTree BT;

        public override void OnStart()
        {
            BT = this.GetComponent<BehaviorTree>();
        }

        public override TaskStatus OnUpdate()
        {
            if (IsThereAnyPlayer())
            {
                return TaskStatus.Failure;
            }    

            return TaskStatus.Success;
        }

        public bool IsThereAnyPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, 1 << LayerMask.NameToLayer("Player"), QueryTriggerInteraction.Collide);

            if (colliders.Length == 0)
            {
                return false;
            }

            BT.GetVariable("TargetPlayer").SetValue(colliders[0].gameObject.transform);
            return true;
        }
    }

    public class EnemyAttackConditional : Conditional
    {
        public BehaviorTree BT;
        public Enemy enemy;
        public Transform targetPlayer;

        public override void OnStart()
        {
            BT = GetComponent<BehaviorTree>();
            enemy = GetComponent<Enemy>();
            targetPlayer = enemy.TargetPlayer;
        }

        public override TaskStatus OnUpdate()
        {
            if (canAttack())
                return TaskStatus.Failure;

            return TaskStatus.Success;
        }

        public bool canAttack()
        {
            if (Vector2.Distance(targetPlayer.position, transform.position) < 2f && !enemy.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                return true;
            }

            return false;
        }
    }
    public class EnemyLockConditional : Conditional
    {
        public BehaviorTree BT;
        public Enemy enemy;
        public Transform targetPlayer;

        private float angle;
        private Vector3 lhs;
        private Vector3 rhs;

        public override void OnStart()
        {
            BT = GetComponent<BehaviorTree>();
            enemy = GetComponent<Enemy>();
            targetPlayer = enemy.TargetPlayer;
        }

        public override TaskStatus OnUpdate()
        {
            if (isLock())
                return TaskStatus.Failure;

            return TaskStatus.Success;
        }

        public bool isLock()
        {
            lhs = enemy.transform.forward;
            rhs = targetPlayer.position - enemy.transform.position;
            angle = Mathf.Acos(Vector3.Dot(lhs, rhs) / (lhs.magnitude * rhs.magnitude)) * Mathf.Rad2Deg;
            Debug.Log(angle);

            return angle < 60f ? false : true;
        }
    }
    #endregion
}
