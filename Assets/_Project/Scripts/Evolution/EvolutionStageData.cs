using FromCell.Core;
using UnityEngine;

namespace FromCell.Evolution
{
    [CreateAssetMenu(fileName = "EvolutionStage", menuName = "From Cell/Evolution Stage")]
    public class EvolutionStageData : ScriptableObject
    {
        [Header("Identity")]
        public EvolutionStageId stageId;
        public string displayName;
        [TextArea] public string humorLine;

        [Header("Movement")]
        public MovementMode movementMode = MovementMode.Walk;
        public float moveSpeed = 5f;
        public float acceleration = 50f;
        public float jumpForce = 12f;
        public float gravityScale = 3f;
        [Range(0f, 1f)] public float airControl = 1f;
        public bool canJump = true;
        public Vector2 colliderSize = new Vector2(0.8f, 1.2f);

        [Header("Abilities")]
        public bool canDoubleJump;
        public bool canDash;
        public float dashForce = 15f;
        public float doubleJumpForce = 10f;

        [Header("Presentation")]
        public Color paletteTint = Color.white;
        public float cameraZoom = 5f;
    }
}
