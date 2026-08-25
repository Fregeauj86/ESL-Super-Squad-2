using System;

namespace FromCell.ESL
{
    /// <summary>
    /// The 6 authored villain encounters, one per villain, difficulty scaling with their
    /// CEFR theming from the original character source: EchoFox (A1, word recognition) up
    /// through TheMimic (C2, mixed review). Referenced by encounterId from level content.
    /// </summary>
    public static class EslContentCatalog
    {
        public static readonly VillainEncounter[] All =
        {
            new VillainEncounter
            {
                encounterId = "echofox_intro",
                villain = VillainId.EchoFox,
                cefrLevel = CefrLevel.A1,
                displayName = "Echo Fox — Echo & Repeat",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.MultipleChoice("Which word means a happy feeling?",
                        new[] { "Sad", "Happy", "Angry" }, 1),
                    EslTask.MultipleChoice("Choose the correct word: I ___ a dog.",
                        new[] { "have", "haves", "having" }, 0),
                    EslTask.MultipleChoice("What is the opposite of 'big'?",
                        new[] { "Small", "Tall", "Fast" }, 0),
                },
            },
            new VillainEncounter
            {
                encounterId = "builderbear_sentences",
                villain = VillainId.BuilderBear,
                cefrLevel = CefrLevel.A2,
                displayName = "Builder Bear — Build a Sentence",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "cat", "The", "sleeps" }, new[] { "The", "cat", "sleeps" }),
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "like", "I", "apples" }, new[] { "I", "like", "apples" }),
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "is", "She", "happy", "very" }, new[] { "She", "is", "very", "happy" }),
                },
            },
            new VillainEncounter
            {
                encounterId = "questionowl_questions",
                villain = VillainId.QuestionOwl,
                cefrLevel = CefrLevel.B1,
                displayName = "Question Owl — Ask a Question",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.MultipleChoice("Choose the correct question: ___ is your name?",
                        new[] { "What", "Who", "Where" }, 0),
                    EslTask.MultipleChoice("Which is a correctly formed question?",
                        new[] { "You are happy?", "Are you happy?", "Happy you are?" }, 1),
                    EslTask.MultipleChoice("Complete the question: ___ do you live?",
                        new[] { "Where", "What", "Why" }, 0),
                },
            },
            new VillainEncounter
            {
                encounterId = "connectorsnake_linking",
                villain = VillainId.ConnectorSnake,
                cefrLevel = CefrLevel.B2,
                displayName = "Connector Snake — Connect Ideas",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "so", "It", "we", "home.", "rained,", "stayed" },
                        new[] { "It", "rained,", "so", "we", "stayed", "home." }),
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "tired,", "kept", "was", "but", "She", "working.", "she" },
                        new[] { "She", "was", "tired,", "but", "she", "kept", "working." }),
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "tea", "warm.", "I", "because", "like", "it", "is" },
                        new[] { "I", "like", "tea", "because", "it", "is", "warm." }),
                },
            },
            new VillainEncounter
            {
                encounterId = "debatehawk_opinions",
                villain = VillainId.DebateHawk,
                cefrLevel = CefrLevel.C1,
                displayName = "Debate Hawk — Express an Opinion",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.MultipleChoice("Which response best expresses a strong opinion?",
                        new[] { "I think maybe it's okay.", "In my opinion, this is clearly the best choice.", "It's fine I guess." }, 1),
                    EslTask.MultipleChoice("Choose the most persuasive sentence.",
                        new[] { "I believe we must act now to protect our environment.", "We could do something about the environment sometime.", "The environment is a thing that exists." }, 0),
                    EslTask.MultipleChoice("Which sentence disagrees politely?",
                        new[] { "That's wrong.", "I see your point, but I respectfully disagree.", "No way, that's silly." }, 1),
                },
            },
            new VillainEncounter
            {
                encounterId = "themimic_fluency",
                villain = VillainId.TheMimic,
                cefrLevel = CefrLevel.C2,
                displayName = "The Mimic — Master Fluency",
                requiredCorrect = 2,
                tasks = new[]
                {
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "the", "rain,", "Despite", "match", "the", "planned.", "continued", "as", "heavy" },
                        new[] { "Despite", "the", "heavy", "rain,", "the", "match", "continued", "as", "planned." }),
                    EslTask.MultipleChoice("Which sentence uses 'nevertheless' correctly?",
                        new[] { "Nevertheless, and she was tired, she smiled.", "She was tired; nevertheless, she smiled.", "She was tired nevertheless smiled." }, 1),
                    EslTask.SentenceBuild("Put the words in order.",
                        new[] { "I", "come", "known,", "have", "would", "Had", "I", "earlier." },
                        new[] { "Had", "I", "known,", "I", "would", "have", "come", "earlier." }),
                },
            },
        };

        public static VillainEncounter Find(string encounterId)
        {
            if (string.IsNullOrEmpty(encounterId)) return null;
            for (int i = 0; i < All.Length; i++)
                if (All[i].encounterId == encounterId)
                    return All[i];
            return null;
        }
    }
}
