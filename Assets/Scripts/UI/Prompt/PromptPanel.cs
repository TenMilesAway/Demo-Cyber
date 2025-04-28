using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Cyber
{
    public class PromptPanel : BasePanel
    {
        private string prompt;
        protected override void Start()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(GetControl<Text>("txtPrompt").DOText(prompt, 2, true, ScrambleMode.All));
            sequence.Append(GetControl<Text>("txtPrompt").DOFade(0, 2));

            sequence.OnComplete(() => 
            {
                UIManager.GetInstance().HidePanel("PromptPanel");
            });
        }

        public void InitInfo(string info)
        {
            prompt = info;
        }
    }
}
