using System;

namespace FromCell.ESL
{
    /// <summary>
    /// One villain's gate content: which villain, which CEFR skill they represent, and the
    /// small task set the player must clear to get past them. Plain data (no live Unity
    /// references) so a level blueprint can reference one by encounterId instead of holding
    /// a direct reference - same declarative philosophy as the level content itself.
    /// </summary>
    [Serializable]
    public class VillainEncounter
    {
        public string encounterId;
        public VillainId villain;
        public CefrLevel cefrLevel;
        public string displayName;
        public EslTask[] tasks;
        public int requiredCorrect;
    }
}
