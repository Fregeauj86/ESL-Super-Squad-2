using FromCell.Evolution;
using FromCell.Input;
using FromCell.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.Core
{
    /// <summary>
    /// Evolved from original GameManager — handles level completion and scene flow.
    /// </summary>
    public class GameFlowSystem : MonoBehaviour
    {
        public static GameFlowSystem Instance { get; private set; }

        [SerializeField] GameConfig gameConfig;
        [SerializeField] float evolutionDelay = 1.5f;
        [SerializeField] string creditsSceneName = "_Credits";

        EvolutionSystem evolutionSystem;
        EvolutionPresenter evolutionPresenter;
        GameFlowState state = GameFlowState.Playing;
        int currentLevelIndex;
        bool levelCompleteTriggered;

        public GameConfig Config => gameConfig;
        public int CurrentLevelIndex => currentLevelIndex;
        public GameFlowState State => state;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        void Start()
        {
            RefreshSceneReferences();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode) => RefreshSceneReferences();

        void RefreshSceneReferences()
        {
            evolutionSystem = FindFirstObjectByType<EvolutionSystem>();
            evolutionPresenter = FindFirstObjectByType<EvolutionPresenter>();
            ResolveLevelIndexFromScene();
        }

        void ResolveLevelIndexFromScene()
        {
            if (gameConfig == null || gameConfig.levels == null) return;

            string activeScene = SceneManager.GetActiveScene().name;
            for (int i = 0; i < gameConfig.levels.Length; i++)
            {
                if (gameConfig.levels[i] != null && gameConfig.levels[i].sceneName == activeScene)
                {
                    currentLevelIndex = i;
                    return;
                }
            }
        }

        public void SetGameConfig(GameConfig config) => gameConfig = config;

        public void StartNewGame()
        {
            SaveProgressService.Instance?.ClearSave();
            currentLevelIndex = 0;
            LoadLevel(0, applyStage: true);
        }

        public void ContinueGame()
        {
            int last = SaveProgressService.Instance?.GetLastCompletedLevel() ?? -1;
            currentLevelIndex = Mathf.Clamp(last + 1, 0, 9);
            LoadLevel(currentLevelIndex, applyStage: true);
        }

        /// <summary>Jumps straight to an arbitrary level, bypassing the "next after last
        /// completed" progression ContinueGame uses - for the level-select screen. Additive:
        /// reuses the same private LoadLevel every other entry point already goes through.</summary>
        public void PlayLevel(int levelIndex)
        {
            LoadLevel(levelIndex, applyStage: true);
        }

        public void ReloadCurrentLevel()
        {
            levelCompleteTriggered = false;
            state = GameFlowState.Playing;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMainMenu()
        {
            levelCompleteTriggered = false;
            state = GameFlowState.MainMenu;
            SceneManager.LoadScene("_MainMenu");
        }

        public void CompleteLevel()
        {
            if (levelCompleteTriggered || state == GameFlowState.LevelComplete) return;
            levelCompleteTriggered = true;
            state = GameFlowState.LevelComplete;

            InputGate.Instance?.SetInputEnabled(false);

            if (evolutionSystem == null)
                evolutionSystem = FindFirstObjectByType<EvolutionSystem>();

            bool isFinalLevel = gameConfig == null ||
                                currentLevelIndex >= gameConfig.levels.Length - 1;

            if (!isFinalLevel)
                evolutionSystem?.AdvanceStage();

            SaveProgressService.Instance?.SaveProgress(
                currentLevelIndex,
                evolutionSystem != null ? evolutionSystem.CurrentStageIndex : 0);

            var runTracker = LevelRunTracker.Instance;
            SaveProfile.Instance?.RecordLevelComplete(
                currentLevelIndex,
                runTracker != null ? runTracker.ElapsedTime : 0f,
                runTracker != null ? runTracker.DeathCount : 0);

            if (evolutionPresenter == null)
                evolutionPresenter = FindFirstObjectByType<EvolutionPresenter>();

            evolutionPresenter?.ShowEvolution(evolutionSystem?.CurrentStageData);

            Invoke(nameof(LoadNextLevel), evolutionDelay);
        }

        void LoadNextLevel()
        {
            levelCompleteTriggered = false;
            InputGate.Instance?.SetInputEnabled(true);

            if (gameConfig == null || gameConfig.levels == null)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                return;
            }

            if (currentLevelIndex >= gameConfig.levels.Length - 1)
            {
                state = GameFlowState.Credits;
                SceneManager.LoadScene(creditsSceneName);
                return;
            }

            currentLevelIndex++;
            LoadLevel(currentLevelIndex, applyStage: false);
        }

        void LoadLevel(int index, bool applyStage)
        {
            state = GameFlowState.LevelLoad;

            if (gameConfig?.levels == null || index < 0 || index >= gameConfig.levels.Length)
                return;

            LevelData level = gameConfig.levels[index];
            if (level == null || string.IsNullOrEmpty(level.sceneName))
                return;

            currentLevelIndex = index;
            SceneManager.LoadScene(level.sceneName);

            if (applyStage && evolutionSystem != null)
                evolutionSystem.ApplyStage((int)level.stageId);
        }
    }
}
