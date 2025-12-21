using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

namespace Cyber
{
    public class BTForEnemy : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgent Agent { get; private set; }
        [HideInInspector] public Animator Animator { get; private set; }
        [HideInInspector] public BehaviorTree BT { get; private set; }
        [field: SerializeField] public LayerMask PlayerAttackLayer { get; private set; }

        public GameObject waypoints;
        public GameObject waypoint1;
        public GameObject waypoint2;
        public GameObject waypoint3;
        public GameObject waypoint4;

        private bool isIdling;
        private bool isWandering;

        private float patrolRadius = 5f;
        private float attackInterval = 2f;

        private Vector3[] waypointArray = new Vector3[4];
        private Coroutine _animatorCo;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();
            BT = GetComponent<BehaviorTree>();

            InitVariables();
            GenerateWaypoints(patrolRadius);
        }

        private void Update()
        {
            // 如果是巡逻状态
            if (isWandering)
            {

            }

            BT.GetVariable("_velocity").SetValue(Agent.velocity.magnitude);

            if (BT.GetVariable("_player").GetValue() != null)
            {
                Debug.Log("看到目标了");
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            // 如果碰到的物体中有 PlayerAttack 层
            int layer = collider.gameObject.layer;
            if ((1 << layer & PlayerAttackLayer) != 0)
            {
                Animator.SetTrigger("reaction");
                if (_animatorCo != null) StopCoroutine(SimualteReaction());
                _animatorCo = StartCoroutine(SimualteReaction());
            }
        }

        #region 状态变化方法
        public void ChangeStateToWander()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            Animator.SetBool("isWalking", true);
            isWandering = true;
        }

        public void ChangeStateToIdle()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            isIdling = true;
        }

        public void ChangeStateToAttack()
        {
            Animator.SetTrigger("attack");
        }
        #endregion

        #region 主要方法
        private void ResetAllBool()
        {
            isWandering = false;
            isIdling = false;
        }

        private void ResetAllAnimatorBool()
        {
            Animator.SetBool("isWalking", false);
            Animator.SetBool("isIdling", false);
        }

        private void InitVariables()
        {
            BT.GetVariable("_attackInterval").SetValue(attackInterval);
        }

        /// <summary>
        /// 在范围内随机生成 4 个路径点
        /// </summary>
        private void GenerateWaypoints(float radius)
        {
            Vector3 randomDirection;
            for (int i = 0; i < waypointArray.Length; i++)
            {
                randomDirection = Random.insideUnitSphere * radius;
                randomDirection += transform.position;
                waypointArray[i] = randomDirection;
            }

            for (int i = 0; i < waypointArray.Length; i++)
            {
                BT.SetVariableValue("_waypoint" + (i + 1), waypointArray[i]);
            }

            waypoint1.transform.position = (Vector3)BT.GetVariable("_waypoint1").GetValue();
            waypoint2.transform.position = (Vector3)BT.GetVariable("_waypoint2").GetValue();
            waypoint3.transform.position = (Vector3)BT.GetVariable("_waypoint3").GetValue();
            waypoint4.transform.position = (Vector3)BT.GetVariable("_waypoint4").GetValue();

            waypoints.transform.SetParent(null);
        }
        #endregion

        #region 辅助方法
        private IEnumerator SimualteReaction()
        {
            Animator.speed = 0.01f;

            yield return new WaitForSeconds(0.1f);

            Animator.speed = 1f;

            _animatorCo = null;
        }
        #endregion
    }
}
