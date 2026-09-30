using System.Collections;
using UnityEngine;

namespace FrozenMine
{
    [DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
    public sealed class BreakableBarricade : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private Material debrisMaterial;
        [SerializeField, Range(0, 20)] private int fragmentCount = 12;
        private BoxCollider blocker;
        private GameObject[] fragments;
        public bool IsBroken { get; private set; }
        private void Awake() { blocker = GetComponent<BoxCollider>(); }
        public void Break(Vector3 direction)
        {
            if (IsBroken) return;
            IsBroken = true;
            blocker.enabled = false; // El acceso queda libre inmediatamente.
            if (visuals != null) visuals.SetActive(false);
            StartCoroutine(Scatter(direction));
        }
        private IEnumerator Scatter(Vector3 direction)
        {
            fragments = new GameObject[fragmentCount];
            Vector3[] origins = new Vector3[fragmentCount];
            Vector3[] velocities = new Vector3[fragmentCount];
            Vector3 center = transform.TransformPoint(blocker.center);
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.name = "Astilla_temporal";
                piece.layer = 2; // Ignore Raycast: no bloquea la interacción.
                Collider collider = piece.GetComponent<Collider>();
                collider.enabled = false;
                Destroy(collider);
                piece.GetComponent<Renderer>().sharedMaterial = debrisMaterial;
                origins[i] = center + transform.right * Random.Range(-1.5f, 1.5f) + Vector3.up * Random.Range(-.6f, .6f);
                velocities[i] = direction * Random.Range(1.8f, 3.5f) + Vector3.up * Random.Range(1.3f, 3f)
                              + transform.right * Random.Range(-1.3f, 1.3f);
                piece.transform.position = origins[i];
                piece.transform.rotation = Random.rotation;
                piece.transform.localScale = new Vector3(.13f, .13f, .5f);
                fragments[i] = piece;
            }
            float time = 0f;
            while (time < 1.25f)
            {
                time += Time.deltaTime;
                for (int i = 0; i < fragmentCount; i++)
                {
                    if (fragments[i] == null) continue;
                    Vector3 pos = origins[i] + velocities[i] * time + Vector3.down * (4f * time * time);
                    pos.y = Mathf.Max(transform.position.y + .08f, pos.y);
                    fragments[i].transform.position = pos;
                    fragments[i].transform.Rotate(new Vector3(110f, 65f, 30f) * Time.deltaTime);
                    fragments[i].transform.localScale = new Vector3(.13f, .13f, .5f) * Mathf.Clamp01((1.25f - time) * 3f);
                }
                yield return null;
            }
            ClearFragments();
        }
        public void Restore()
        {
            StopAllCoroutines();
            ClearFragments();
            IsBroken = false;
            if (blocker != null) blocker.enabled = true;
            if (visuals != null) visuals.SetActive(true);
        }
        private void ClearFragments()
        {
            if (fragments != null) foreach (GameObject piece in fragments) if (piece != null) Destroy(piece);
            fragments = null;
        }
        private void OnDisable() { StopAllCoroutines(); ClearFragments(); }
    }
}
