using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GenshinImpactMovementSystem
{
    public class InventoryPanel : BasePanel
    {
        private void Start()
        {
            GetControl<Button>("btnClose").onClick.AddListener(() => 
            {
                UIManager.GetInstance().HidePanel("InventoryPanel");
            });
        }
    }
}
