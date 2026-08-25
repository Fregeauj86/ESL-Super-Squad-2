using UnityEngine;

namespace FromCell.Feel
{
    /// <summary>
    /// Static facade over FxPool - ports web/js/effects.js's Effects.burst() radial-particle
    /// math (angle steps around a circle plus random speed/size/life jitter) into Unity world
    /// units. The web values were tuned in pixels for a ~30px-per-unit canvas look; speed,
    /// gravity (see SpriteParticle) and size below are that same shape divided by ~30, not a
    /// literal port of the pixel numbers.
    /// </summary>
    public static class Fx
    {
        public static void Burst(Vector3 position, Color color, int count = 8)
        {
            if (FxPool.Instance == null) return;

            for (int i = 0; i < count; i++)
            {
                float angle = (Mathf.PI * 2f * i) / count + Random.value * 0.4f;
                float speed = 2f + Random.value * 4f;
                var velocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed - 1.3f);
                float life = 0.45f + Random.value * 0.2f;
                float size = 0.1f + Random.value * 0.1f;

                FxPool.Instance.Spawn(position, velocity, life, color, size);
            }
        }
    }
}
