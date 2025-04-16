using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyber
{
    public class DialogueMgr : BaseManager<DialogueMgr>
    {
        private List<DialogueString> dialogueStrings;
        private Transform NPCTransform;

        public Player player;

        public bool isTalking;

        public void Init()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        public void OnTriggerEnter(Collider other, List<DialogueString> dialogueStrings, Transform NPCTransform)
        {
            ShowInteractionPanel(other);

            this.dialogueStrings = dialogueStrings;
            this.NPCTransform = NPCTransform;
        }

        public void OnTriggerExit(Collider other)
        {
            HideInteractionPanel();
        }

        #region Main Methods
        private void ShowInteractionPanel(Collider other)
        {
            if (other.CompareTag("Player") && !isTalking)
            {
                // 显示交互提示面板
                UIManager.GetInstance().ShowPanel<InteractionPanel>("InteractionPanel");
                // 允许使用 E 交互对话
                player.Input.PlayerActions.TalkInteraction.started += Talk;
            }
        }

        private void HideInteractionPanel()
        {
            // 隐藏交互提示面板
            if (UIManager.GetInstance().GetPanel<InteractionPanel>("InteractionPanel") != null)
                UIManager.GetInstance().HidePanel("InteractionPanel");

            // 禁止使用 E 交互对话
            player.Input.PlayerActions.TalkInteraction.started -= Talk;
        }

        private void Talk(InputAction.CallbackContext call)
        {
            // 正在对话
            isTalking = true;
            // 因为对话进行了，所以把事件先移除
            player.Input.PlayerActions.TalkInteraction.started -= Talk;

            // 禁用用户操作
            DisablePlayerActions();

            // 相互看
            LookAtEachOther();

            // 先隐藏其它面板
            if (UIManager.GetInstance().GetPanel<InteractionPanel>("InteractionPanel") != null)
                EventCenter.GetInstance().EventTrigger("ClearAllPanels", "ok");

            // 切换相机
            CameraController.GetInstance().SwitchCamera("DialogueCamera");

            // 显示对话面板
            UIManager.GetInstance().ShowPanel<DialoguePanel>("DialoguePanel", E_UI_Layer.System, (panel) => 
            {
                panel.InitDialogueStrings(dialogueStrings);
            });
        }

        private void LookAtEachOther()
        {
            // 玩家看向 NPC 无效
            player.transform.LookAt(NPCTransform);

            NPCTransform.LookAt(player.transform);
        }

        public void DialogueIsOver()
        {
            isTalking = false;

            EventCenter.GetInstance().EventTrigger("ClearAllPanels", "over");
            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
            CameraController.GetInstance().SwitchCamera("PlayerCamera", EnablePlayerActions);
        }

        public void DisablePlayerActions()
        {
            player.Input.PlayerActions.Disable();
        }

        public void EnablePlayerActions()
        {
            player.Input.PlayerActions.Enable();
        }
        #endregion
    }
}
