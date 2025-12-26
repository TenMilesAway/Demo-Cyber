using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class PlayerReactionTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _reactionLayer;

        private void OnTriggerEnter(Collider other)
        {
            int layer = other.gameObject.layer;
            if (layer == 27) HADebug.Log("´¥Åöµ½ÁË¹¥»÷Åö×²Ìå");
            if ((1 << layer & _reactionLayer) != 0)
            {
                BTForEnemy bt = other.GetComponentInParent<BTForEnemy>();
                int damage = bt.CalculateAttack();
                GameManager.Event.Broadcast<int>(GameEventType.PlayerReaction, damage);
            }
        }
    }
}
