using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class PlayerInteractiveTrigger : MonoBehaviour
    {
        [SerializeField]
        private float _interactiveRadius = 2f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractive interactiveObject))
            {
                InteractiveDataManager.GetInstance().AddInteractive(interactiveObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractive interactiveObject))
            {
                InteractiveDataManager.GetInstance().RemoveInteractive(interactiveObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // 设置颜色为黄色，表示交互范围
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            // 在玩家位置绘制球体
            Gizmos.DrawSphere(transform.position, _interactiveRadius);
        }
    }
}
