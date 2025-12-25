using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class LevelDataManager : BaseManager<LevelDataManager>
    {
        private readonly static Dictionary<int, TBLevelData> levelDataDic = new Dictionary<int, TBLevelData>();

        public async void Init()
        {
            List<TBLevelData> levelList = await HAJsonData.LoadAsync<TBLevelData>("Assets/HotUpdate/TableData/tblevel.json");

            foreach (TBLevelData level in levelList)
            {
                levelDataDic[level.id] = level;
            }
        }

        public TBLevelData GetData(int level)
        {
            return levelDataDic[level];
        }
    }
}
