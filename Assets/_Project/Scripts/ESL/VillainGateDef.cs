using System;
using UnityEngine;

namespace FromCell.ESL
{
    /// <summary>
    /// Where a villain gate goes in a level - position + which encounter it triggers. Plain
    /// data (matches how platforms/hazards are authored in level content), no live
    /// VillainGate reference. A future level-content builder instantiates a VillainGate
    /// GameObject per def and sets its encounterId directly.
    /// </summary>
    [Serializable]
    public class VillainGateDef
    {
        public string encounterId;
        public Vector2 position;
        public Vector2 gateSize = new Vector2(2f, 3f);
    }
}
