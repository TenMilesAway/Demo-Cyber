using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class MainPanel : BasePanel
    {
        private Text txtLevel;
        private Text txtGold;
        private Text txtGem;
        private Text txtName;

        protected override void Awake()
        {
            base.Awake();

            InitUI();
        }

        protected override void Start()
        {
            base.Start();
        }

        private void InitUI()
        {
            txtLevel = GetControl<Text>("txtLevel");
            txtGold = GetControl<Text>("txtGold");
            txtGem = GetControl<Text>("txtGem");
            txtName = GetControl<Text>("txtName");

            txtLevel.text = GameDataMgr.GetInstance().playerInfo.level.ToString();
            txtGold.text = GameDataMgr.GetInstance().playerInfo.gold.ToString();
            txtGem.text = GameDataMgr.GetInstance().playerInfo.gem.ToString();
            txtName.text = GameDataMgr.GetInstance().playerInfo.id;

            GetControl<Button>("btnInventory").onClick.AddListener(() =>
            {
                UIManager.GetInstance().ShowPanel<InventoryPanel>("InventoryPanel");
                UIManager.GetInstance().ShowPanel<EquipPanel>("EquipPanel");
            });
        }
    }
}
