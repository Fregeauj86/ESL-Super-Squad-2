using UnityEngine;

namespace FromCell.Art
{
    /// <summary>
    /// The editor-time level builders (FromCellSetupMenu.CreateGroundPlatform etc.) get their
    /// placeholder visuals from AssetDatabase.GetBuiltinExtraResource - a single shared white
    /// square/circle sprite, tinted per-entity via SpriteRenderer.color. That API is
    /// editor-only, so LevelAssembler (which builds levels at play time, not edit time) can't
    /// use it. This is the runtime equivalent: one shared white square sprite and one shared
    /// white circle sprite, generated once via Texture2D + Sprite.Create (both plain runtime
    /// APIs, no AssetDatabase involved), reused by every assembled entity and tinted the same
    /// way the editor-time entities already are. Not a general art system - just enough for
    /// gameplay-entity visuals until real character/tile art exists.
    /// </summary>
    public static class RuntimeShapes
    {
        const int Size = 32;
        const float PixelsPerUnit = 32f;

        static Sprite squareSprite;
        static Sprite circleSprite;

        public static Sprite Square()
        {
            if (squareSprite == null)
                squareSprite = BuildSprite(FillSquare);
            return squareSprite;
        }

        public static Sprite Circle()
        {
            if (circleSprite == null)
                circleSprite = BuildSprite(FillCircle);
            return circleSprite;
        }

        static Sprite BuildSprite(System.Action<Color32[]> fill)
        {
            var pixels = new Color32[Size * Size];
            fill(pixels);

            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            return Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), PixelsPerUnit);
        }

        static void FillSquare(Color32[] pixels)
        {
            var white = new Color32(255, 255, 255, 255);
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = white;
        }

        static void FillCircle(Color32[] pixels)
        {
            var white = new Color32(255, 255, 255, 255);
            var clear = new Color32(255, 255, 255, 0);
            float radius = Size / 2f;
            float cx = Size / 2f - 0.5f;
            float cy = Size / 2f - 0.5f;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    bool inside = (dx * dx + dy * dy) <= radius * radius;
                    pixels[y * Size + x] = inside ? white : clear;
                }
            }
        }
    }
}
