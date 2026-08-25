using FromCell.Abilities;
using FromCell.Core;
using FromCell.Player;
using FromCell.UI;
using UnityEngine;

namespace FromCell.Evolution
{
    /// <summary>
    /// Evolved from original EvolutionManager — data-driven stage application.
    /// </summary>
    public class EvolutionSystem : MonoBehaviour
    {
        [SerializeField] GameConfig gameConfig;
        [SerializeField] int startingStageIndex;

        PlayerController player;
        PlayerMovement movement;
        AbilityManager abilities;
        PlayerVisual visual;

        int currentStageIndex;

        public int CurrentStageIndex => currentStageIndex;
        public EvolutionStageData CurrentStageData => GetStageData(currentStageIndex);

        void Awake()
        {
            player = FindFirstObjectByType<PlayerController>();
            movement = FindFirstObjectByType<PlayerMovement>();
            abilities = FindFirstObjectByType<AbilityManager>();
            visual = FindFirstObjectByType<PlayerVisual>();
        }

        void Start()
        {
            if (GameFlowSystem.Instance?.Config != null)
                gameConfig = GameFlowSystem.Instance.Config;

            if (FindFirstObjectByType<Level.LevelBootstrap>() != null)
                return;

            ApplyStage(startingStageIndex);
        }

        public void SetGameConfig(GameConfig config) => gameConfig = config;

        public void AdvanceStage()
        {
            if (currentStageIndex < 9)
            {
                currentStageIndex++;
                ApplyStage(currentStageIndex);
            }
        }

        public void ApplyStage(int stageIndex)
        {
            if (movement == null)
                movement = FindFirstObjectByType<PlayerMovement>();
            if (player == null)
                player = FindFirstObjectByType<PlayerController>();
            if (abilities == null)
                abilities = FindFirstObjectByType<AbilityManager>();
            if (visual == null)
                visual = FindFirstObjectByType<PlayerVisual>();

            currentStageIndex = Mathf.Clamp(stageIndex, 0, 9);
            EvolutionStageData data = GetStageData(currentStageIndex);

            if (data != null)
                ApplyFromData(data);
            else
                ApplyFallback(stageIndex);

            var hud = FindFirstObjectByType<GameplayHUD>();
            if (hud != null)
                hud.Refresh();

            GameSignals.RaiseStageApplied(currentStageIndex);
            Debug.Log("Evolved to: " + (data != null ? data.displayName : ((EvolutionStageId)stageIndex).ToString()));
        }

        EvolutionStageData GetStageData(int index)
        {
            if (gameConfig?.evolutionStages == null) return null;
            if (index < 0 || index >= gameConfig.evolutionStages.Length) return null;
            return gameConfig.evolutionStages[index];
        }

        void ApplyFromData(EvolutionStageData data)
        {
            if (movement != null)
                movement.ApplyStageSettings(data);

            if (player != null)
                player.ApplyStageSettings(data);

            if (abilities != null)
                abilities.ApplyStageSettings(data);

            if (visual != null)
                visual.ApplyStageSettings(data);
        }

        /// <summary>
        /// Fallback values from original EvolutionManager switch blocks when ScriptableObjects are missing.
        /// </summary>
        void ApplyFallback(int stageIndex)
        {
            var stage = (EvolutionStageId)stageIndex;
            float moveSpeed = 5f;
            float jumpForce = 12f;
            bool canDoubleJump = false;
            bool canDash = false;

            switch (stage)
            {
                case EvolutionStageId.Cell:
                    moveSpeed = 2f;
                    jumpForce = 0f;
                    break;
                case EvolutionStageId.Cluster:
                    moveSpeed = 2.5f;
                    jumpForce = 0f;
                    break;
                case EvolutionStageId.Organism:
                    moveSpeed = 3f;
                    jumpForce = 6f;
                    break;
                case EvolutionStageId.Primitive:
                    moveSpeed = 3f;
                    jumpForce = 8f;
                    break;
                case EvolutionStageId.Embryo:
                    moveSpeed = 3.2f;
                    jumpForce = 8f;
                    break;
                case EvolutionStageId.Nervous:
                    moveSpeed = 4f;
                    jumpForce = 9f;
                    break;
                case EvolutionStageId.Newborn:
                    moveSpeed = 4.5f;
                    jumpForce = 10f;
                    break;
                case EvolutionStageId.Child:
                    moveSpeed = 5f;
                    jumpForce = 11f;
                    canDoubleJump = true;
                    break;
                case EvolutionStageId.Teen:
                    moveSpeed = 6f;
                    jumpForce = 11f;
                    canDoubleJump = true;
                    canDash = true;
                    break;
                case EvolutionStageId.Adult:
                    moveSpeed = 7f;
                    jumpForce = 12f;
                    canDoubleJump = true;
                    canDash = true;
                    break;
            }

            if (player != null)
            {
                player.moveSpeed = moveSpeed;
                player.jumpForce = jumpForce;
            }

            if (abilities != null)
            {
                abilities.canDoubleJump = canDoubleJump;
                abilities.canDash = canDash;
            }
        }
    }
}
