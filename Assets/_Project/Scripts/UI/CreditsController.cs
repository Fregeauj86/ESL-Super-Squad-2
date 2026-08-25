using UnityEngine;
using UnityEngine.SceneManagement;

namespace FromCell.UI
{
    public class CreditsController : MonoBehaviour
    {
        [SerializeField] string mainMenuScene = "_MainMenu";

        public void OnBackToMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
