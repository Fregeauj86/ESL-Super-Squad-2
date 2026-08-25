using UnityEngine;

public class JumpButton : MonoBehaviour
{
    public PlayerController player;

    public void OnJumpPressed()
    {
        player.Jump();
    }
}
