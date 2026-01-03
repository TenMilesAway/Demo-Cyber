using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[Serializable]
public sealed partial class GlobalDefine
{
    // UI
    public const string MainPanel = "Assets/UI/Main View/Prefabs/MainPanel.prefab";
    public const string LoadingPanel = "Assets/UI/Start/Prefabs/LoadingPanel.prefab";
    public const string ToastPanel = "Assets/UI/Start/Prefabs/ToastPanel.prefab";
    public const string InventoryPanel = "Assets/UI/Items/Prefabs/InventoryPanel.prefab";
    public const string ItemCell = "Assets/UI/Items/Prefabs/ItemCell.prefab";
    public const string ItemImage = "Assets/UI/Items/Prefabs/ItemImage.prefab";
    public const string ItemDetailInfoPanel = "Assets/UI/Items/Prefabs/ItemDetailInfoPanel.prefab";
    public const string InteractivePanel = "Assets/UI/Interactive/Prefabs/InteractivePanel.prefab";
    public const string InteractiveOption = "Assets/UI/Interactive/Prefabs/InteractiveOption.prefab";
    public const string DialoguePanel = "Assets/UI/Interactive/Prefabs/DialoguePanel.prefab";
    public const string DialogueOption = "Assets/UI/Interactive/Prefabs/DialogueOption.prefab";
    public const string TreasurePanel = "Assets/UI/Interactive/Prefabs/TreasurePanel.prefab";
    public const string MapPanel = "Assets/UI/Map/Prefabs/MapPanel.prefab";
    public const string PropertyPanel = "Assets/UI/Items/Prefabs/PropertyPanel.prefab";
    public const string EquipmentTipPanel = "Assets/UI/Items/Prefabs/EquipmentTipPanel.prefab";
    public const string DamagePanel = "Assets/UI/Common/Prefabs/DamagePanel.prefab";

    // 敌人
    public const string WhiteBaboon = "Assets/UI/Spawner/Prefabs/WhiteBaboon.prefab";

    // 宝箱
    public const string FeiCuiLinHaiTreasure1 = "Assets/UI/Spawner/Prefabs/PrettyTreasure_1.prefab";
    public const string FeiCuiLinHaiTreasure2 = "Assets/UI/Spawner/Prefabs/PrettyTreasure_2.prefab";
    public const string FeiCuiLinHaiTreasure3 = "Assets/UI/Spawner/Prefabs/PrettyTreasure_3.prefab";
    public const string FeiCuiLinHaiTreasure4 = "Assets/UI/Spawner/Prefabs/PrettyTreasure_4.prefab";
    public const string FeiCuiLinHaiTreasure5 = "Assets/UI/Spawner/Prefabs/PrettyTreasure_5.prefab";

    private static Dictionary<string, string> _pathCache = null;

    private static void InitializeCache()
    {
        if (_pathCache != null) return;

        _pathCache = new Dictionary<string, string>();

        Type type = typeof(GlobalDefine);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(string))
            {
                string fieldName = field.Name;
                string fieldValue = field.GetValue(null) as string;
                _pathCache[fieldName] = fieldValue;
            }
        }
    }

    public static string GetPath(string name)
    {
        // 这一步很耗时间，考虑预热
        InitializeCache();

        if (_pathCache.TryGetValue(name, out string path))
        {
            return path;
        }

        HADebug.LogErrorFormat("未找到 {0} 预制体的路径", name);
        return string.Empty;
    }
}

