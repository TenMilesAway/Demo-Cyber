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

        private static string cameraTag = "VirtualCamera";

        
        public void Init()
        {
            cameras = GameObject.FindGameObjectsWithTag(cameraTag);

            brain = Camera.main.GetComponent<CinemachineBrain>();

            for (int i = 0; i < cameras.Length; i++)
            {
                cameraDic.Add(cameras[i].name, cameras[i].GetComponent<CinemachineVirtualCamera>());
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
    }
}
