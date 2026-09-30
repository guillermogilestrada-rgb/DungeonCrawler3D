using UnityEngine;

namespace FrozenMine
{
    // Trayecto recto y corto. La vagoneta sólo se activa una vez por recorrido.
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public sealed class Minecart : MonoBehaviour
    {
        [Header("Recorrido sobre el eje azul local (+Z)")]
        [SerializeField, Min(0.5f)] private float travelDistance = 12f;
        [SerializeField, Min(0.1f)] private float speed = 3f;
        [SerializeField, Min(0.1f)] private float acceleration = 2f;
        [Header("Ruedas (opcional)")]
        [SerializeField] private Transform[] wheels;
        [SerializeField, Min(0.05f)] private float wheelRadius = 0.28f;
        private Rigidbody body;
        private BoxCollider hitbox;
        private Vector3 startPosition;
        private Vector3 direction;
        private Quaternion[] wheelRotations;
        private float travelled;
        private float velocity;
        private bool activated;
        public bool IsFinished { get; private set; }
        public bool IsBlocked { get; private set; }
        public bool CanActivate => !activated;
        public string Prompt => CanActivate ? "[E] Activar vagoneta" :
            IsFinished ? "Paso despejado" : IsBlocked ? "Deja libre la vía" : "Vagoneta en marcha";

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            hitbox = GetComponent<BoxCollider>();
            startPosition = body.position;
            direction = transform.forward.normalized;
            wheelRotations = new Quaternion[wheels == null ? 0 : wheels.Length];
            for (int i = 0; i < wheelRotations.Length; i++)
                if (wheels[i] != null) wheelRotations[i] = wheels[i].localRotation;
        }
        public void Activate() { if (CanActivate) activated = true; }

        private void FixedUpdate()
        {
            if (!activated || IsFinished) return;
            velocity = Mathf.MoveTowards(velocity, speed, acceleration * Time.fixedDeltaTime);
            float step = Mathf.Min(velocity * Time.fixedDeltaTime, travelDistance - travelled);
            Vector3 center = body.position + body.rotation * hitbox.center;
            Vector3 half = Vector3.Scale(hitbox.size * 0.5f, transform.lossyScale);
            RaycastHit[] hits = Physics.BoxCastAll(center, half, direction, body.rotation,
                step + 0.08f, ~0, QueryTriggerInteraction.Ignore);
            // Un muro o jugador detiene la vagoneta antes de romper nada al otro lado.
            float solidDistance = float.PositiveInfinity;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.attachedRigidbody == body) continue;
                BreakableBarricade barrier = hit.collider.GetComponentInParent<BreakableBarricade>();
                if (barrier == null || barrier.IsBroken)
                    solidDistance = Mathf.Min(solidDistance, hit.distance);
            }
            IsBlocked = solidDistance <= step + 0.08f;
            if (IsBlocked) { velocity = 0f; return; }
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.attachedRigidbody == body || hit.distance > step + 0.025f) continue;
                BreakableBarricade barrier = hit.collider.GetComponentInParent<BreakableBarricade>();
                if (barrier != null) barrier.Break(direction);
            }
            travelled += step;
            body.MovePosition(startPosition + direction * travelled);
            if (wheels != null)
                foreach (Transform wheel in wheels)
                    if (wheel != null) wheel.Rotate(Vector3.up, step / wheelRadius * Mathf.Rad2Deg, Space.Self);
            if (travelled >= travelDistance - 0.001f) { IsFinished = true; velocity = 0f; }
        }

        public void ResetCart()
        {
            body.position = startPosition;
            travelled = velocity = 0f;
            activated = IsFinished = IsBlocked = false;
            for (int i = 0; i < wheelRotations.Length; i++)
                if (wheels[i] != null) wheels[i].localRotation = wheelRotations[i];
        }
        public static void ResetPuzzle()
        {
            foreach (Minecart cart in Object.FindObjectsByType<Minecart>(FindObjectsSortMode.None)) cart.ResetCart();
            foreach (BreakableBarricade barrier in Object.FindObjectsByType<BreakableBarricade>(FindObjectsSortMode.None)) barrier.Restore();
            Physics.SyncTransforms();
        }
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.cyan;
            Vector3 end = origin + transform.forward * travelDistance;
            Gizmos.DrawLine(origin + Vector3.up, end + Vector3.up);
            Gizmos.DrawWireCube(end + Vector3.up * .8f, new Vector3(1.4f, 1.3f, 2f));
        }
    }
}
