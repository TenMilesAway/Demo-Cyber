using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class DataTableComponent : BaseComponent
    {
        protected override void Awake()
        {
            base.Awake();

            ItemDataManager.GetInstance().Init();
        }
    }
}
