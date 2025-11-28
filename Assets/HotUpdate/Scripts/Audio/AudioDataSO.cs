using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(fileName = "AudioDataSO", menuName = "AudioDataSO", order = 0)]
    public class AudioDataSO : ScriptableObject
    {
        public string desc = "此配置是自动生成的，请勿手动修改";

        [SerializeField]
        public List<AudioData> conf;
    }

    [Serializable]
    public class AudioData
    {
        // key 值
        public string key;
        // 存储路径
        public string path;
        // 是否循环播放
        public bool loop;
    }
}
