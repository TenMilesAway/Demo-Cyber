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
        [Header("怪物信息SO")]
        [SerializeField] private EnemyData _enemyData;
        [Header("受击层级")]
        [SerializeField] private LayerMask _playerAttackLayer;

        [Header("AI 路径点")]
        [SerializeField] private GameObject _waypoints;
        [SerializeField] private GameObject _waypoint1;
        [SerializeField] private GameObject _waypoint2;
        [SerializeField] private GameObject _waypoint3;
        [SerializeField] private GameObject _waypoint4;

        private NavMeshAgent _agent;                        // AI Agent
        private Animator _animator;                         // 动画状态机
        private BehaviorTree BT;                            // 行为树
                                                            
        private bool _isInit;                               // 是否已经被初始化过 (路径点)
        private bool _isIdling;
        private bool _isWandering;

        private float _patrolRadius = 5f;                   // 巡逻半径
        private float _attackInterval = 2f;                 // 攻击间隔

        // 属性
        private int _currentHP = 1;
        private int _currentMP = 0;

        private Vector3[] _waypointArray = new Vector3[4];  // 路径点
        private Coroutine _animatorReactionCo;              // 受击动画协程

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            BT = GetComponent<BehaviorTree>();

            InitVariables();

            _currentHP = _enemyData._maxHP;
            _currentMP = _enemyData._maxMP;

            AddListeners();
        }

        private void Update()
        {
            BT.GetVariable("_velocity").SetValue(_agent.velocity.magnitude);
        }

        private void OnTriggerEnter(Collider collider)
        {
            // 如果碰到的物体中有 PlayerAttack 层
            int layer = collider.gameObject.layer;
            if ((1 << layer & _playerAttackLayer) != 0)
            {
                // 受伤
                OnReaction(PlayerDataManager.GetInstance().GetPlayerInfo()._pAttack);

                _animator.SetTrigger("reaction");
                if (_animatorReactionCo != null) StopCoroutine(SimualteReaction());
                _animatorReactionCo = StartCoroutine(SimualteReaction());
            }
        }

        #region 状态变化方法
        /// <summary>
        /// 攻击
        /// </summary>
        public void ChangeStateToAttack()
        {
            _animator.SetTrigger("attack");
        }

        public void ChangeStateToWander()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            _animator.SetBool("isWalking", true);
            _isWandering = true;
        }

        public void ChangeStateToIdle()
        {
            ResetAllBool();
            ResetAllAnimatorBool();

            _isIdling = true;
        }
        #endregion

        #region 主要方法
        private void OnReaction(int attack)
        {
            _currentHP -= attack;

            HADebug.LogFormat("怪物 {0} 受到伤害 {1}, 当前剩余血量 {2}", gameObject.name, attack, _currentHP);

            if (_currentHP <= 0)
            {
                OnDeath();
            }
        }

        /// <summary>
        /// 死亡触发事件
        /// </summary>
        private void OnDeath()
        {
            PlayerDataManager.GetInstance().SendEXPToPlayer(_enemyData._EXP);
            Destroy(this.gameObject);
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        private void InitVariables()
        {
            BT.GetVariable("_attackInterval").SetValue(_attackInterval);
        }

        private void ResetAllBool()
        {
            _isWandering = false;
            _isIdling = false;
        }

        private void ResetAllAnimatorBool()
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isIdling", false);
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
        private void AddListeners()
        {
            GameManager.Event.AddListener(GameEventType.UpdateEntityInfoAfterSpawn, GenerateWaypoints);
        }

        public void RemoveAllListeners()
        {
            GameManager.Event.RemoveListener(GameEventType.UpdateEntityInfoAfterSpawn, GenerateWaypoints);
        }

        private IEnumerator SimualteReaction()
        {
            _animator.speed = 0.01f;

            yield return new WaitForSeconds(0.1f);

            _animator.speed = 1f;

            _animatorReactionCo = null;
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 初始化路径点
        /// </summary>
        private void GenerateWaypoints()
        {
            if (_isInit) return;

            HADebug.Log("开始初始化路径点");
            _isInit = true;
            GenerateWaypoints(_patrolRadius);
        }
        #endregion
    }
}
