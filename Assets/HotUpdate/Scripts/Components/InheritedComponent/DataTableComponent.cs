using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class DataTableComponent : BaseComponent
    {
        protected override void Awake()
        {
            base.Awake();

            // 由于部分数据在玩家未登录时无法初始化
            // 因此这里的逻辑全部挪至状态机中
        }
    }
}
