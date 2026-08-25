using UnityEngine;

namespace FromCell.Level
{
    public class PlayerRoleState : MonoBehaviour
    {
        public enum SquadRole
        {
            Default,
            Muscle,
            Nerve
        }

        public SquadRole CurrentRole { get; private set; } = SquadRole.Default;

        public void SetRole(SquadRole role)
        {
            CurrentRole = role;
        }
    }
}
