using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HA
{
    public class ItemDataManager : BaseManager<ItemDataManager>
    {
        private readonly static Dictionary<int, ItemData> itemDataDic = new Dictionary<int, ItemData>();

        public async void Init()
        {
            // 从 json 去读取数据
            List<ItemData> items = await HAJsonData.LoadAsync<ItemData>("Assets/HotUpdate/TableData/tbitem.json");

            // 存进 itemDataDic
            foreach (ItemData item in items)
            {
                itemDataDic[item.id] = item;
            }
        }

        public ItemData GetItemData(int id)
        {
            return itemDataDic[id];
        }
    }
}
