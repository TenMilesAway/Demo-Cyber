using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

namespace Cyber
{
    public class CameraController : BaseManager<CameraController>
    {
        private GameObject[] cameras;

        private Dictionary<string, CinemachineVirtualCamera> cameraDic = new Dictionary<string, CinemachineVirtualCamera>();

        private CinemachineBrain brain;

        private CinemachineVirtualCamera mainCamera;

        private static string virtualCameraTag = "VirtualCamera";

        private static string mainCameraTag = "PlayerCamera";

        
        public void Init()
        {
            cameras = GameObject.FindGameObjectsWithTag(virtualCameraTag);

            brain = Camera.main.GetComponent<CinemachineBrain>();

            for (int i = 0; i < cameras.Length; i++)
            {
                cameraDic.Add(cameras[i].name, cameras[i].GetComponent<CinemachineVirtualCamera>());

                if (cameras[i].name == mainCameraTag)
                    mainCamera = cameraDic[cameras[i].name];
            }
        }

        public void SwitchCamera(string cameraName)
        {
            foreach (string camera in cameraDic.Keys)
            {
                if (camera != cameraName)
                    cameraDic[camera].Priority = 0;

                if (camera == cameraName)
                    cameraDic[camera].Priority = 1;
            }
        }

        public void SwitchCamera(string cameraName, UnityAction callback)
        {
            foreach (string camera in cameraDic.Keys)
            {
                if (camera != cameraName)
                    cameraDic[camera].Priority = 0;

                if (camera == cameraName)
                    cameraDic[camera].Priority = 1;
            }

            if (callback != null)
                callback();
        }

        public void SwitchToShake(float amplitudeGain)
        {
            mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitudeGain;
        }

        public void ReturnToNoShake()
        {
            mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        }
    }
}
