using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class EnemyDataManager : BaseManager<EnemyDataManager>
    {
        private readonly static Dictionary<int, TBEnemyData> enemyDataDic = new Dictionary<int, TBEnemyData>();

        public async void Init()
        {
            List<TBEnemyData> enemyList = await HAJsonData.LoadAsync<TBEnemyData>("Assets/HotUpdate/TableData/tbenemy.json");

            foreach (TBEnemyData enemy in enemyList)
            {
                enemyDataDic[enemy.id] = enemy;
            }
        }

        public TBEnemyData GetData(int id)
        {
            return enemyDataDic[id];
        }
    }
}
