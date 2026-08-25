using System.Collections.Generic;
using FromCell.Core;
using FromCell.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FromCell.ESL
{
    /// <summary>
    /// Runs one villain encounter: shows the challenge overlay, walks through its tasks one
    /// at a time (multiple choice or sentence-build), scores them, and reports pass/fail.
    ///
    /// Owns Time.timeScale + InputGate for the duration - the same primitive PauseManager
    /// already uses, which is what makes this safe against death/pause without a ref-counted
    /// InputGate: freezing timeScale also freezes every Invoke-based timer in the game
    /// (PlayerHealth's pending respawn, GameFlowSystem's pending level load), so neither can
    /// fire mid-challenge. PauseManager.TogglePause() has one added guard line checking
    /// IsActive here to close the one remaining path (the pause key/button).
    ///
    /// Per-scene singleton, rebuilt by FromCellEslUiBuilder on every scene load - NOT
    /// DontDestroyOnLoad, unlike GameFlowSystem/SaveProgressService/AudioManager.
    /// Fixed button pools only (max 4 multiple-choice options, max 8 sentence-build words -
    /// the largest authored task needs no more) - no runtime prefab instantiation, matching
    /// how every other UI piece in this project is built.
    /// </summary>
    public class EslChallengeController : MonoBehaviour
    {
        public static EslChallengeController Instance { get; private set; }

        // ---- Wired directly by FromCellEslUiBuilder - public fields, no SerializedObject ----
        public GameObject overlayRoot;
        public TextMeshProUGUI villainLabel;
        public TextMeshProUGUI promptText;
        public TextMeshProUGUI feedbackText;

        public GameObject optionsRoot;
        public Button[] optionButtons = new Button[4];
        public TextMeshProUGUI[] optionLabels = new TextMeshProUGUI[4];

        public GameObject wordBuildRoot;
        public Button[] wordButtons = new Button[8];
        public TextMeshProUGUI[] wordLabels = new TextMeshProUGUI[8];
        public TextMeshProUGUI answerPreviewText;
        public Button clearButton;
        public Button submitButton;
        // ---------------------------------------------------------------------------------

        public bool IsActive { get; private set; }

        const float FeedbackDuration = 1.1f;

        VillainEncounter activeEncounter;
        VillainGate activeGate;
        int currentTaskIndex;
        int correctCount;
        bool waitingToAdvance;
        float feedbackTimer;

        readonly List<int> selectedWordSlots = new List<int>();

        void Awake()
        {
            Instance = this;
            if (overlayRoot != null)
                overlayRoot.SetActive(false);
        }

        void Update()
        {
            if (!waitingToAdvance) return;

            feedbackTimer -= Time.unscaledDeltaTime;
            if (feedbackTimer <= 0f)
            {
                waitingToAdvance = false;
                AdvanceOrFinish();
            }
        }

        public void Begin(string encounterId, VillainGate gate)
        {
            if (IsActive) return;

            var encounter = EslContentCatalog.Find(encounterId);
            if (encounter == null || encounter.tasks == null || encounter.tasks.Length == 0)
            {
                Debug.LogWarning($"EslChallengeController: unknown or empty encounter '{encounterId}'.");
                return;
            }

            activeEncounter = encounter;
            activeGate = gate;
            currentTaskIndex = 0;
            correctCount = 0;
            waitingToAdvance = false;
            IsActive = true;

            Time.timeScale = 0f;
            InputGate.Instance?.SetInputEnabled(false);

            if (overlayRoot != null)
                overlayRoot.SetActive(true);
            if (villainLabel != null)
                villainLabel.text = encounter.displayName;

            GameSignals.RaiseChallengeStarted(encounterId);
            ShowCurrentTask();
        }

        void ShowCurrentTask()
        {
            var task = activeEncounter.tasks[currentTaskIndex];
            if (feedbackText != null)
                feedbackText.text = string.Empty;
            if (promptText != null)
                promptText.text = task.prompt;

            bool isMultipleChoice = task.kind == EslTaskKind.MultipleChoice;
            if (optionsRoot != null) optionsRoot.SetActive(isMultipleChoice);
            if (wordBuildRoot != null) wordBuildRoot.SetActive(!isMultipleChoice);

            if (isMultipleChoice)
                ShowMultipleChoice(task);
            else
                ShowSentenceBuild(task);
        }

        void ShowMultipleChoice(EslTask task)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                bool inUse = task.options != null && i < task.options.Length;
                if (optionButtons[i] != null)
                    optionButtons[i].gameObject.SetActive(inUse);
                if (inUse && optionLabels[i] != null)
                    optionLabels[i].text = task.options[i];
            }
        }

        void ShowSentenceBuild(EslTask task)
        {
            selectedWordSlots.Clear();
            if (answerPreviewText != null)
                answerPreviewText.text = string.Empty;

            for (int i = 0; i < wordButtons.Length; i++)
            {
                bool inUse = task.wordBank != null && i < task.wordBank.Length;
                if (wordButtons[i] != null)
                    wordButtons[i].gameObject.SetActive(inUse);
                if (inUse && wordLabels[i] != null)
                    wordLabels[i].text = task.wordBank[i];
            }

            if (submitButton != null)
                submitButton.gameObject.SetActive(false);
        }

        // ---- Multiple-choice button hooks (bound by the editor builder, one per pool slot) ----
        public void OnOptionButton0() => OnOptionSelected(0);
        public void OnOptionButton1() => OnOptionSelected(1);
        public void OnOptionButton2() => OnOptionSelected(2);
        public void OnOptionButton3() => OnOptionSelected(3);

        void OnOptionSelected(int index)
        {
            if (!IsActive || waitingToAdvance) return;
            var task = activeEncounter.tasks[currentTaskIndex];
            if (task.kind != EslTaskKind.MultipleChoice) return;
            if (task.options == null || index < 0 || index >= task.options.Length) return;

            bool correct = index == task.correctOptionIndex;
            ShowFeedback(correct, task.options[task.correctOptionIndex]);
        }

        // ---- Sentence-build button hooks ----
        public void OnWordButton0() => OnWordSelected(0);
        public void OnWordButton1() => OnWordSelected(1);
        public void OnWordButton2() => OnWordSelected(2);
        public void OnWordButton3() => OnWordSelected(3);
        public void OnWordButton4() => OnWordSelected(4);
        public void OnWordButton5() => OnWordSelected(5);
        public void OnWordButton6() => OnWordSelected(6);
        public void OnWordButton7() => OnWordSelected(7);

        void OnWordSelected(int index)
        {
            if (!IsActive || waitingToAdvance) return;
            var task = activeEncounter.tasks[currentTaskIndex];
            if (task.kind != EslTaskKind.SentenceBuild) return;
            if (task.wordBank == null || index < 0 || index >= task.wordBank.Length) return;
            if (selectedWordSlots.Contains(index)) return;

            selectedWordSlots.Add(index);
            if (wordButtons[index] != null)
                wordButtons[index].gameObject.SetActive(false);

            RefreshSentencePreview(task);

            if (submitButton != null)
                submitButton.gameObject.SetActive(selectedWordSlots.Count == task.wordBank.Length);
        }

        void RefreshSentencePreview(EslTask task)
        {
            if (answerPreviewText == null) return;
            var words = new string[selectedWordSlots.Count];
            for (int i = 0; i < selectedWordSlots.Count; i++)
                words[i] = task.wordBank[selectedWordSlots[i]];
            answerPreviewText.text = string.Join(" ", words);
        }

        public void OnClearSentence()
        {
            if (!IsActive || waitingToAdvance) return;
            var task = activeEncounter.tasks[currentTaskIndex];
            if (task.kind != EslTaskKind.SentenceBuild) return;
            ShowSentenceBuild(task);
        }

        public void OnSubmitSentence()
        {
            if (!IsActive || waitingToAdvance) return;
            var task = activeEncounter.tasks[currentTaskIndex];
            if (task.kind != EslTaskKind.SentenceBuild || task.correctOrder == null) return;
            if (selectedWordSlots.Count != task.wordBank.Length) return;

            bool correct = selectedWordSlots.Count == task.correctOrder.Length;
            if (correct)
            {
                for (int i = 0; i < selectedWordSlots.Count; i++)
                {
                    if (task.wordBank[selectedWordSlots[i]] != task.correctOrder[i])
                    {
                        correct = false;
                        break;
                    }
                }
            }

            ShowFeedback(correct, string.Join(" ", task.correctOrder));
        }

        void ShowFeedback(bool correct, string correctAnswerText)
        {
            if (correct)
                correctCount++;

            if (feedbackText != null)
                feedbackText.text = correct ? "Correct!" : $"Not quite. Correct: {correctAnswerText}";

            waitingToAdvance = true;
            feedbackTimer = FeedbackDuration;
        }

        void AdvanceOrFinish()
        {
            currentTaskIndex++;
            if (activeEncounter != null && currentTaskIndex < activeEncounter.tasks.Length)
            {
                ShowCurrentTask();
                return;
            }

            Finish();
        }

        void Finish()
        {
            bool passed = activeEncounter != null && correctCount >= activeEncounter.requiredCorrect;
            string encounterId = activeEncounter != null ? activeEncounter.encounterId : string.Empty;
            int totalTasks = activeEncounter != null ? activeEncounter.tasks.Length : 0;

            if (passed && activeGate != null)
                activeGate.MarkPassed();

            GameSignals.RaiseChallengeCompleted(encounterId, passed, correctCount, totalTasks);

            IsActive = false;
            activeEncounter = null;
            activeGate = null;

            if (overlayRoot != null)
                overlayRoot.SetActive(false);

            Time.timeScale = 1f;
            InputGate.Instance?.SetInputEnabled(true);
        }
    }
}
