using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// Convert 和 ConvertGroup 表数据
    /// </summary>
    public class ConvertDataManager : BaseManager<ConvertDataManager>
    {
        private readonly static Dictionary<int, TBConvertGroupData> convertGroupDataDic = new Dictionary<int, TBConvertGroupData>();
        private readonly static Dictionary<int, TBConvertData> convertDataDic = new Dictionary<int, TBConvertData>();

        public async void Init()
        {
            List<TBConvertGroupData> convertGroupList = await HAJsonData.LoadAsync<TBConvertGroupData>("Assets/HotUpdate/TableData/tbconvertgroup.json");

            foreach (TBConvertGroupData convertGroup in convertGroupList)
            {
                convertGroupDataDic[convertGroup.id] = convertGroup;
            }

            List<TBConvertData> convertList = await HAJsonData.LoadAsync<TBConvertData>("Assets/HotUpdate/TableData/tbconvert.json");

            foreach (TBConvertData convert in convertList)
            {
                convertDataDic[convert.id] = convert;
            }
        }

        /// <summary>
        /// 获得兑换商人数据
        /// </summary>
        public TBConvertGroupData GetGroupData(int id)
        {
            if (id == 0) return null;

            return convertGroupDataDic[id];
        }

        /// <summary>
        /// 获得兑换商人下兑换列表数据
        /// </summary>
        public List<TBConvertData> GetConvertDatas(int id)
        {
            if (id == 0) return null;

            List<TBConvertData> result = new List<TBConvertData>();

            foreach (TBConvertData data in convertDataDic.Values)
            {
                if (data.parent == id)
                {
                    result.Add(data);
                }
            }

            return result;
        }
    }
}
