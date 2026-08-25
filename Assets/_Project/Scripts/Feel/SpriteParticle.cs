using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// One pooled particle: a plain SpriteRenderer on RuntimeShapes.Circle(), physics ported
    /// from web/js/effects.js's Effects.burst() (radial velocity + downward gravity + linear
    /// fade). The web values were tuned in pixels for a canvas with a ~30px-per-unit look, so
    /// only the physics shape is ported here, not the literal numbers - see Fx.Burst for the
    /// unit-scaled constants.
    /// </summary>
    public class SpriteParticle : MonoBehaviour
    {
        SpriteRenderer sr;
        Vector2 velocity;
        float life;
        float maxLife;
        float baseSize;

        public bool IsActive { get; private set; }

        void Awake()
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = FromCell.Art.RuntimeShapes.Circle();
            sr.sortingOrder = 50;
        }

        public void Activate(Vector3 position, Vector2 velocity, float life, Color color, float size)
        {
            transform.position = position;
            transform.localScale = Vector3.one * size;
            this.velocity = velocity;
            this.life = life;
            maxLife = life;
            baseSize = size;
            sr.color = color;
            IsActive = true;
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (!IsActive) return;

            velocity.y -= 6f * Time.deltaTime;
            transform.position += (Vector3)(velocity * Time.deltaTime);
            life -= Time.deltaTime;

            if (life <= 0f)
            {
                IsActive = false;
                gameObject.SetActive(false);
                return;
            }

            float t = Mathf.Clamp01(life / maxLife);
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, t);
            transform.localScale = Vector3.one * (baseSize * t);
        }
    }
}
