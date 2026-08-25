using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public enum EvolutionStage
    {
        Cell,
        Cluster,
        Organism,
        Primitive,
        Embryo,
        Nervous,
        Newborn,
        Child,
        Teen,
        Adult
    }

    public EvolutionStage currentStage = EvolutionStage.Cell;

    public PlayerController player;

    void Start()
    {
        ApplyStage();
    }

    public void Evolve()
    {
        if ((int)currentStage < System.Enum.GetValues(typeof(EvolutionStage)).Length - 1)
        {
            currentStage++;
            ApplyStage();
        }
    }

    void ApplyStage()
    {
        switch (currentStage)
        {
            case EvolutionStage.Cell:
                player.moveSpeed = 2f;
                player.jumpForce = 0f;
                break;

            case EvolutionStage.Cluster:
                player.moveSpeed = 2.5f;
                break;

            case EvolutionStage.Organism:
                player.moveSpeed = 3f;
                player.jumpForce = 6f;
                break;

            case EvolutionStage.Primitive:
                player.jumpForce = 8f;
                break;

            case EvolutionStage.Embryo:
                player.moveSpeed = 3.2f;
                break;

            case EvolutionStage.Nervous:
                player.moveSpeed = 4f;
                break;

            case EvolutionStage.Newborn:
                player.moveSpeed = 4.5f;
                player.jumpForce = 10f;
                break;

            case EvolutionStage.Child:
                player.jumpForce = 11f;
                break;

            case EvolutionStage.Teen:
                player.moveSpeed = 6f;
                break;

            case EvolutionStage.Adult:
                player.moveSpeed = 6f;
                player.jumpForce = 12f;
                break;
        }

        Debug.Log("Evolved to: " + currentStage);
    }
}
