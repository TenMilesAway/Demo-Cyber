using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class NewButtonChoice : BasePanel
    {
        private Text txtChoice;

        protected override void Awake()
        {
            base.Awake();

            txtChoice = GetControl<Text>("txtChoice");
        }

        public void Initialize(DSDialogueChoiceData choice)
        {
            txtChoice.text = choice.Text;
        }
    }
}
