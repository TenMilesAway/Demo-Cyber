using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class ConvertParam : OpenUIParam
    {
        public int id; // 兑换商人 ID
    }

    public class ConvertPanel : UIBasePanel
    {
        [SerializeField] private Transform _convertLineContainer;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Button _btnClose;

        private int _convertGroupID = 0;

        public override string GetPanelName()
        {
            return GlobalDefine.ConvertPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            ConvertParam convertParam = param as ConvertParam;
            _convertGroupID = convertParam.id;

            // 获得数据
            TBConvertGroupData groupData = ConvertDataManager.GetInstance().GetGroupData(_convertGroupID);
            List<TBConvertData> convertDatas = ConvertDataManager.GetInstance().GetConvertDatas(_convertGroupID);

            // 初始化信息
            _txtTitle.text = groupData.name;
            foreach (TBConvertData data in convertDatas)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.SingleConvertLine, GetInstanceID().ToString(), (convertLine) =>
                {
                    convertLine.GetComponent<SingleConvertLine>().Init(data);
                    convertLine.transform.SetParent(_convertLineContainer, false);
                });
            }

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            for (int i = _convertLineContainer.childCount - 1; i >= 0; i--)
            {
                Transform convertLine = _convertLineContainer.GetChild(i);
                convertLine.GetComponent<SingleConvertLine>().PutBackToPool();

                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.SingleConvertLine, convertLine.gameObject);
            }

            RemoveListeners();
        }

        private void AddListeners()
        {
            _btnClose.onClick.AddListener(OnClickBtnClose);
        }

        private void RemoveListeners()
        {
            _btnClose.onClick.RemoveAllListeners();
        }

        #region 监听方法：UI
        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        private void OnClickBtnClose()
        {
            UIManager.GetInstance().ClosePanel(GetPanelName());
        }
        #endregion
    }
}
