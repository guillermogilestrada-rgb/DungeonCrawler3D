using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FrozenMine
{
    // Mirar una vagoneta cercana muestra el aviso; E la pone en marcha.
    [DisallowMultipleComponent]
    public sealed class MineInteractor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.5f)] private float reach = 3f;
        private Minecart target;

        private void Update()
        {
            target = null;
            Transform eye = viewCamera.transform;
            if (Physics.Raycast(eye.position, eye.forward, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore))
                target = hit.collider.GetComponentInParent<Minecart>();
            if (target == null || !target.CanPush) { target = null; return; }

            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            pressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
            pressed = Input.GetKeyDown(KeyCode.E);
#endif
            if (pressed) target.Activate();
        }

        private void OnGUI()
        {
            if (target == null) return;
            GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, Screen.height * 0.6f, Screen.width, 30), "Pulsa E para empujar", style);
        }
    }
}
