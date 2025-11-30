using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class AudioComponent : BaseComponent
    {
        public AudioDataSO audioDataSO;

        private Dictionary<string, AudioData> _audioDic = new Dictionary<string, AudioData>();
        private Dictionary<string, AudioSource> _playingAudioSources = new Dictionary<string, AudioSource>(); 
        private static string s_MusicVolumeKey = "MusicVolume";
        private static string s_SFXVolumeKey = "SFXVolume";
        private static string s_MusicKey = "Music";
        private static string s_SFXKey = "SFX";
        private string _currentPlayMain;
        private bool _isPlaySound;
        private bool _isPlayMainMusic;

        private void Start()
        {
            // 监听 GameMananger.Event

            _audioDic.Clear();

            if (audioDataSO)
            {
                foreach (var data in audioDataSO.conf)
                {
                    _audioDic.Add(data.key, data);
                }
            }

            // 加载音量
            LoadMusicVolumeSetting();
            LoadSFXVolumeSetting();
            LoadSettingStatus();
        }

        #region 初始加载方法
        /// <summary>
        /// 加载背景音乐音量
        /// </summary>
        private void LoadMusicVolumeSetting()
        {
            float volume = 0;

            if (PlayerPrefs.HasKey(s_MusicVolumeKey))
            {
                volume = PlayerPrefs.GetFloat(s_MusicVolumeKey);
            }

            // 给 audio mixer 或 audio source 设置音量
        }

        /// <summary>
        /// 加载音效音量
        /// </summary>
        private void LoadSFXVolumeSetting()
        {
            float volume = 0;

            if (PlayerPrefs.HasKey(s_SFXVolumeKey))
            {
                volume = PlayerPrefs.GetFloat(s_SFXVolumeKey);
            }

            // 给 audio mixer 或 audio source 设置音量
        }

        /// <summary>
        /// 加载音乐、音效的开启
        /// </summary>
        private void LoadSettingStatus()
        {
            // 0 为关闭，1 为开启
            // 音乐
            if (PlayerPrefs.HasKey(s_MusicKey))
            {
                _isPlayMainMusic = PlayerPrefs.GetInt(s_MusicKey) == 1;
            }
            else
            {
                _isPlayMainMusic = true;
                PlayerPrefs.SetInt(s_MusicKey, 1);
            }

            // 音效
            if (PlayerPrefs.HasKey(s_SFXKey))
            {
                _isPlaySound = PlayerPrefs.GetInt(s_SFXKey) == 1;
            }
            else
            {
                _isPlaySound = true;
                PlayerPrefs.SetInt(s_SFXKey, 1);
            }
        }
        #endregion

        #region 调用获取音量设置

        #endregion

        #region 播放

        #endregion
    }
}
