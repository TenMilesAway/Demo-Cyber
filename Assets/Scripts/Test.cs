using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Test : MonoBehaviour
    {
        public Transform Player;
        public Transform Cube;

        private void Start()
        {
            
        }

        private void Update()
        {
            Vector3 PtoC = Cube.position - Player.position;
            Vector3 PF = Player.forward;

            float cosTheta = Vector3.Dot(PtoC, PF) / (PtoC.magnitude * PF.magnitude);
            print(Mathf.Acos(cosTheta) * Mathf.Rad2Deg);
        }
    }
}
