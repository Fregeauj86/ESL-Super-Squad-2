using UnityEngine;

namespace FromCell.Input
{
    public class DashButton : MonoBehaviour
    {
        public void OnDashPressed()
        {
            TouchInputManager.Instance?.RegisterDash();
        }
    }
}
