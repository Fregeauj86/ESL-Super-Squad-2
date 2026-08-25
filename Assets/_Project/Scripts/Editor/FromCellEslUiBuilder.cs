#if UNITY_EDITOR
using FromCell.ESL;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace FromCell.Editor
{
    /// <summary>
    /// Builds the ESL villain-challenge overlay, following this project's existing "everything
    /// is built by editor C#, wired with public fields, zero SerializedObject.FindProperty for
    /// new components" pattern. Called from FromCellSetupMenu.BuildMobileUiHierarchy() right
    /// after BuildDashCooldown(...).
    /// </summary>
    static class FromCellEslUiBuilder
    {
        public static void BuildEslChallengeOverlay(GameObject canvasGo)
        {
            var overlay = FromCellSetupMenu.CreateAnchoredPanel(
                canvasGo.transform, "EslChallengeOverlay",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(920, 560));
            var panelImage = overlay.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.05f, 0.08f, 0.12f, 0.96f);

            var villainLabel = CreateLabel(overlay, "VillainLabel", new Vector2(0, 230), new Vector2(860, 40), 28, new Color(1f, 0.6f, 0.6f));
            var promptText = CreateLabel(overlay, "PromptText", new Vector2(0, 150), new Vector2(860, 100), 24, Color.white);
            var feedbackText = CreateLabel(overlay, "FeedbackText", new Vector2(0, 60), new Vector2(860, 36), 22, new Color(0.6f, 0.95f, 0.7f));

            var optionsRoot = new GameObject("OptionsRoot", typeof(RectTransform));
            optionsRoot.transform.SetParent(overlay, false);
            var optionButtons = new Button[4];
            var optionLabels = new TextMeshProUGUI[4];
            for (int i = 0; i < 4; i++)
            {
                float y = -30 - i * 56;
                var (btn, label) = CreateEslButton(optionsRoot.transform, $"OptionButton{i}", new Vector2(0, y), new Vector2(780, 46),
                    new Color(0.2f, 0.45f, 0.55f, 0.95f));
                optionButtons[i] = btn;
                optionLabels[i] = label;
            }

            var wordBuildRoot = new GameObject("WordBuildRoot", typeof(RectTransform));
            wordBuildRoot.transform.SetParent(overlay, false);
            var answerPreviewText = CreateLabel(wordBuildRoot.transform, "AnswerPreviewText", new Vector2(0, -10), new Vector2(860, 40), 22, new Color(1f, 0.9f, 0.6f));

            var wordButtons = new Button[8];
            var wordLabels = new TextMeshProUGUI[8];
            for (int i = 0; i < 8; i++)
            {
                int col = i % 4;
                int row = i / 4;
                float x = -330 + col * 220;
                float y = -70 - row * 56;
                var (btn, label) = CreateEslButton(wordBuildRoot.transform, $"WordButton{i}", new Vector2(x, y), new Vector2(200, 44),
                    new Color(0.3f, 0.5f, 0.35f, 0.95f));
                wordButtons[i] = btn;
                wordLabels[i] = label;
            }

            var (clearBtn, _) = CreateEslButton(wordBuildRoot.transform, "ClearButton", new Vector2(-150, -200), new Vector2(160, 44),
                new Color(0.5f, 0.3f, 0.3f, 0.95f), "CLEAR");
            var (submitBtn, _) = CreateEslButton(wordBuildRoot.transform, "SubmitButton", new Vector2(150, -200), new Vector2(160, 44),
                new Color(0.3f, 0.5f, 0.3f, 0.95f), "SUBMIT");

            var controller = canvasGo.AddComponent<EslChallengeController>();
            controller.overlayRoot = overlay.gameObject;
            controller.villainLabel = villainLabel;
            controller.promptText = promptText;
            controller.feedbackText = feedbackText;
            controller.optionsRoot = optionsRoot;
            controller.optionButtons = optionButtons;
            controller.optionLabels = optionLabels;
            controller.wordBuildRoot = wordBuildRoot;
            controller.wordButtons = wordButtons;
            controller.wordLabels = wordLabels;
            controller.answerPreviewText = answerPreviewText;
            controller.clearButton = clearBtn;
            controller.submitButton = submitBtn;

            // Named zero-arg method wiring, matching the pattern every other button in this
            // project uses (JumpButton.OnJumpPressed, PauseManager.OnResume, etc.) - not a
            // captured-index lambda, which UnityEventTools.AddPersistentListener can't bind.
            UnityEventTools.AddPersistentListener(optionButtons[0].onClick, controller.OnOptionButton0);
            UnityEventTools.AddPersistentListener(optionButtons[1].onClick, controller.OnOptionButton1);
            UnityEventTools.AddPersistentListener(optionButtons[2].onClick, controller.OnOptionButton2);
            UnityEventTools.AddPersistentListener(optionButtons[3].onClick, controller.OnOptionButton3);

            UnityEventTools.AddPersistentListener(wordButtons[0].onClick, controller.OnWordButton0);
            UnityEventTools.AddPersistentListener(wordButtons[1].onClick, controller.OnWordButton1);
            UnityEventTools.AddPersistentListener(wordButtons[2].onClick, controller.OnWordButton2);
            UnityEventTools.AddPersistentListener(wordButtons[3].onClick, controller.OnWordButton3);
            UnityEventTools.AddPersistentListener(wordButtons[4].onClick, controller.OnWordButton4);
            UnityEventTools.AddPersistentListener(wordButtons[5].onClick, controller.OnWordButton5);
            UnityEventTools.AddPersistentListener(wordButtons[6].onClick, controller.OnWordButton6);
            UnityEventTools.AddPersistentListener(wordButtons[7].onClick, controller.OnWordButton7);

            UnityEventTools.AddPersistentListener(clearBtn.onClick, controller.OnClearSentence);
            UnityEventTools.AddPersistentListener(submitBtn.onClick, controller.OnSubmitSentence);

            overlay.gameObject.SetActive(false);
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 anchoredPos, Vector2 size, float fontSize, Color color)
        {
            var slot = FromCellSetupMenu.CreateAnchoredPanel(parent, name + "Slot",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPos, size);
            return FromCellSetupMenu.CreateTmpText(name, slot, string.Empty, fontSize, color);
        }

        static (Button button, TextMeshProUGUI label) CreateEslButton(
            Transform parent, string name, Vector2 anchoredPos, Vector2 size, Color bgColor, string staticLabel = null)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = bgColor;

            var label = FromCellSetupMenu.CreateTmpText("Label", go.transform, staticLabel ?? string.Empty, 22, Color.white);
            return (go.GetComponent<Button>(), label);
        }
    }
}
#endif
