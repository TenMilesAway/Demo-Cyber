using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public interface IPoolObjectItem
    {
        void OnGetHandle();

        void OnPutHandle();
    }
}
