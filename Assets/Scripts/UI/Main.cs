using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
        }
    }
}
