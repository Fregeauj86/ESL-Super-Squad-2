using System;

namespace FromCell.ESL
{
    /// <summary>
    /// One question inside a VillainEncounter. Plain data, no Unity object references, so it
    /// can be authored and statically validated (EslContentValidator) without Unity.
    ///
    /// MultipleChoice uses options/correctOptionIndex; SentenceBuild uses wordBank/correctOrder.
    /// A task only fills in the fields for its own kind - the other kind's fields stay null/empty.
    /// </summary>
    [Serializable]
    public class EslTask
    {
        public EslTaskKind kind;
        public string prompt;

        // MultipleChoice
        public string[] options;
        public int correctOptionIndex;

        // SentenceBuild - correctOrder must be assemblable from wordBank (see EslContentValidator)
        public string[] wordBank;
        public string[] correctOrder;

        public static EslTask MultipleChoice(string prompt, string[] options, int correctOptionIndex) =>
            new EslTask
            {
                kind = EslTaskKind.MultipleChoice,
                prompt = prompt,
                options = options,
                correctOptionIndex = correctOptionIndex,
            };

        public static EslTask SentenceBuild(string prompt, string[] wordBank, string[] correctOrder) =>
            new EslTask
            {
                kind = EslTaskKind.SentenceBuild,
                prompt = prompt,
                wordBank = wordBank,
                correctOrder = correctOrder,
            };
    }
}
