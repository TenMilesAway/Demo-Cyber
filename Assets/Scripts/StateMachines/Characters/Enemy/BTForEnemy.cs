using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using HA;

namespace Cyber
{
    public class BTForEnemy : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgent Agent { get; private set; }
        [HideInInspector] public Animator Animator { get; private set; }
        [HideInInspector] public BehaviorTree BT { get; private set; }
        [field: SerializeField] public LayerMask PlayerAttackLayer { get; private set; }

        public GameObject _waypoints;
        public GameObject _waypoint1;
        public GameObject _waypoint2;
        public GameObject _waypoint3;
        public GameObject _waypoint4;

        private bool _isIdling;
        private bool _isWandering;
        private bool _isInit;

        private float _patrolRadius = 5f;
        private float _attackInterval = 2f;

        private Vector3[] _waypointArray = new Vector3[4];
        private Coroutine _animatorCo;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();
            BT = GetComponent<BehaviorTree>();

            InitVariables();
            GameManager.Event.AddListener(GameEventType.UpdateEntityInfoAfterSpawn, GenerateWaypoints);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            // 如果是巡逻状态
            if (_isWandering)
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
            _isWandering = true;
        }

        public void ChangeStateToIdle()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            _isIdling = true;
        }

        public void ChangeStateToAttack()
        {
            Animator.SetTrigger("attack");
        }
        #endregion

        #region 主要方法
        private void ResetAllBool()
        {
            _isWandering = false;
            _isIdling = false;
        }

        private void ResetAllAnimatorBool()
        {
            Animator.SetBool("isWalking", false);
            Animator.SetBool("isIdling", false);
        }

        private void InitVariables()
        {
            BT.GetVariable("_attackInterval").SetValue(_attackInterval);
        }

        /// <summary>
        /// 在范围内随机生成 4 个路径点
        /// </summary>
        private void GenerateWaypoints(float radius)
        {
            Vector3 randomDirection;
            for (int i = 0; i < _waypointArray.Length; i++)
            {
                randomDirection = Random.insideUnitSphere * radius;
                randomDirection += transform.position;
                _waypointArray[i] = randomDirection;
            }

            for (int i = 0; i < _waypointArray.Length; i++)
            {
                BT.SetVariableValue("_waypoint" + (i + 1), _waypointArray[i]);
            }

            _waypoint1.transform.position = (Vector3)BT.GetVariable("_waypoint1").GetValue();
            _waypoint2.transform.position = (Vector3)BT.GetVariable("_waypoint2").GetValue();
            _waypoint3.transform.position = (Vector3)BT.GetVariable("_waypoint3").GetValue();
            _waypoint4.transform.position = (Vector3)BT.GetVariable("_waypoint4").GetValue();

            GameObject parent = GameObject.Find("Waypoints");
            if (parent == null) parent = new GameObject("Waypoints");

            _waypoints.transform.SetParent(parent.transform, true);
        }
        #endregion

        #region 辅助方法
        private void GenerateWaypoints()
        {
            if (_isInit) return;

            HADebug.Log("开始初始化路径点");
            _isInit = true;
            GenerateWaypoints(_patrolRadius);
        }

        private IEnumerator SimualteReaction()
        {
            Animator.speed = 0.01f;

            yield return new WaitForSeconds(0.1f);

            Animator.speed = 1f;

            _animatorCo = null;
        }

        public void RemoveAllListeners()
        {
            GameManager.Event.RemoveListener(GameEventType.UpdateEntityInfoAfterSpawn, GenerateWaypoints);
        }
        #endregion
    }
}
