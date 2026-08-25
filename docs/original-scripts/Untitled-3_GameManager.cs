using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public EvolutionManager evolutionManager;

    public void CompleteLevel()
    {
        evolutionManager.Evolve();

        Invoke("LoadNextLevel", 1.5f);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
