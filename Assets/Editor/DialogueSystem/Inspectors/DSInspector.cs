using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cyber
{
    [CustomEditor(typeof(DSDialogue))]
    public class DSInspector : Editor
    {
        private SerializedProperty dialogueContainerProperty;
        private SerializedProperty dialogueGroupProperty;
        private SerializedProperty dialogueProperty;

        private SerializedProperty groupedDialgueProperty;
        private SerializedProperty startingDialogueOnlyProperty;

        private SerializedProperty selectedDialogueGroupIndexProperty;
        private SerializedProperty selectedDialogueIndexProperty;

        private void OnEnable()
        {
            dialogueContainerProperty = serializedObject.FindProperty("dialogueContainer");
            dialogueGroupProperty = serializedObject.FindProperty("dialogueGroup");
            dialogueProperty = serializedObject.FindProperty("dialogue");

            groupedDialgueProperty = serializedObject.FindProperty("groupedDialogues");
            startingDialogueOnlyProperty = serializedObject.FindProperty("startingDialogueOnly");

            selectedDialogueGroupIndexProperty = serializedObject.FindProperty("selectedDialogueGroupIndex");
            selectedDialogueIndexProperty = serializedObject.FindProperty("selectedDialogueIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制对话容器区域
            DrawDialogueContainerArea();

            // 获得选择的对话容器
            DSDialogueContainerSO dialogueContainer = dialogueContainerProperty.objectReferenceValue as DSDialogueContainerSO;

            if (dialogueContainer == null)
            {
                StopDrawing("请选择一个对话容器");

                return;
            }

            // 绘制过滤器区域
            DrawFiltersArea();

            // 是否只获取起始对话
            bool currentStartingDialoguesOnlyFilter = startingDialogueOnlyProperty.boolValue;

            List<string> dialogueNames;

            // 获得对话容器的文件夹路径
            string dialogueFolderPath = $"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}";

            string dialogueInfoMessage;

            // 是否勾选使用对话分组
            if (groupedDialgueProperty.boolValue)
            {
                // 获得对话分组名列表
                List<string> dialogueGroupNames = dialogueContainer.GetDialogueGroupNames();

                if (dialogueGroupNames.Count == 0)
                {
                    StopDrawing("在这个对话容器中没有任何对话分组");

                    return;
                }

                // 绘制下拉选择对话组
                DrawDialogueGroupArea(dialogueContainer, dialogueGroupNames);

                DSDialogueGroupSO dialogueGroup = dialogueGroupProperty.objectReferenceValue as DSDialogueGroupSO;

                // 获得当前分组的所有对话名
                dialogueNames = dialogueContainer.GetGroupedDialogueNames(dialogueGroup, currentStartingDialoguesOnlyFilter);

                dialogueFolderPath += $"/Groups/{dialogueGroup.GroupName}/Dialogues";

                dialogueInfoMessage = "此对话分组中没有" + (currentStartingDialoguesOnlyFilter ? "起始的" : "")  + "对话";
            }
            // 未勾选使用对话分组
            else
            {
                dialogueNames = dialogueContainer.GetUngroupedDialogueNames(currentStartingDialoguesOnlyFilter);

                dialogueFolderPath += "/Global/Dialogues";

                dialogueInfoMessage = "此对话容器中没有未分组的" + (currentStartingDialoguesOnlyFilter ? "起始的" : "") + "对话";
            }

            // 如果没有对话框
            if (dialogueNames.Count == 0)
            {
                StopDrawing(dialogueInfoMessage);

                return;
            }

            // 绘制对话
            DrawDialogueArea(dialogueNames, dialogueFolderPath);

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw Methods
        private void DrawDialogueContainerArea()
        {
            DSInspectorUtility.DrawHeader("Dialogue Container");

            dialogueContainerProperty.DrawPropertyField();

            DSInspectorUtility.DrawSpace();
        }

        private void DrawFiltersArea()
        {
            DSInspectorUtility.DrawHeader("Filters");

            groupedDialgueProperty.DrawPropertyField();
            startingDialogueOnlyProperty.DrawPropertyField();

            DSInspectorUtility.DrawSpace();
        }

        private void DrawDialogueGroupArea(DSDialogueContainerSO dialogueContainer, List<string> dialogueGroupNames)
        {
            DSInspectorUtility.DrawHeader("Dialogue Group");

            // 获得下拉选择的索引
            int oldSelectedDialogueGroupIndex = selectedDialogueGroupIndexProperty.intValue;

            // 获得上次绘制时的分组 SO
            DSDialogueGroupSO oldDialogueGroup = dialogueGroupProperty.objectReferenceValue as DSDialogueGroupSO;

            bool isOldDialogueGroupNull = oldDialogueGroup == null;

            string oldDialogueGroupName = isOldDialogueGroupNull ? "" : oldDialogueGroup.GroupName;
            
            // 根据上次绘制的情况更新本次绘制的下拉选择
            UpdateIndexOnNamesListUpdate(dialogueGroupNames, selectedDialogueGroupIndexProperty, oldSelectedDialogueGroupIndex, oldDialogueGroupName, isOldDialogueGroupNull);

            selectedDialogueGroupIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue Group", selectedDialogueGroupIndexProperty, dialogueGroupNames.ToArray());

            // 获得本次绘制的下拉选择分组名，以加载 SO
            string selectedDialogueGroupName = dialogueGroupNames[selectedDialogueGroupIndexProperty.intValue];

            DSDialogueGroupSO selectedDialogueGroup = DSIOUtility.LoadAsset<DSDialogueGroupSO>($"Assets/DialogueSystem/Dialogues/{dialogueContainer.FileName}/Groups/{selectedDialogueGroupName}", selectedDialogueGroupName);

            dialogueGroupProperty.objectReferenceValue = selectedDialogueGroup;

            // 禁止选择
            DSInspectorUtility.DrawDisabledFields(()=> dialogueGroupProperty.DrawPropertyField());

            DSInspectorUtility.DrawSpace();
        }

        private void DrawDialogueArea(List<string> dialogueNames, string dialogueFolderPath)
        {
            DSInspectorUtility.DrawHeader("Dialogue");

            // 获得下拉选择的索引
            int oldSelectedDialogueIndex = selectedDialogueIndexProperty.intValue;

            // 获得上次绘制时的对话 SO
            DSDialogueSO oldDialogue = dialogueProperty.objectReferenceValue as DSDialogueSO;

            bool isOldDialogueNull = oldDialogue == null;

            string oldDialogueName = isOldDialogueNull ? "" : oldDialogue.DialogueName;

            // 根据上次绘制的情况更新本次绘制的下拉选择
            UpdateIndexOnNamesListUpdate(dialogueNames, selectedDialogueIndexProperty, oldSelectedDialogueIndex, oldDialogueName, isOldDialogueNull);

            selectedDialogueIndexProperty.intValue = DSInspectorUtility.DrawPopup("Dialogue", selectedDialogueIndexProperty, dialogueNames.ToArray());

            // 获得本次绘制的下拉选择对话框名，以加载 SO
            string selectedDialogueName = dialogueNames[selectedDialogueIndexProperty.intValue];

            DSDialogueSO selectedDialogue = DSIOUtility.LoadAsset<DSDialogueSO>(dialogueFolderPath, selectedDialogueName);

            dialogueProperty.objectReferenceValue = selectedDialogue;

            // 禁止选择
            DSInspectorUtility.DrawDisabledFields(() => dialogueProperty.DrawPropertyField());
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            DSInspectorUtility.DrawHelpBox(reason, messageType);

            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHelpBox("您需要选择一个对话才能使此组件正常运行", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Index Methods
        private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
        {
            if (isOldPropertyNull)
            {
                indexProperty.intValue = 0;

                return;
            }

            // 判断上次绘制的分组索引是否超过了当前分组的最大索引
            bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
            // 判断上次绘制的分组索引对应的名称与本次的分组名是否相同
            bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

            if (oldNameIsDifferentThanSelectedName)
            {
                // 如果新的分组包含上次绘制的分组索引对应分组名
                if (optionNames.Contains(oldPropertyName))
                {
                    indexProperty.intValue = optionNames.IndexOf(oldPropertyName);
                }
                else
                {
                    indexProperty.intValue = 0;
                }
            }
        }
        #endregion
    }
}
