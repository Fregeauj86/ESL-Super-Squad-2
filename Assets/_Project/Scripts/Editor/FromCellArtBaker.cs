#if UNITY_EDITOR
using System.IO;
using FromCell.Art;
using UnityEditor;
using UnityEngine;

namespace FromCell.Editor
{
    /// <summary>
    /// Bakes the 15 character SVGs (Assets/_Project/Art/SourceCharacters/) into real Sprite
    /// assets via MiniSvgRasterizer - the only Unity-API-touching step in the art pipeline;
    /// the rasterizer itself is plain C# and already verified separately. Writes into a
    /// Resources folder so SpriteBank can load them at runtime with zero further editor
    /// interaction, and re-running this is always safe (deterministic rasterizer output,
    /// files are simply overwritten).
    ///
    /// Correct import order (a fresh PNG has no Sprite sub-asset until SaveAndReimport runs):
    /// write bytes -> ImportAsset(sync) -> configure TextureImporter -> SaveAndReimport.
    /// </summary>
    static class FromCellArtBaker
    {
        const string OutRoot = "Assets/_Project/Art/Generated/Resources/FromCell";
        const string SourceRoot = "Assets/_Project/Art/SourceCharacters";
        // The 3D conversion displays the same SVG art at a much larger on-screen size than
        // the 2D prototype. A 256px bake keeps eyes, mouths, and thin outlines clean on a
        // perspective camera without making the Android character atlas unnecessarily large.
        const int OutputSize = 256;

        // Deliberately NOT 32 (the project-wide PPU used for tiles/pickups/RuntimeShapes).
        // The player's SpriteRenderer lives directly on the player root GameObject today
        // (Phase 3's "move it to a child Visual object" refactor hasn't happened yet), so
        // PlayerVisual cannot safely rescale it via transform.localScale without also
        // shrinking GroundCheck's child offset and the collider. Baking at this PPU instead
        // makes a 128px character's NATURAL size ~1.16 units - already sane for direct use
        // on the player root, no runtime scale needed. Villain gates aren't affected by this
        // choice either way - LevelAssembler already rescales them explicitly from gateSize.
        const float PixelsPerUnit = 110f;

        [MenuItem("From Cell/Setup/Bake Character Art", false, 4)]
        public static void BakeAll()
        {
            EnsureFolderChain(OutRoot);

            int ok = 0, failed = 0;
            foreach (var (key, relPath) in ArtKeys.AllSourceSprites)
            {
                string svgPath = $"{SourceRoot}/{relPath}";
                if (!File.Exists(svgPath))
                {
                    Debug.LogError($"From Cell: source SVG not found for '{key}' at {svgPath} - skipped.");
                    failed++;
                    continue;
                }

                try
                {
                    BakeOne(key, svgPath);
                    ok++;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"From Cell: failed to bake '{key}' from {svgPath}: {ex.Message}");
                    failed++;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"From Cell: baked {ok} character sprite(s) to {OutRoot} ({failed} failed).");
        }

        static void BakeOne(string key, string svgPath)
        {
            string svgText = File.ReadAllText(svgPath);
            RgbaImage image = MiniSvgRasterizer.Rasterize(svgText, OutputSize, supersample: 4);

            var colors = new Color32[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int i = (y * image.Width + x) * 4;
                    // RgbaImage is top-left origin; Texture2D.SetPixels32 expects bottom-left
                    // origin, so flip rows on the way in.
                    int flippedY = image.Height - 1 - y;
                    colors[flippedY * image.Width + x] = new Color32(
                        image.Pixels[i], image.Pixels[i + 1], image.Pixels[i + 2], image.Pixels[i + 3]);
                }
            }

            var texture = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false, false);
            texture.SetPixels32(colors);
            texture.Apply(false, false);
            byte[] png = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);

            string assetPath = $"{OutRoot}/{key}.png";
            string absPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
            File.WriteAllBytes(absPath, png);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            }
            if (importer == null)
            {
                Debug.LogError($"From Cell: no TextureImporter found for {assetPath} after import.");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = true;
            importer.isReadable = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 256;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spriteExtrude = 0;
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);

            importer.SaveAndReimport();
        }

        static void EnsureFolderChain(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;

            var parts = folder.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
