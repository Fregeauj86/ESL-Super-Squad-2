using FromCell.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] string mainMenuScene = "_MainMenu";
        [SerializeField] GameConfig gameConfig;

        void Awake()
        {
            EnsureServices();

            if (SceneManager.GetActiveScene().name == "_Boot")
                SceneManager.LoadScene(mainMenuScene);
        }

        void EnsureServices()
        {
            if (SaveProgressService.Instance == null)
            {
                var saveGo = new GameObject("SaveProgressService");
                saveGo.AddComponent<SaveProgressService>();
            }

            if (GameFlowSystem.Instance == null)
            {
                var flowGo = new GameObject("GameFlowSystem");
                var flow = flowGo.AddComponent<GameFlowSystem>();
                if (gameConfig != null)
                    flow.SetGameConfig(gameConfig);
            }
            else if (gameConfig != null)
            {
                GameFlowSystem.Instance.SetGameConfig(gameConfig);
            }

            if (AudioManager.Instance == null)
            {
                var audioGo = new GameObject("AudioManager");
                audioGo.AddComponent<AudioManager>();
            }
        }
    }
}
