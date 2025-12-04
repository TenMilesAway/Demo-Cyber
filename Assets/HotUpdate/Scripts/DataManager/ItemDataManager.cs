using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 物品数据管理器
    /// </summary>
    public class ItemDataManager : BaseManager<ItemDataManager>
    {
        private readonly static Dictionary<int, TBItemData> itemDataDic = new Dictionary<int, TBItemData>();

        public async void Init()
        {
            // 从 json 去读取数据
            List<TBItemData> items = await HAJsonData.LoadAsync<TBItemData>("Assets/HotUpdate/TableData/tbitem.json");

            // 存进 itemDataDic
            foreach (TBItemData item in items)
            {
                itemDataDic[item.id] = item;
            }
        }
        
        public TBItemData GetData(int id)
        {
            return itemDataDic[id];
        }
    }
}
