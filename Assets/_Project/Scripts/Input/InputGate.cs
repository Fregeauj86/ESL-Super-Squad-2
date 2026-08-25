namespace FromCell.Input
{
    public class InputGate : UnityEngine.MonoBehaviour
    {
        public static InputGate Instance { get; private set; }

        public bool InputEnabled { get; private set; } = true;

        void Awake()
        {
            Instance = this;
        }

        public void SetInputEnabled(bool enabled) => InputEnabled = enabled;
    }
}
