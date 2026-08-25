using FromCell.Player;
using UnityEngine;

namespace FromCell.Level
{
    public class KillZone : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.Die();
        }
    }
}
