using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Injured : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "ReactionCheck")
                return;

            EventCenter.GetInstance().EventTrigger("PlayerInjured", 10f);
        }
    }
}
