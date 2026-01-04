using Cyber;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class OneTimeSpawner : MonoBehaviour, ISpawner
    {
        [Header("基础信息")]
        [SerializeField] private ESpawnType _spawnType = ESpawnType.Enemy;
        [SerializeField] private List<SpawnerData> _spawnerDatas = new List<SpawnerData>();
        [SerializeField] private bool _spawnOnStart;

        [Header("生成范围")]
        [SerializeField] private ESpawnAreaType _spawnAreaType = ESpawnAreaType.Circle;
        [SerializeField] private float _spawnRadius = 10f;

        [Header("生成数据")]
        [SerializeField] private SpawnNumRange _spawnNumRange = new SpawnNumRange(1, 10);
        [SerializeField] private SpawnScaleRange _spawnScaleRange = new SpawnScaleRange(0.8f, 1.2f);

        [Header("生成模式")]
        private ESpawnMode _spawnMode = ESpawnMode.OneTime;

        [Header("调试")]
        [SerializeField] private Color _gizmoColor = new Color(0, 1, 0, 0.3f);
        [SerializeField] private bool _showGizmos = false;

        [Header("贴地生成")]
        [SerializeField] private bool _alignToGround = true;
        [SerializeField] private float _groundCheckDistance = 10f;
        [SerializeField] private LayerMask _groundLayer;

        private List<GameObject> _spawnedObjects = new List<GameObject>();
        private int _totalWeight = 0;
        private int _seed;

        #region 属性
        public List<SpawnerData> SpawnablePrefabs { get => _spawnerDatas; }
        public bool SpawnOnStart { get => _spawnOnStart; }
        public ESpawnAreaType SpawnAreaType { get => _spawnAreaType; }
        public float SpawnRadius { get => _spawnRadius; }
        public SpawnNumRange SpawnNumRange { get => _spawnNumRange; }
        public SpawnScaleRange SpawnScaleRange { get => _spawnScaleRange; }
        public ESpawnMode SpawnMode { get => _spawnMode; }
        public Color GizmoColor { get => _gizmoColor; }
        public bool ShowGizmos { get => _showGizmos; }
        public bool AlignToGround { get => _alignToGround; }
        public float GroundCheckDistance { get => _groundCheckDistance; }
        public LayerMask GoundLayer { get => _groundLayer; }
        #endregion

        private void Start()
        {
            CalculateTotalWeight();

            if (_spawnOnStart)
            {
                SpawnPrefabs();
            }
        }

        /// <summary>
        /// 计算权重
        /// </summary>
        public void CalculateTotalWeight()
        {
            _totalWeight = 0;
            foreach (SpawnerData data in _spawnerDatas)
            {
                // 初始化路径
                data._prefabPath = InitSpawnerDataPath(data);

                // 不计算 path 为空的数据
                if (data._prefabPath != null)
                {
                    _totalWeight += data._weight;
                }
            }
        }

        /// <summary>
        /// 刷怪
        /// </summary>
        public void SpawnPrefabs()
        {
            int count = _spawnNumRange.GetRandomFromRange();
            SpawnPrefabs(count);
        }

        /// <summary>
        /// 生成指定数量的 prefab
        /// </summary>
        public void SpawnPrefabs(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnerData data = GetRandomSpawnerData();

                if (data == null)
                {
                    HADebug.LogErrorFormat("刷怪点 [{0},{1},{2}] 无可生成的预制体或权重总和为 0", transform.position.x, transform.position.y, transform.position.z);
                    continue;
                }

                //if (_alignToGround) spawnPosition = AdjustPositionToGround(spawnPosition);

                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.GetPath(data._prefabPath), GetInstanceID().ToString(), (GameObject entity) =>
                {
                    InitSpawnerInfo(entity, data);

                    if (data._randomRotation) entity.transform.rotation = UnityEngine.Random.rotation;
                    if (data._randomScale) entity.transform.localScale = Vector3.one * _spawnScaleRange.GetRandomFromRange();
                    _spawnedObjects.Add(entity);

                    entity.transform.SetParent(transform, true);
                    entity.transform.position = GetRandomSpawnPosition();
                });
            }
        }

        #region 辅助方法
        /// <summary>
        /// 初始化刷怪路径
        /// </summary>
        private string InitSpawnerDataPath(SpawnerData data)
        {
            string path = null;

            switch (_spawnType)
            {
                case ESpawnType.Enemy:
                    {
                        path = EnemyDataManager.GetInstance().GetData(data._prefabID).globalDefine;
                    }
                    break;
                case ESpawnType.Treasure:
                    {
                        path = HATreasureDataManager.GetInstance().GetData(data._prefabID).globalDefine;
                    }
                    break;
            }

            return path;
        }

        /// <summary>
        /// 初始化刷怪信息
        /// </summary>
        private void InitSpawnerInfo(GameObject entity, SpawnerData data)
        {
            switch (_spawnType)
            {
                case ESpawnType.Enemy:
                    {
                        // 敌人的初始化
                        BTForEnemy enemy = entity.GetComponent<BTForEnemy>();
                        if (enemy != null) enemy.Init(true);
                    }
                    break;
                case ESpawnType.Treasure:
                    {
                        // 宝藏的初始化
                        HATreasure treasure = entity.GetComponent<HATreasure>();
                        if (treasure != null)
                        {
                            TBTreasureData treasureData = HATreasureDataManager.GetInstance().GetData(data._prefabID);
                            treasure.InitTreasureName(treasureData.id, treasureData.name);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 随机获得一个预制体路径
        /// </summary>
        private SpawnerData GetRandomSpawnerData()
        {
            if (_spawnerDatas.Count == 0 || _totalWeight == 0) return null;

            int randomWeight = UnityEngine.Random.Range(0, _totalWeight);
            int currentWeight = 0;

            foreach (SpawnerData data in _spawnerDatas)
            {
                // path 为空的数据排除
                if (data._prefabPath == null) continue;

                currentWeight += data._weight;

                if (randomWeight < currentWeight) return data;
            }

            return null;
        }

        /// <summary>
        /// 随机一个生成位置
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 center = transform.position;

            switch (_spawnAreaType)
            {
                case ESpawnAreaType.Circle:
                    {
                        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _spawnRadius;
                        return center + new Vector3(randomCircle.x, 0, randomCircle.y);
                    }
                case ESpawnAreaType.Rectangle:
                    {
                        float x = UnityEngine.Random.Range(-_spawnRadius / 2, _spawnRadius / 2);
                        float z = UnityEngine.Random.Range(-_spawnRadius / 2, _spawnRadius / 2);
                        return center + new Vector3(x, 0, z);
                    }
                case ESpawnAreaType.Sphere:
                    {
                        return center + UnityEngine.Random.insideUnitSphere * _spawnRadius;
                    }
                case ESpawnAreaType.Box:
                    {
                        float x = UnityEngine.Random.Range(-_spawnRadius / 2, _spawnRadius / 2);
                        float y = UnityEngine.Random.Range(-_spawnRadius / 2, _spawnRadius / 2);
                        float z = UnityEngine.Random.Range(-_spawnRadius / 2, _spawnRadius / 2);
                        return center + new Vector3(x, y, z);
                    }
                default:
                    return center;
            }
        }

        /// <summary>
        /// 调整位置至地面 (没有实用)
        /// </summary>
        private Vector3 AdjustPositionToGround(Vector3 position)
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, _groundCheckDistance, _groundLayer))
            {
                return hit.point;
            }
            return position;
        }
        #endregion

        #region 编辑器拓展
        private void OnDrawGizmosSelected()
        {
            if (!_showGizmos) return;
            Gizmos.color = _gizmoColor;

            switch (_spawnAreaType)
            {
                case ESpawnAreaType.Circle:
                    {
                        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
                        Gizmos.DrawSphere(transform.position, 0.5f);
                    }
                    break;
                case ESpawnAreaType.Rectangle:
                    {
                        Gizmos.DrawWireCube(transform.position, new Vector3(_spawnRadius, 0.1f, _spawnRadius));
                        Gizmos.DrawSphere(transform.position, 0.5f);
                    }
                    break;
                case ESpawnAreaType.Sphere:
                    {
                        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
                        Gizmos.DrawSphere(transform.position, 0.5f);
                    }
                    break;
                case ESpawnAreaType.Box:
                    {
                        Gizmos.DrawWireCube(transform.position, new Vector3(_spawnRadius, _spawnRadius, _spawnRadius));
                        Gizmos.DrawSphere(transform.position, 0.5f);
                    }
                    break;
            }
        }
        #endregion
    }
}
