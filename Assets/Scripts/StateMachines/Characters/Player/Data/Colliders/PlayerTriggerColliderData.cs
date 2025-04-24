using System;
using UnityEngine;

namespace Cyber
{
    [Serializable]
    public class PlayerTriggerColliderData
    {
        [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }

        public Vector3 GroundCheckColliderVerticalExtents { get; private set; }

        [field: SerializeField] public BoxCollider AttackCheckCollider { get; private set; }

        public Vector3 AttackCheckColliderVerticalExtent { get; private set; }

        public void Initialize()
        {
            GroundCheckColliderVerticalExtents = GroundCheckCollider.bounds.extents;

            AttackCheckColliderVerticalExtent = AttackCheckCollider.bounds.extents;

            HideAttackCheckCollider();
        }

        public void ShowAttackCheckCollider()
        {
            AttackCheckCollider.gameObject.SetActive(true);
        }

        public void HideAttackCheckCollider()
        {
            AttackCheckCollider.gameObject.SetActive(false);
        }
    }
}