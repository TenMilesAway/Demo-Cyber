using BehaviorDesigner.Runtime;
using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyber
{
    public class BTForEnemy : MonoBehaviour
    {
        [Header("怪物信息SO")]
        [SerializeField] private EnemyData _enemyData;
        [Header("受击层级")]
        [SerializeField] private LayerMask _playerAttackLayer;
        [Header("攻击")]
        [SerializeField] private GameObject _attackCheck;
        [SerializeField] private GameObject _attackArea;

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
        private bool _isSpawner;                            // 是否由刷怪器生成
        private bool _isIdling;
        private bool _isWandering;

        private float _patrolRadius = 5f;                   // 巡逻半径
        private float _attackInterval = 2f;                 // 攻击间隔

        // 属性
        private int _currentHP = 1;
        private int _currentMP = 0;

        private Vector3[] _waypointArray = new Vector3[4];  // 路径点
        private Coroutine _animatorReactionCo;              // 受击动画协程

        public void Init(bool isGenerateBySpawner)
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            BT = GetComponent<BehaviorTree>();
            _isSpawner = isGenerateBySpawner;

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
                _animator.SetTrigger("reaction");
                if (_animatorReactionCo != null) StopCoroutine(SimualteReaction());
                _animatorReactionCo = StartCoroutine(SimualteReaction());

                // 受伤
                OnReaction(PlayerDataManager.GetInstance().GetPlayerInfo()._pAttack);
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
        /// <summary>
        /// 计算本身伤害
        /// </summary>
        public int CalculateAttack()
        {
            return _enemyData._pAttack;
        }


        #region 主要方法
        /// <summary>
        /// 受到伤害
        /// </summary>
        private void OnReaction(int attack)
        {
            _currentHP -= attack;

            ShowAttackCollider(false);
            ShowAttackArea(false);

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

            // 按照生成规则生成一个宝箱
            // 计算权重
            int totalWeight = 0;
            for (int i = 0; i < _enemyData._dropItems.Count; i++)
            {
                totalWeight += _enemyData._dropItems[i]._dropWeight;
            }

            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;
            for (int i = 0; i < _enemyData._dropItems.Count; i++)
            {
                currentWeight += _enemyData._dropItems[i]._dropWeight;

                if (currentWeight > randomWeight)
                {
                    // 生成当前配置
                    string treasurePath = HATreasureDataManager.GetInstance().GetData(_enemyData._dropItems[i]._treasureID).globalDefine;

                    UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.GetPath(treasurePath), GetInstanceID().ToString(), (GameObject treasure) =>
                    {
                        treasure.transform.rotation = this.transform.rotation;
                        treasure.transform.position = this.transform.position;

                        GameObject parent = GameObject.Find("DropItems");
                        if (parent == null) parent = new GameObject("DropItems");

                        treasure.transform.SetParent(parent.transform, false);
                    });
                    break;
                }
            }

            // 由刷怪器生成的怪物，通过 UOPF 入池
            if (_isSpawner)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.GetPath(_enemyData._prefabPath), this.gameObject);
            }
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

        public void ShowAttackCollider(bool isShow = true)
        {
            _attackCheck.SetActive(isShow);
        }

        public void ShowAttackArea(bool isShow = true)
        {
            _attackArea.SetActive(isShow);
        }
        #endregion
    }
}
