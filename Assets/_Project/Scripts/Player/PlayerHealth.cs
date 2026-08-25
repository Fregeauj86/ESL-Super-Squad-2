using FromCell.Abilities;
using FromCell.Core;
using FromCell.Input;
using FromCell.Level;
using UnityEngine;

namespace FromCell.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] float respawnDelay = 0.25f;

        Rigidbody2D rb;
        AbilityManager abilities;
        bool isDead;

        public bool IsDead => isDead;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            abilities = GetComponent<AbilityManager>();
        }

        public void Die()
        {
            if (isDead) return;
            isDead = true;

            InputGate.Instance?.SetInputEnabled(false);
            GameSignals.RaisePlayerDied();
            Invoke(nameof(Respawn), respawnDelay);
        }

        void Respawn()
        {
            Transform spawn = CheckpointSystem.Instance != null
                ? CheckpointSystem.Instance.GetRespawnPoint()
                : transform;

            transform.position = spawn.position;
            rb.linearVelocity = Vector2.zero;
            abilities?.ResetAirborneAbilities();
            isDead = false;
            InputGate.Instance?.SetInputEnabled(true);
            GameSignals.RaisePlayerRespawned();
        }
    }
}
