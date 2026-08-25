using System.Collections.Generic;
using System.Linq;

namespace FromCell.ESL
{
    /// <summary>
    /// Pure data validation for ESL content - no MonoBehaviour/GameObject/scene access, so
    /// this runs under plain `dotnet` (the Tools.Validate console project) exactly like the
    /// level-blueprint validator, without needing Unity installed. Vector2/Color-only Unity
    /// value types are fine here (same rule the blueprint data follows) - what's excluded is
    /// anything that needs a live scene.
    /// </summary>
    public static class EslContentValidator
    {
        public static (List<string> errors, List<string> warnings) Validate(VillainEncounter[] catalog)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            if (catalog == null || catalog.Length == 0)
            {
                errors.Add("EslContentCatalog.All is empty.");
                return (errors, warnings);
            }

            var seenIds = new HashSet<string>();
            var villainToLevel = new Dictionary<VillainId, CefrLevel>();

            foreach (var encounter in catalog)
            {
                string tag = string.IsNullOrEmpty(encounter.encounterId) ? "(no id)" : encounter.encounterId;

                if (string.IsNullOrEmpty(encounter.encounterId))
                    errors.Add($"Encounter has an empty encounterId ({encounter.displayName}).");
                else if (!seenIds.Add(encounter.encounterId))
                    errors.Add($"Duplicate encounterId '{encounter.encounterId}'.");

                if (villainToLevel.TryGetValue(encounter.villain, out var existingLevel))
                {
                    if (existingLevel != encounter.cefrLevel)
                        errors.Add($"[{tag}] Villain {encounter.villain} has inconsistent CEFR levels: {existingLevel} vs {encounter.cefrLevel}.");
                }
                else
                {
                    villainToLevel[encounter.villain] = encounter.cefrLevel;
                }

                if (encounter.tasks == null || encounter.tasks.Length == 0)
                {
                    errors.Add($"[{tag}] has no tasks.");
                    continue;
                }

                if (encounter.requiredCorrect < 1 || encounter.requiredCorrect > encounter.tasks.Length)
                    errors.Add($"[{tag}] requiredCorrect ({encounter.requiredCorrect}) must be between 1 and {encounter.tasks.Length}.");

                bool longSentenceOnEasyTier = false;
                bool shortOptionsOnHardTier = false;

                for (int i = 0; i < encounter.tasks.Length; i++)
                {
                    var task = encounter.tasks[i];
                    string taskTag = $"{tag} task[{i}]";

                    if (task.kind == EslTaskKind.MultipleChoice)
                    {
                        ValidateMultipleChoice(task, taskTag, errors);
                        if ((encounter.cefrLevel == CefrLevel.C1 || encounter.cefrLevel == CefrLevel.C2) &&
                            (task.options == null || task.options.Length < 3))
                            shortOptionsOnHardTier = true;
                    }
                    else if (task.kind == EslTaskKind.SentenceBuild)
                    {
                        ValidateSentenceBuild(task, taskTag, errors);
                        if ((encounter.cefrLevel == CefrLevel.A1 || encounter.cefrLevel == CefrLevel.A2) &&
                            task.wordBank != null && task.wordBank.Length > 5)
                            longSentenceOnEasyTier = true;
                    }
                }

                if (longSentenceOnEasyTier)
                    warnings.Add($"[{tag}] has a sentence-build task longer than ~5 tokens for an {encounter.cefrLevel} tier.");
                if (shortOptionsOnHardTier)
                    warnings.Add($"[{tag}] has a multiple-choice task with fewer than 3 options for a {encounter.cefrLevel} tier.");
            }

            return (errors, warnings);
        }

        static void ValidateMultipleChoice(EslTask task, string tag, List<string> errors)
        {
            if (task.options == null || task.options.Length < 2)
            {
                errors.Add($"{tag}: MultipleChoice needs at least 2 options.");
                return;
            }

            if (task.correctOptionIndex < 0 || task.correctOptionIndex >= task.options.Length)
                errors.Add($"{tag}: correctOptionIndex {task.correctOptionIndex} out of range.");

            var normalized = task.options.Select(o => (o ?? string.Empty).Trim().ToLowerInvariant()).ToList();
            if (normalized.Distinct().Count() != normalized.Count)
                errors.Add($"{tag}: MultipleChoice has duplicate options.");
        }

        static void ValidateSentenceBuild(EslTask task, string tag, List<string> errors)
        {
            if (task.wordBank == null || task.wordBank.Length < 2)
            {
                errors.Add($"{tag}: SentenceBuild needs a wordBank of at least 2 tokens.");
                return;
            }

            if (task.correctOrder == null || task.correctOrder.Length < 2)
            {
                errors.Add($"{tag}: SentenceBuild needs a correctOrder of at least 2 tokens.");
                return;
            }

            // correctOrder must be assemblable from wordBank by MULTISET (handles repeated
            // words like "the" correctly, not just distinct-set membership).
            var remaining = task.wordBank.ToList();
            foreach (var word in task.correctOrder)
            {
                int idx = remaining.IndexOf(word);
                if (idx < 0)
                {
                    errors.Add($"{tag}: correctOrder token '{word}' is not consumable from wordBank.");
                    return;
                }
                remaining.RemoveAt(idx);
            }

            if (task.correctOrder.Length != task.wordBank.Length)
                errors.Add($"{tag}: correctOrder uses {task.correctOrder.Length} of {task.wordBank.Length} wordBank tokens - every token must be used exactly once.");
        }

        /// <summary>
        /// Cross-checks level-authored villain gates against the catalog. Takes the encounter
        /// IDs referenced by gates directly (rather than a VillainGateDef[]) so this has no
        /// dependency on the level-blueprint types, which don't exist yet.
        /// </summary>
        public static List<string> ValidateGateReferences(IEnumerable<string> referencedEncounterIds, VillainEncounter[] catalog)
        {
            var errors = new List<string>();
            var known = new HashSet<string>(catalog.Select(e => e.encounterId));
            foreach (var id in referencedEncounterIds)
            {
                if (!known.Contains(id))
                    errors.Add($"Villain gate references unknown encounterId '{id}'.");
            }
            return errors;
        }
    }
}
