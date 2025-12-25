using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 刷怪数据
    /// </summary>
    [System.Serializable]
    public class SpawnerData
    {
        public string _prefabPath;
        public bool _randomRotation;
        public bool _randomScale;
        public int _weight;
    }

    /// <summary>
    /// 刷怪范围
    /// </summary>
    public enum ESpawnAreaType
    {
        Circle,
        Rectangle,
        Sphere,
        Box,
    }

    /// <summary>
    /// 刷怪模式：一次性 & 持续
    /// </summary>
    public enum ESpawnMode
    {
        OneTime,
        Continuous,
    }

    /// <summary>
    /// 生成数量范围
    /// </summary>
    [System.Serializable]
    public class SpawnNumRange
    {
        public int _min = 0;
        public int _max = 1;

        public SpawnNumRange(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int GetRandomFromRange()
        {
            return Random.Range(_min, _max);
        }
    }

    /// <summary>
    /// 生成物体的缩放范围
    /// </summary>
    [System.Serializable]
    public class SpawnScaleRange
    {
        public float _min = 0.8f;
        public float _max = 1.2f;

        public SpawnScaleRange(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public float GetRandomFromRange()
        {
            return Random.Range(_min, _max);
        }
    }
}
