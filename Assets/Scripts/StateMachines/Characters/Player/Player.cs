using HA;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerResizableCapsuleCollider))]
    public class Player : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public PlayerSO Data { get; private set; }

        [field: Header("Collisions")]
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }

        [field: Header("Camera")]
        [field: SerializeField] public PlayerCameraRecenteringUtility CameraRecenteringUtility { get; private set; }

        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }

        public PlayerInput Input { get; private set; }
        public PlayerResizableCapsuleCollider ResizableCapsuleCollider { get; private set; }

        public Transform MainCameraTransform { get; set; }

        public PlayerMovementStateMachine movementStateMachine;

        public List<AttackSO> combo;

        private void Awake()
        {
            CameraRecenteringUtility.Initialize();
            AnimationData.Initialize();

            Rigidbody = GetComponent<Rigidbody>();
            Animator = GetComponentInChildren<Animator>();

            Input = GetComponent<PlayerInput>();
            ResizableCapsuleCollider = GetComponent<PlayerResizableCapsuleCollider>();

            MainCameraTransform = Camera.main.transform;

            movementStateMachine = new PlayerMovementStateMachine(this);

            AddEventLiseners();

            DontDestroyOnLoad(transform.parent.gameObject);
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdlingState);

            PlayerDataManager.GetInstance().SetPlayer(transform.parent.transform);
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }

        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            movementStateMachine.OnTriggerStay(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            movementStateMachine.OnTriggerExit(collider);
        }

        public void OnMovementStateAnimationEnterEvent()
        {
            movementStateMachine.OnAnimationEnterEvent();
        }

        public void OnMovementStateAnimationExitEvent()
        {
            movementStateMachine.OnAnimationExitEvent();
        }

        public void OnMovementStateAnimationTransitionEvent()
        {
            movementStateMachine.OnAnimationTransitionEvent();
        }

        private void AddEventLiseners()
        {
            GameManager.Event.AddListener<int>(GameEventType.PlayerReaction, OnReaction);
        }

        private void OnReaction(int damage)
        {
            HADebug.Log("受到伤害: " + damage);

            // 扣血 (未申报服务端)
            PlayerDataManager.GetInstance().GetPlayerInfo()._currentHP -= damage;
            GameManager.Event.Broadcast<HA.PlayerInfo>(GameEventType.UpdateMainPanelUI, PlayerDataManager.GetInstance().GetPlayerInfo());

            movementStateMachine.ChangeState(movementStateMachine.ReactionState);
        }
    }
}