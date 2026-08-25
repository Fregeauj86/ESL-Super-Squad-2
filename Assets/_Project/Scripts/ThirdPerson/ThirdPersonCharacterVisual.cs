using FromCell.Art;
using FromCell.Core;
using UnityEngine;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Displays one of the authored From Cell SVG characters in a 3D scene. The art remains a
    /// crisp, alpha-tested sprite card while its actor still has a 3D collider and NavMesh
    /// movement. Stage and encounter keys come from ArtKeys, so the full existing roster works
    /// without duplicating SVG files or hardcoding texture paths.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ThirdPersonCharacterVisual : MonoBehaviour
    {
        [SerializeField] string spriteKey = ArtKeys.HeroMiloMouse;
        [SerializeField] float desiredHeight = 2.2f;
        [SerializeField] bool followEvolutionStage;
        [SerializeField] bool billboardToCamera = true;

        SpriteRenderer spriteRenderer;
        Camera targetCamera;
        string activeKey;

        public string SpriteKey => activeKey;
        public SpriteRenderer Renderer => spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            targetCamera = Camera.main;
            ApplySprite(spriteKey);
        }

        void OnEnable()
        {
            GameSignals.StageApplied += OnStageApplied;
        }

        void OnDisable()
        {
            GameSignals.StageApplied -= OnStageApplied;
        }

        void LateUpdate()
        {
            if (!billboardToCamera)
                return;

            if (targetCamera == null)
                targetCamera = Camera.main;
            if (targetCamera == null)
                return;

            Vector3 toCamera = targetCamera.transform.position - transform.position;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
        }

        public void SetSpriteKey(string key)
        {
            spriteKey = key;
            ApplySprite(key);
        }

        void OnStageApplied(int stageIndex)
        {
            if (followEvolutionStage)
                ApplySprite(ArtKeys.HeroForStage(stageIndex));
        }

        void ApplySprite(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            var sprite = SpriteBank.Get(key);
            if (sprite == null)
                return;

            spriteRenderer.sprite = sprite;
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
            spriteRenderer.sortingOrder = 10;
            activeKey = key;

            float sourceHeight = sprite.bounds.size.y;
            if (sourceHeight > 0.001f)
                transform.localScale = Vector3.one * (desiredHeight / sourceHeight);
        }
    }
}