using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Fixed pool of SpriteParticle instances, matching this project's existing "no runtime
    /// prefab instantiation" convention - built once via AddComponent, never Instantiate.
    /// Scene-local singleton (like TouchInputManager/PauseManager), not DontDestroyOnLoad.
    /// </summary>
    public class FxPool : MonoBehaviour
    {
        public static FxPool Instance { get; private set; }

        [SerializeField] int poolSize = 64;

        SpriteParticle[] particles;
        int cursor;

        void Awake()
        {
            Instance = this;

            particles = new SpriteParticle[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go = new GameObject("FxParticle_" + i);
                go.transform.SetParent(transform, false);
                particles[i] = go.AddComponent<SpriteParticle>();
                go.SetActive(false);
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Spawn(Vector3 position, Vector2 velocity, float life, Color color, float size)
        {
            if (particles == null || particles.Length == 0) return;

            for (int i = 0; i < particles.Length; i++)
            {
                int index = (cursor + i) % particles.Length;
                if (!particles[index].IsActive)
                {
                    particles[index].Activate(position, velocity, life, color, size);
                    cursor = (index + 1) % particles.Length;
                    return;
                }
            }

            // Pool exhausted: steal the next slot in round-robin order rather than growing.
            particles[cursor].Activate(position, velocity, life, color, size);
            cursor = (cursor + 1) % particles.Length;
        }
    }
}
