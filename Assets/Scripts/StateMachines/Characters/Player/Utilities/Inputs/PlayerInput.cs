using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using HA;

namespace Cyber
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions { get; private set; }
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }

        private void Awake()
        {
            InputActions = new PlayerInputActions();

            PlayerActions = InputActions.Player;

            GameManager.Event.AddListener(GameEventType.EnablePlayerInput, InputActions.Enable);
            GameManager.Event.AddListener(GameEventType.DisablePlayerInput, InputActions.Disable);
            GameManager.Event.AddListener(GameEventType.EnablePlayerFlipInput, InputActions.Player.Flip.Enable);
            GameManager.Event.AddListener(GameEventType.DisablePlayerFlipInput, InputActions.Player.Flip.Disable);
            GameManager.Event.AddListener(GameEventType.EnableInteractiveInput, InputActions.Player.InteractiveOption.Enable);
            GameManager.Event.AddListener(GameEventType.DisableInteractiveInput, InputActions.Player.InteractiveOption.Disable);
        }

        public void OnEnable()
        {
            InputActions.Enable();
        }

        public void OnDisable()
        {
            InputActions.Disable();
        }

        public void DisableActionFor(InputAction action, float seconds)
        {
            StartCoroutine(DisableAction(action, seconds));
        }

        private IEnumerator DisableAction(InputAction action, float seconds)
        {
            action.Disable();

            yield return new WaitForSeconds(seconds);

            action.Enable();
        }
    }
}