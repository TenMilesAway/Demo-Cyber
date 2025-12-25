using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ContinousSpawner : MonoBehaviour, ISpawner
    {
        [Header("基础信息")]
        [SerializeField] private List<SpawnerData> _spawnablePrefabs = new List<SpawnerData>();
        [SerializeField] private bool _spawnOnStart;

        [Header("生成范围")]
        [SerializeField] private ESpawnAreaType _spawnAreaType = ESpawnAreaType.Circle;
        [SerializeField] private float _spawnRadius = 10f;

        [Header("生成数据")]
        [SerializeField] private SpawnNumRange _spawnNumRange = new SpawnNumRange(1, 1);
        [SerializeField] private SpawnScaleRange _spawnScaleRange = new SpawnScaleRange(0.8f, 1.2f);

        [Header("生成模式")]
        private ESpawnMode _spawnMode = ESpawnMode.Continuous;

        [Header("调试")]
        [SerializeField] private Color _gizmoColor = new Color(0, 1, 0, 0.3f);
        [SerializeField] private bool _showGizmos = false;

        [Header("贴地生成")]
        [SerializeField] private bool _alignToGround = true;
        [SerializeField] private float _groundCheckDistance = 10f;
        [SerializeField] private LayerMask _groundLayer;

        #region 属性
        public List<SpawnerData> SpawnablePrefabs { get => _spawnablePrefabs; }
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
            
        }

        public void CalculateTotalWeight()
        {

        }

        public void SpawnPrefabs()
        {
            
        }
    }
}
