using TMPro;
using UnityEngine;

namespace FromCell.UI
{
    public class TutorialBanner : MonoBehaviour
    {
        [SerializeField] GameObject root;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] float displayDuration = 4f;

        float timer;

        void Awake()
        {
            if (root != null)
                root.SetActive(false);
        }

        void Update()
        {
            if (timer <= 0f) return;

            timer -= Time.deltaTime;
            if (timer <= 0f && root != null)
                root.SetActive(false);
        }

        public void Show(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || root == null)
                return;

            root.SetActive(true);
            timer = displayDuration;

            if (messageText != null)
                messageText.text = message;
        }
    }
}
