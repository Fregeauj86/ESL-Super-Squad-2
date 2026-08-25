using FromCell.Art;
using FromCell.Evolution;
using UnityEngine;

namespace FromCell.Player
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] CapsuleCollider2D bodyCollider;

        public void ApplyStageSettings(EvolutionStageData data)
        {
            if (spriteRenderer != null)
            {
                string heroKey = ArtKeys.HeroForStage((int)data.stageId);
                var sprite = heroKey != null ? SpriteBank.Get(heroKey) : null;
                if (sprite != null)
                {
                    // A baked character sprite is already correctly colored - no tint.
                    // paletteTint only still applies if no hero mapping/sprite is available
                    // at all, so an unmapped stage doesn't render as a flat white square.
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.color = Color.white;
                }
                else
                {
                    spriteRenderer.color = data.paletteTint;
                }
            }

            if (bodyCollider != null)
                bodyCollider.size = data.colliderSize;
        }
    }
}
