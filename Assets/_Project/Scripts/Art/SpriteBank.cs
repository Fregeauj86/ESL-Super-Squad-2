using System.Collections.Generic;
using UnityEngine;

namespace FromCell.Art
{
    /// <summary>
    /// Runtime sprite lookup by key. Tries the baked PNG first (written by the editor-only
    /// FromCellArtBaker into a Resources folder); if it's missing - not yet baked, or the
    /// key is simply unknown - falls back to a plain RuntimeShapes placeholder so a missing
    /// sprite can never produce a broken/invisible-object scene. Results are cached so each
    /// key only pays the lookup cost once.
    /// </summary>
    public static class SpriteBank
    {
        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        static readonly HashSet<string> warnedMissing = new HashSet<string>();

        public static Sprite Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return RuntimeShapes.Square();

            if (cache.TryGetValue(key, out var cached))
                return cached;

            var loaded = Resources.Load<Sprite>("FromCell/" + key);
            if (loaded != null)
            {
                cache[key] = loaded;
                return loaded;
            }

            if (warnedMissing.Add(key))
                Debug.LogWarning($"SpriteBank: no baked sprite for '{key}' (run From Cell > Setup > Bake Character Art) - using a placeholder.");

            var fallback = RuntimeShapes.Square();
            cache[key] = fallback;
            return fallback;
        }
    }
}
