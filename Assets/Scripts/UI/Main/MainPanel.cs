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

        private void Start()
        {
            UpdatePlayerInfo();

            GetControl<Button>("btnInventory").onClick.AddListener(() => 
            {
                UIManager.GetInstance().ShowPanel<InventoryPanel>("InventoryPanel");
            });
        }

        public void UpdatePlayerInfo()
        {
            txtLevel = GetControl<Text>("txtLevel");
            txtGold = GetControl<Text>("txtGold");
            txtGem = GetControl<Text>("txtGem");
            txtName = GetControl<Text>("txtName");

            txtLevel.text = GameDataMgr.GetInstance().playerInfo.level.ToString();
            txtGold.text = GameDataMgr.GetInstance().playerInfo.gold.ToString();
            txtGem.text = GameDataMgr.GetInstance().playerInfo.gem.ToString();
            txtName.text = GameDataMgr.GetInstance().playerInfo.name;
        }
    }
}
