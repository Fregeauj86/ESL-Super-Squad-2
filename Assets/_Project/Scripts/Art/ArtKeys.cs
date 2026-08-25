namespace FromCell.Art
{
    /// <summary>
    /// Named sprite keys for the 15 baked character sprites, so callers never hand-type a
    /// Resources path. Matches the file names FromCellArtBaker writes under
    /// Assets/_Project/Art/Generated/Resources/FromCell/ (loaded at runtime via
    /// Resources.Load&lt;Sprite&gt;("FromCell/" + key)).
    /// </summary>
    public static class ArtKeys
    {
        // Squad heroes - one per ability tier (see the hero-tier table in the project plan).
        public const string HeroMiloMouse = "hero_milomouse";
        public const string HeroTimmyTurtle = "hero_timmyturtle";
        public const string HeroMaxElephant = "hero_maxelephant";
        public const string HeroFinnWhale = "hero_finnwhale";
        public const string HeroSkyEagle = "hero_skyeagle";
        public const string HeroDashCheetah = "hero_dashcheetah";
        public const string HeroBigTick = "hero_bigtick";
        public const string HeroDrImperfecto = "hero_drimperfecto";
        public const string HeroKingLeo = "hero_kingleo";

        // Villains - one per ESL encounter (see FromCell.ESL.EslContentCatalog).
        public const string VillainEchoFox = "villain_echofox";
        public const string VillainBuilderBear = "villain_builderbear";
        public const string VillainQuestionOwl = "villain_questionowl";
        public const string VillainConnectorSnake = "villain_connectorsnake";
        public const string VillainDebateHawk = "villain_debatehawk";
        public const string VillainTheMimic = "villain_themimic";

        /// <summary>Hero sprite key for a given ability tier (0-9), matching the hero-tier
        /// table exactly. Returns null for an out-of-range index.</summary>
        public static string HeroForStage(int stageIndex)
        {
            switch (stageIndex)
            {
                case 0: return HeroMiloMouse;      // First Steps
                case 1: return HeroMiloMouse;      // Steady Scout (same hero, 2nd tier)
                case 2: return HeroTimmyTurtle;     // Shell Guard
                case 3: return HeroMaxElephant;     // Gentle Giant
                case 4: return HeroFinnWhale;       // Deep Diver
                case 5: return HeroSkyEagle;        // Rising Wings
                case 6: return HeroDashCheetah;     // Fast Track
                case 7: return HeroBigTick;         // Power Hop
                case 8: return HeroDrImperfecto;    // Master Mentor
                case 9: return HeroKingLeo;         // Squad Champion
                default: return null;
            }
        }

        /// <summary>Villain sprite key for a villain gate's encounterId, resolved via the
        /// real ESL catalog rather than a second hardcoded mapping.</summary>
        public static string VillainForEncounter(string encounterId)
        {
            var encounter = FromCell.ESL.EslContentCatalog.Find(encounterId);
            if (encounter == null) return null;
            switch (encounter.villain)
            {
                case FromCell.ESL.VillainId.EchoFox: return VillainEchoFox;
                case FromCell.ESL.VillainId.BuilderBear: return VillainBuilderBear;
                case FromCell.ESL.VillainId.QuestionOwl: return VillainQuestionOwl;
                case FromCell.ESL.VillainId.ConnectorSnake: return VillainConnectorSnake;
                case FromCell.ESL.VillainId.DebateHawk: return VillainDebateHawk;
                case FromCell.ESL.VillainId.TheMimic: return VillainTheMimic;
                default: return null;
            }
        }

        /// <summary>All 15 keys with their source SVG path, relative to
        /// Assets/_Project/Art/SourceCharacters/ - the single list FromCellArtBaker bakes
        /// from and this class's own self-test can verify against.</summary>
        public static readonly (string key, string svgRelativePath)[] AllSourceSprites =
        {
            (HeroMiloMouse, "Squad/MiloMouse.svg"),
            (HeroTimmyTurtle, "Squad/TimmyTurtle.svg"),
            (HeroMaxElephant, "Squad/MaxElephant.svg"),
            (HeroFinnWhale, "Squad/FinnWhale.svg"),
            (HeroSkyEagle, "Squad/SkyEagle.svg"),
            (HeroDashCheetah, "Squad/DashCheetah.svg"),
            (HeroBigTick, "Squad/BigTick.svg"),
            (HeroDrImperfecto, "Squad/DrImperfecto.svg"),
            (HeroKingLeo, "Squad/KingLeo.svg"),
            (VillainEchoFox, "Villains/EchoFox.svg"),
            (VillainBuilderBear, "Villains/BuilderBear.svg"),
            (VillainQuestionOwl, "Villains/QuestionOwl.svg"),
            (VillainConnectorSnake, "Villains/ConnectorSnake.svg"),
            (VillainDebateHawk, "Villains/DebateHawk.svg"),
            (VillainTheMimic, "Villains/TheMimic.svg"),
        };
    }
}
