using FromCell.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace FromCell.UI
{
    public class DashCooldownUI : MonoBehaviour
    {
        [SerializeField] Image fillImage;
        [SerializeField] GameObject root;

        AbilityManager abilities;

        void Update()
        {
            if (abilities == null)
                abilities = FindFirstObjectByType<AbilityManager>();

            if (abilities == null) return;

            if (root != null)
                root.SetActive(abilities.canDash);

            if (!abilities.canDash || fillImage == null) return;

            fillImage.fillAmount = abilities.DashReadyNormalized;
        }
    }
}
