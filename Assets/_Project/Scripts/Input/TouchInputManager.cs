using UnityEngine;
using UnityEngine.EventSystems;

namespace FromCell.Input
{
    public class TouchInputManager : MonoBehaviour
    {
        public static TouchInputManager Instance { get; private set; }

        [SerializeField] Joystick joystick;
        [SerializeField] bool allowKeyboardInEditor = true;

        public bool JumpPressedThisFrame { get; private set; }
        public bool DashPressedThisFrame { get; private set; }

        // JumpHeld backs variable jump height (PlayerMovement.ApplyGravityMode): true while
        // the jump input is physically held, from either the touch button (RegisterJumpDown/
        // Up) or the keyboard fallback below - distinct from JumpPressedThisFrame, which is a
        // single-frame edge used for jump buffering, not sustained hold state.
        bool touchJumpHeld;
        public bool JumpHeld { get; private set; }

        void Awake()
        {
            Instance = this;
            EnsureEventSystem();
        }

        static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        void Update()
        {
            if (allowKeyboardInEditor && UnityEngine.Input.GetButtonDown("Jump"))
                JumpPressedThisFrame = true;

            if (allowKeyboardInEditor && UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                DashPressedThisFrame = true;

            bool keyboardJumpHeld = allowKeyboardInEditor && UnityEngine.Input.GetButton("Jump");
            JumpHeld = touchJumpHeld || keyboardJumpHeld;
        }

        void LateUpdate()
        {
            JumpPressedThisFrame = false;
            DashPressedThisFrame = false;
        }

        public void RegisterJump() => JumpPressedThisFrame = true;
        public void RegisterDash() => DashPressedThisFrame = true;
        public void RegisterJumpDown() { JumpPressedThisFrame = true; touchJumpHeld = true; }
        public void RegisterJumpUp() => touchJumpHeld = false;

        public float Horizontal
        {
            get
            {
                float touch = joystick != null ? joystick.Horizontal : 0f;
                if (float.IsNaN(touch) || float.IsInfinity(touch))
                    touch = 0f;
                float keyboard = 0f;
                if (allowKeyboardInEditor)
                {
                    keyboard = UnityEngine.Input.GetAxisRaw("Horizontal");
                    if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
                        keyboard = -1f;
                    if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
                        keyboard = 1f;
                }

                if (Mathf.Abs(touch) > 0.1f)
                    return touch;
                return keyboard;
            }
        }

        public float Vertical
        {
            get
            {
                float touch = joystick != null ? joystick.Vertical : 0f;
                if (float.IsNaN(touch) || float.IsInfinity(touch))
                    touch = 0f;
                float keyboard = 0f;
                if (allowKeyboardInEditor)
                {
                    keyboard = UnityEngine.Input.GetAxisRaw("Vertical");
                    if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
                        keyboard = -1f;
                    if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
                        keyboard = 1f;
                }

                if (Mathf.Abs(touch) > 0.1f)
                    return touch;
                return keyboard;
            }
        }
    }
}
