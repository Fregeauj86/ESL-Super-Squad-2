using FromCell.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] GameObject continueButtonRoot;
        [SerializeField] string firstLevelScene = "Level_01_FirstSteps";

        void Start()
        {
            bool hasSave = SaveProgressService.Instance != null && SaveProgressService.Instance.HasSave();
            if (continueButtonRoot != null)
                continueButtonRoot.SetActive(hasSave);
        }

        public void OnNewGame()
        {
            if (GameFlowSystem.Instance != null)
            {
                GameFlowSystem.Instance.StartNewGame();
                return;
            }

            SaveProgressService.Instance?.ClearSave();
            SceneManager.LoadScene(firstLevelScene);
        }

        public void OnContinue()
        {
            if (GameFlowSystem.Instance != null)
            {
                GameFlowSystem.Instance.ContinueGame();
                return;
            }

            SceneManager.LoadScene(firstLevelScene);
        }

        public void OnLevelSelect()
        {
            SceneManager.LoadScene("_LevelSelect");
        }

        public void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
