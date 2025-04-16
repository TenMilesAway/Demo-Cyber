using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class ButtonChoice : BasePanel
    {
        private Text txtChoice;

        protected override void Awake()
        {
            base.Awake();

            txtChoice = GetControl<Text>("txtChoice");
        }

        public void InitInfo(DialogueChoice choice, DialoguePanel panel)
        {
            txtChoice.text = choice.answerOption;
        }
    }
}
