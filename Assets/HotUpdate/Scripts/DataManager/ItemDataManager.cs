using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// Item ±íÊý¾Ý
    /// </summary>
    public class ItemDataManager : BaseManager<ItemDataManager>
    {
        private readonly static Dictionary<int, TBItemData> itemDataDic = new Dictionary<int, TBItemData>();

        public async void Init()
        {
            List<TBItemData> items = await HAJsonData.LoadAsync<TBItemData>("Assets/HotUpdate/TableData/tbitem.json");

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
