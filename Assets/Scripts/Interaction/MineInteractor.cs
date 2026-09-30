using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    [DisallowMultipleComponent]
    public sealed class MineInteractor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField] private FirstPersonLook look;
        [SerializeField, Min(0.5f)] private float reach = 3f;
        public string Prompt { get; private set; } = "";
        private void Update()
        {
            Prompt = "";
            if (look == null || !look.InputCaptured || viewCamera == null) return;
            Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore)) return;
            Minecart cart = hit.collider.GetComponentInParent<Minecart>();
            if (cart == null) return;
            Prompt = cart.Prompt;
            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            pressed = Input.GetKeyDown(KeyCode.E);
#endif
            if (pressed && cart.CanActivate) { cart.Activate(); Prompt = cart.Prompt; }
        }
        private void OnDisable() { Prompt = ""; }
    }
}
