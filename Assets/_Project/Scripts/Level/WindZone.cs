using UnityEngine;

namespace FromCell.Level
{
    [RequireComponent(typeof(Collider2D))]
    public class WindZone : MonoBehaviour
    {
        [SerializeField] Vector2 force = new Vector2(2f, 0f);

        // Additive: lets runtime-built levels (LevelAssembler) configure this without
        // SerializedObject.FindProperty, which only works in the editor.
        public void Configure(Vector2 windForce) => force = windForce;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var rb = other.attachedRigidbody;
            if (rb == null) return;

            rb.AddForce(force, ForceMode2D.Force);
        }
    }
}
