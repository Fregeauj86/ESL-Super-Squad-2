using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public enum Stage
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

    public Stage currentStage;

    public PlayerController player;
    public AbilityManager abilities;

    public void Evolve()
    {
        if ((int)currentStage < System.Enum.GetValues(typeof(Stage)).Length - 1)
        {
            currentStage++;
            ApplyStage();
        }
    }

    void ApplyStage()
    {
        switch (currentStage)
        {
            case Stage.Cell:
                abilities.canDash = false;
                abilities.canDoubleJump = false;
                player.moveSpeed = 2f;
                player.jumpForce = 0f;
                break;

            case Stage.Organism:
                player.jumpForce = 6f;
                break;

            case Stage.Primitive:
                player.jumpForce = 8f;
                break;

            case Stage.Nervous:
                player.moveSpeed = 4f;
                break;

            case Stage.Newborn:
                player.jumpForce = 10f;
                break;

            case Stage.Child:
                abilities.canDoubleJump = true;
                break;

            case Stage.Teen:
                abilities.canDash = true;
                player.moveSpeed = 6f;
                break;

            case Stage.Adult:
                abilities.canDash = true;
                abilities.canDoubleJump = true;
                player.moveSpeed = 7f;
                player.jumpForce = 12f;
                break;
        }

        Debug.Log("Evolved: " + currentStage);
    }
}
