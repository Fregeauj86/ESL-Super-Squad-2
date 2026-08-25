using FromCell.Abilities;
using UnityEngine;

namespace FromCell.UI
{
    public class AbilityButtonVisibility : MonoBehaviour
    {
        [SerializeField] GameObject dashButtonRoot;
        [SerializeField] float pollInterval = 0.25f;

        AbilityManager abilities;
        float timer;

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            timer = pollInterval;

            if (abilities == null)
                abilities = FindAnyObjectByType<AbilityManager>();

            if (dashButtonRoot != null && abilities != null)
                dashButtonRoot.SetActive(abilities.canDash);
        }
    }
}
