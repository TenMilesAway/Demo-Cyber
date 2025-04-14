using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GenshinImpactMovementSystem
{
    public class MainPanel : BasePanel
    {
        private void Start()
        {
            GetControl<Button>("btnInventory").onClick.AddListener(() => 
            {
                UIManager.GetInstance().ShowPanel<InventoryPanel>("InventoryPanel");
            });
        }
    }
}
