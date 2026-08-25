using UnityEngine;

namespace FromCell.Level
{
    public class RoleGate : MonoBehaviour
    {
        [SerializeField] PlayerRoleState.SquadRole requiredRole = PlayerRoleState.SquadRole.Nerve;
        [SerializeField] Collider2D blockingCollider;

        // Additive: lets runtime-built levels (LevelAssembler) configure this without
        // SerializedObject.FindProperty, which only works in the editor.
        public void Configure(PlayerRoleState.SquadRole role, Collider2D blocker)
        {
            requiredRole = role;
            blockingCollider = blocker;
        }

        void Awake()
        {
            if (blockingCollider == null)
                blockingCollider = GetComponent<Collider2D>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || blockingCollider == null) return;

            var roleState = other.GetComponent<PlayerRoleState>();
            bool unlocked = roleState != null && roleState.CurrentRole == requiredRole;
            blockingCollider.enabled = !unlocked;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || blockingCollider == null) return;
            blockingCollider.enabled = true;
        }
    }
}
