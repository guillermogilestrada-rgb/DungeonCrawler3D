using UnityEngine;

namespace FrozenMine
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DungeonGoal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            FirstPersonMotor player = other.GetComponentInParent<FirstPersonMotor>();
            if (player == null) return;
            DungeonHUD hud = player.GetComponent<DungeonHUD>();
            if (hud != null) hud.CompleteJourney();
        }
        private void Reset() { GetComponent<BoxCollider>().isTrigger = true; }
    }
}
