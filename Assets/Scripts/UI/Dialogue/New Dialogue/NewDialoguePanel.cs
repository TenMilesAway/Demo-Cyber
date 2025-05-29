using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Cyber
{
    public class NewDialoguePanel : BasePanel
    {
        private DSDialogue dsDialogue;
        private DSDialogueSO dialogue;
        private DSDialogueSO nextDialogue;
        private List<NewButtonChoice> buttonChoices = new List<NewButtonChoice>();

        private Text txtDialog;

        private string content;

        public GameObject ButtonGroups;

        protected override void Awake()
        {
            base.Awake();

            txtDialog = GetControl<Text>("txtDialog");
        }

        public void Initialize(DSDialogue dsDialogue)
        {
            this.dsDialogue = dsDialogue;

            dialogue = dsDialogue.Dialogue;

            PlayNextDialogue();
        }

        public void PlayNextDialogue()
        {
            if (nextDialogue != null)
            {
                dialogue = nextDialogue;
            }

            content = dialogue.Text;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(txtDialog.DOText(content, 2, true, ScrambleMode.All));
            sequence.OnComplete(() =>
            {
                // 初始化选项
                UpdateButtonGroups();
            });
        }

        public void UpdateButtonGroups()
        {
            // 根据当前 SO 的 Choices index 来更新
            // 如果没有 Choices，那就更新一个结束对话的 Button
            Transform parent = ButtonGroups.transform;

            // 如果没有后续选择
            if (dialogue.Choices.Count == 1 && dialogue.Choices[0].NextDialogue == null)
            {
                NewButtonChoice tempButtonChoice = ResMgr.GetInstance().Load<GameObject>("UI/NewButtonChoice").GetComponent<NewButtonChoice>();

                tempButtonChoice.transform.SetParent(parent);

                DSDialogueChoiceData tempChocie = new DSDialogueChoiceData()
                {
                    Text = "结束对话"
                };

                tempButtonChoice.Initialize(tempChocie);

                buttonChoices.Add(tempButtonChoice);

                Button tempButton = tempButtonChoice.gameObject.GetComponent<Button>();

                tempButton.onClick.AddListener(() =>
                {
                    NewDialogueMgr.GetInstance().DialogueIsOver();
                });

                return;
            }

            Debug.Log("？");

            // 有后续选择，遍历选择项
            foreach (DSDialogueChoiceData choice in dialogue.Choices)
            {
                NewButtonChoice tempButtonChoice = ResMgr.GetInstance().Load<GameObject>("UI/NewButtonChoice").GetComponent<NewButtonChoice>();

                tempButtonChoice.transform.SetParent(parent);

                tempButtonChoice.Initialize(choice);

                buttonChoices.Add(tempButtonChoice);

                Button tempButton = tempButtonChoice.gameObject.GetComponent<Button>();

                tempButton.onClick.AddListener(() =>
                {
                    nextDialogue = choice.NextDialogue;

                    ClearButtonChoices();

                    PlayNextDialogue();
                });
            }

        }

        #region Additional Methods
        private void ClearButtonChoices()
        {
            foreach (NewButtonChoice buttonChoice in buttonChoices)
                Destroy(buttonChoice.gameObject);

            buttonChoices.Clear();
        }
        #endregion
    }
}
