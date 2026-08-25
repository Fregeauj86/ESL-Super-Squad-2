using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Trauma-based screen shake: Trigger() raises trauma toward 1, which decays every frame
    /// on unscaled time (so it still animates while Time.timeScale is 0 during pause/hitstop/
    /// ESL challenges). Offset is squared against trauma so small hits barely shake and big
    /// hits shake hard. CameraDirector reads CurrentOffset once per LateUpdate and applies it
    /// after CameraFollow2D's smooth-follow has already positioned the camera.
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        public static ScreenShake Instance { get; private set; }

        [SerializeField] float decayPerSecond = 2.5f;
        [SerializeField] float maxOffset = 0.35f;

        float trauma;

        public Vector3 CurrentOffset { get; private set; }

        void Awake() => Instance = this;

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            trauma = Mathf.Max(0f, trauma - decayPerSecond * Time.unscaledDeltaTime);

            if (trauma <= 0f)
            {
                CurrentOffset = Vector3.zero;
                return;
            }

            float amount = trauma * trauma * maxOffset;
            CurrentOffset = new Vector3(
                (Random.value * 2f - 1f) * amount,
                (Random.value * 2f - 1f) * amount,
                0f);
        }

        public void Trigger(float amount) => trauma = Mathf.Clamp01(Mathf.Max(trauma, amount));
    }
}
