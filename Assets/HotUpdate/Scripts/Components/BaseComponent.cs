using UnityEngine;

namespace HA
{
    public class BaseComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameManager.RegisterComponent(this);
        }
    }
}
