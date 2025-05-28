using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class DSDialogue : MonoBehaviour
    {
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;

        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialogueOnly;

        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;
    }
}
