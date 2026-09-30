using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    [DefaultExecutionOrder(-50), DisallowMultipleComponent]
    public sealed class FirstPersonLook : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Light flashlight;
        [Header("Comodidad")]
        [SerializeField, Range(0.025f, 0.3f)] private float sensitivity = 0.1f;
        [SerializeField, Range(60f, 100f)] private float fieldOfView = 78f;
        [SerializeField, Range(45f, 89f)] private float pitchLimit = 85f;
        [SerializeField] private bool invertY;
        [SerializeField] private bool captureOnStart = true;
        private float pitch;
        private bool focused = true;
        private int captureFrame;
        public bool InputCaptured => focused && Cursor.lockState == CursorLockMode.Locked;
        public float Sensitivity { get => sensitivity; set => sensitivity = Mathf.Clamp(value, 0.025f, 0.3f); }
        public float FieldOfView { get => fieldOfView; set { fieldOfView = Mathf.Clamp(value, 60f, 100f); ApplyFov(); } }

        private void Start() { ApplyFov(); SetCapture(captureOnStart); }
        private void Update()
        {
            bool escape = false;
            bool toggleLight = false;
            Vector2 delta = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                escape = Keyboard.current.escapeKey.wasPressedThisFrame;
                toggleLight = Keyboard.current.fKey.wasPressedThisFrame;
            }
            if (Mouse.current != null) delta = Mouse.current.delta.ReadValue();
#elif ENABLE_LEGACY_INPUT_MANAGER
            escape = Input.GetKeyDown(KeyCode.Escape);
            toggleLight = Input.GetKeyDown(KeyCode.F);
            delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 15f;
#endif
            if (escape) { SetCapture(!InputCaptured); return; }
            if (!InputCaptured || Time.frameCount <= captureFrame + 1) return;
            if (toggleLight && flashlight != null) flashlight.enabled = !flashlight.enabled;
            // Mouse.delta ya es desplazamiento por frame: NO se multiplica por deltaTime.
            if (playerBody != null) playerBody.Rotate(Vector3.up, delta.x * sensitivity, Space.World);
            pitch = Mathf.Clamp(pitch + delta.y * sensitivity * (invertY ? 1f : -1f), -pitchLimit, pitchLimit);
            transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
        public void SetCapture(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
            captureFrame = Time.frameCount;
        }
        public void ResetPitch() { pitch = 0f; transform.localRotation = Quaternion.identity; }
        private void ApplyFov() { if (viewCamera != null) viewCamera.fieldOfView = fieldOfView; }
        private void OnApplicationFocus(bool value) { focused = value; if (!value) SetCapture(false); }
        private void OnDisable() { SetCapture(false); }
        private void OnValidate() { ApplyFov(); }
    }
}
