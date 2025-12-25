using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 刷怪点接口
    /// </summary>
    public interface ISpawner
    {
        #region 基础信息
        /// <summary>
        /// 刷怪物体列表
        /// </summary>
        public List<SpawnerData> SpawnablePrefabs { get; }

        /// <summary>
        /// 是否开始时刷新
        /// </summary>
        public bool SpawnOnStart { get; }
        #endregion


        #region 生成范围
        /// <summary>
        /// 生成范围类型
        /// </summary>
        public ESpawnAreaType SpawnAreaType { get; }

        /// <summary>
        /// 生成范围半径
        /// </summary>
        public float SpawnRadius { get; }
        #endregion


        #region 生成数据
        /// <summary>
        /// 生成数量范围
        /// </summary>
        public SpawnNumRange SpawnNumRange { get; }

        /// <summary>
        /// 生成缩放范围
        /// </summary>
        public SpawnScaleRange SpawnScaleRange { get; }
        #endregion


        #region 生成模式
        /// <summary>
        /// 生成模式
        /// </summary>
        public ESpawnMode SpawnMode { get; }
        #endregion


        #region 调试
        /// <summary>
        /// 颜色
        /// </summary>
        public Color GizmoColor { get; }

        /// <summary>
        /// 是否显示调试
        /// </summary>
        public bool ShowGizmos { get; }
        #endregion


        #region 贴地生成
        public bool AlignToGround  { get; }

        public float GroundCheckDistance { get; }

        public LayerMask GoundLayer { get; }
        #endregion

        /// <summary>
        /// 计算总权重
        /// </summary>
        public void CalculateTotalWeight();

        /// <summary>
        /// 刷怪
        /// </summary>
        public void SpawnPrefabs();
    }
}
