using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    // Ratón -> giro del cuerpo (horizontal) y de la cámara (vertical). F enciende la linterna.
    [DefaultExecutionOrder(-50), DisallowMultipleComponent]
    public sealed class FirstPersonLook : MonoBehaviour
    {
        [SerializeField] private Transform playerBody;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private Light flashlight;
        [SerializeField, Range(0.025f, 0.3f)] private float sensitivity = 0.1f;
        [SerializeField, Range(60f, 100f)] private float fieldOfView = 78f;
        [SerializeField, Range(45f, 89f)] private float pitchLimit = 85f;
        private float pitch;

        private void Start() { ApplyFov(); LockCursor(); }

        private void Update()
        {
            Vector2 delta = Vector2.zero;
            bool toggleLight = false;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) delta = Mouse.current.delta.ReadValue();
            if (Keyboard.current != null) toggleLight = Keyboard.current.fKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * 15f;
            toggleLight = Input.GetKeyDown(KeyCode.F);
#endif
            if (toggleLight && flashlight != null) flashlight.enabled = !flashlight.enabled;
            playerBody.Rotate(Vector3.up, delta.x * sensitivity, Space.World);
            pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -pitchLimit, pitchLimit);
            transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        public void ResetPitch() { pitch = 0f; transform.localRotation = Quaternion.identity; }
        private void ApplyFov() { if (viewCamera != null) viewCamera.fieldOfView = fieldOfView; }
        private static void LockCursor() { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        private void OnApplicationFocus(bool focused) { if (focused) LockCursor(); }
        private void OnValidate() { ApplyFov(); }
    }
}
