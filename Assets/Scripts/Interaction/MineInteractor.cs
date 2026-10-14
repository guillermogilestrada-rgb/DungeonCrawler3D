using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    // E sobre una vagoneta cercana la pone en marcha.
    [DisallowMultipleComponent]
    public sealed class MineInteractor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.5f)] private float reach = 3f;

        private void Update()
        {
            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            pressed = Input.GetKeyDown(KeyCode.E);
#endif
            if (!pressed) return;
            Transform eye = viewCamera.transform;
            if (!Physics.Raycast(eye.position, eye.forward, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore)) return;
            Minecart cart = hit.collider.GetComponentInParent<Minecart>();
            if (cart != null) cart.Activate();
        }
    }
}
