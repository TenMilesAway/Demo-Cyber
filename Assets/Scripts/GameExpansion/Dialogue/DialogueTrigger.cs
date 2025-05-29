using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Cyber
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private List<DialogueString> dialogueStrings = new List<DialogueString>();
        [SerializeField] private Transform NPCTransform;

        private DSDialogue dialogue;

        private void Awake()
        {
            dialogue = GetComponent<DSDialogue>();
        }

        private void OnTriggerEnter(Collider other)
        {
            //DialogueMgr.GetInstance().OnTriggerEnter(other, dialogueStrings, NPCTransform);
            NewDialogueMgr.GetInstance().OnTriggerEnter(other, dialogue, NPCTransform);
        }

        private void OnTriggerExit(Collider other)
        {
            //DialogueMgr.GetInstance().OnTriggerExit(other);
            NewDialogueMgr.GetInstance().OnTriggerExit(other);
        }
    }

    #region Old Dialgue System
    [System.Serializable]
    public class DialogueString
    {
        public string text;

        [Header("Branch")]
        public bool isQuestion;
        public List<DialogueChoice> dialogueChoices;

        [Header("Triggered Events")]
        public UnityEvent startDialogueEvent;
        public UnityEvent endDialogueEvent;
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string answerOption;
        public int optionIndexJump;
        public bool isEnd;

        public UnityEvent chooseEvent;
        
    }
    #endregion
}
