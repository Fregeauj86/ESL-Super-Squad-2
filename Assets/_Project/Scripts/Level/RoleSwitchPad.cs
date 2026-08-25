using UnityEngine;

namespace FromCell.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class RoleSwitchPad : MonoBehaviour
    {
        [SerializeField] PlayerRoleState.SquadRole role = PlayerRoleState.SquadRole.Nerve;

        // Additive: lets runtime-built levels (LevelAssembler) configure this without
        // SerializedObject.FindProperty, which only works in the editor.
        public void Configure(PlayerRoleState.SquadRole squadRole) => role = squadRole;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var roleState = other.GetComponent<PlayerRoleState>();
            if (roleState == null)
                roleState = other.gameObject.AddComponent<PlayerRoleState>();

            roleState.SetRole(role);
        }
    }
}
