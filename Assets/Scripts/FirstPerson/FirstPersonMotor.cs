using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    // CharacterController resuelve las colisiones; este script aporta velocidad y gravedad.
    [DisallowMultipleComponent, RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonMotor : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private FirstPersonLook look;
        [SerializeField] private Transform spawnPoint;
        [Header("Movimiento (metros / segundo)")]
        [SerializeField, Min(0.1f)] private float walkSpeed = 3.6f;
        [SerializeField, Min(0.1f)] private float runSpeed = 5.5f;
        [SerializeField, Min(0.1f)] private float acceleration = 24f;
        [Header("Gravedad y seguridad")]
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float respawnBelowY = -8f;

        private CharacterController controller;
        private Vector3 planarVelocity;
        private float verticalVelocity;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        private void Update()
        {
            if (transform.position.y < respawnBelowY) { Respawn(); return; }
            Vector2 input = Vector2.zero;
            bool running = false;
            bool reset = false;
#if ENABLE_INPUT_SYSTEM
            Keyboard keys = Keyboard.current;
            if (keys != null)
            {
                input.x = (keys.dKey.isPressed || keys.rightArrowKey.isPressed ? 1f : 0f)
                        - (keys.aKey.isPressed || keys.leftArrowKey.isPressed ? 1f : 0f);
                input.y = (keys.wKey.isPressed || keys.upArrowKey.isPressed ? 1f : 0f)
                        - (keys.sKey.isPressed || keys.downArrowKey.isPressed ? 1f : 0f);
                running = keys.leftShiftKey.isPressed || keys.rightShiftKey.isPressed;
                reset = keys.rKey.wasPressedThisFrame;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            running = Input.GetKey(KeyCode.LeftShift);
            reset = Input.GetKeyDown(KeyCode.R);
#endif
            if (reset) { Respawn(); return; }
            input = Vector2.ClampMagnitude(input, 1f); // Diagonal sin velocidad extra.
            Vector3 target = (transform.right * input.x + transform.forward * input.y)
                           * (running ? runSpeed : walkSpeed);
            planarVelocity = Vector3.MoveTowards(planarVelocity, target, acceleration * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity = Mathf.Max(verticalVelocity + gravity * Time.deltaTime, -40f);
            CollisionFlags hit = controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            if ((hit & CollisionFlags.Above) != 0 && verticalVelocity > 0f) verticalVelocity = 0f;
        }

        public void Respawn()
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(spawnPoint != null ? spawnPoint.position : initialPosition,
                spawnPoint != null ? spawnPoint.rotation : initialRotation);
            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;
            controller.enabled = true;
            if (look != null) look.ResetPitch();
            Minecart.ResetPuzzle();
        }

        private void OnValidate()
        {
            runSpeed = Mathf.Max(runSpeed, walkSpeed);
            gravity = Mathf.Min(gravity, -0.1f);
        }
    }
}
