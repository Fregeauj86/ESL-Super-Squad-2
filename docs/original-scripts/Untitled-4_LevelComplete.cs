using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.CompleteLevel();
        }
    }
}
