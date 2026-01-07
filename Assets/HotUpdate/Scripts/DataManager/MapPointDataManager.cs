using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 用于地图切换后设置点位信息的单例类
    /// </summary>
    public class MapPointDataManager : BaseManager<MapPointDataManager>
    {
        private static GameObject _player = GameManager.GlobalData.Player;

        public void SetPoint(Vector3 point)
        {
            Transform playerTransform = _player.transform.GetChild(0);
            Transform cameraTransform = _player.transform.GetChild(1).GetChild(0);

            playerTransform.position = point;
            cameraTransform.position = point;
            playerTransform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
