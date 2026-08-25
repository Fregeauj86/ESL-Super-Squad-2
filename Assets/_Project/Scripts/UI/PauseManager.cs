using FromCell.Core;
using FromCell.ESL;
using FromCell.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.UI
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager Instance { get; private set; }

        [SerializeField] GameObject pausePanel;
        [SerializeField] string mainMenuScene = "_MainMenu";

        bool isPaused;

        public bool IsPaused => isPaused;

        void Awake()
        {
            Instance = this;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        public void TogglePause()
        {
            // A challenge overlay owns Time.timeScale/InputGate for its duration and can't
            // be paused mid-way - there's no state to reconcile, it simply can't happen.
            if (EslChallengeController.Instance != null && EslChallengeController.Instance.IsActive)
                return;

            isPaused = !isPaused;

            if (pausePanel != null)
                pausePanel.SetActive(isPaused);

            Time.timeScale = isPaused ? 0f : 1f;
            InputGate.Instance?.SetInputEnabled(!isPaused);
        }

        public void OnResume()
        {
            if (isPaused)
                TogglePause();
        }

        public void OnRestartLevel()
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (pausePanel != null)
                pausePanel.SetActive(false);
            InputGate.Instance?.SetInputEnabled(true);

            if (GameFlowSystem.Instance != null)
                GameFlowSystem.Instance.ReloadCurrentLevel();
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnQuitToMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;
            InputGate.Instance?.SetInputEnabled(true);

            if (GameFlowSystem.Instance != null)
                GameFlowSystem.Instance.ReturnToMainMenu();
            else
                SceneManager.LoadScene(mainMenuScene);
        }
    }
}
