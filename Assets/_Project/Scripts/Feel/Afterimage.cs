using FromCell.Core;
using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Dash ghost trail: on GameSignals.PlayerDashed, stamps a copy of the player's current
    /// sprite that fades out over a short lifetime. Uses its own small fixed pool (separate
    /// from FxPool) because it needs the player's actual sprite/flip state, not a plain circle.
    /// sourceRenderer is wired directly in code (CreatePlayerGameObject), not via
    /// SerializedObject, per this project's rule for new components.
    /// </summary>
    public class Afterimage : MonoBehaviour
    {
        public SpriteRenderer sourceRenderer;

        [SerializeField] int poolSize = 4;
        [SerializeField] float life = 0.25f;
        [SerializeField] Color tint = new Color(1f, 1f, 1f, 0.5f);

        SpriteRenderer[] ghosts;
        float[] remaining;
        int cursor;

        void Awake()
        {
            ghosts = new SpriteRenderer[poolSize];
            remaining = new float[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go = new GameObject("Afterimage_" + i);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.enabled = false;
                ghosts[i] = sr;
            }
        }

        void OnEnable() => GameSignals.PlayerDashed += OnDashed;
        void OnDisable() => GameSignals.PlayerDashed -= OnDashed;

        void OnDashed(string context)
        {
            if (sourceRenderer == null) return;

            int index = cursor;
            cursor = (cursor + 1) % ghosts.Length;

            var sr = ghosts[index];
            sr.sprite = sourceRenderer.sprite;
            sr.flipX = sourceRenderer.flipX;
            sr.sortingOrder = sourceRenderer.sortingOrder - 1;
            sr.transform.position = sourceRenderer.transform.position;
            sr.transform.localScale = sourceRenderer.transform.lossyScale;
            sr.color = tint;
            sr.enabled = true;
            remaining[index] = life;
        }

        void Update()
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (remaining[i] <= 0f) continue;

                remaining[i] -= Time.deltaTime;
                float t = Mathf.Clamp01(remaining[i] / life);
                ghosts[i].color = new Color(tint.r, tint.g, tint.b, tint.a * t);

                if (remaining[i] <= 0f)
                    ghosts[i].enabled = false;
            }
        }
    }
}
