using UnityEngine;

namespace Cyber
{
    public class BaseComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameManager.RegisterComponent(this);
        }
    }
}
