using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Cyber
{
    public class DialoguePanel : BasePanel
    {
        private List<DialogueString> dialogueStringsList;
        private List<ButtonChoice> buttonChoicesList = new List<ButtonChoice>();

        private Text txtDialog;
        private string content;

        public GameObject ButtonGroups;

        protected override void Awake()
        {
            base.Awake();
            txtDialog = GetControl<Text>("txtDialog");
        }

        public void InitDialogueStrings(List<DialogueString> dialogueStrings)
        {
            this.dialogueStringsList = dialogueStrings;
            PlayNextDialogue(1);
        }

        public void PlayNextDialogue(int id)
        {
            int index = id - 1;

            content = dialogueStringsList[index].text;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtDialog.DOText(content, 2, true, ScrambleMode.All));
            sequence.OnComplete(() =>
            {
                // 初始化选项
                if (!dialogueStringsList[index].isQuestion)
                    return;
                UpdateButtonGroups(id);
            });
        }

        public void UpdateButtonGroups(int id)
        {
            int index = id - 1;

            Transform parent = ButtonGroups.transform;

            foreach (DialogueChoice choice in dialogueStringsList[index].dialogueChoices)
            {
                ButtonChoice tempButtonChoice = ResMgr.GetInstance().Load<GameObject>("UI/ButtonChoice").GetComponent<ButtonChoice>();
                tempButtonChoice.transform.SetParent(parent);
                tempButtonChoice.InitInfo(choice, this);
                buttonChoicesList.Add(tempButtonChoice);

                Button tempButton = tempButtonChoice.gameObject.GetComponent<Button>();
                tempButton.onClick.AddListener(() =>
                {
                    if (choice.isEnd)
                    {
                        // 如果对话完结，需要清空所有面板，生成主要面板
                        //EventCenter.GetInstance().EventTrigger("ClearAllPanels", "over");
                        //UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
                        //DialogueMgr.GetInstance().isTalking = false;
                        //CameraController.GetInstance().SwitchCamera("PlayerCamera");
                        DialogueMgr.GetInstance().DialogueIsOver();
                        return;
                    }

                    ClearButtonChoicesList();

                    PlayNextDialogue(choice.optionIndexJump);
                });
            }
        }

        public void ClearButtonChoicesList()
        {
            foreach (ButtonChoice buttonChoice in buttonChoicesList)
                Destroy(buttonChoice.gameObject);

            buttonChoicesList.Clear();
        }
    }
}
