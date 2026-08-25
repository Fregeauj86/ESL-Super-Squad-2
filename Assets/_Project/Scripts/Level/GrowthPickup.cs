using FromCell.Core;
using FromCell.Player;
using UnityEngine;

namespace FromCell.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class GrowthPickup : MonoBehaviour
    {
        [SerializeField] float speedBonus = 0.5f;
        [SerializeField] float jumpBonus = 1f;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.ApplyGrowthBonus(speedBonus, jumpBonus);

            GameSignals.RaiseGrowthPickupCollected();
            Destroy(gameObject);
        }
    }
}
