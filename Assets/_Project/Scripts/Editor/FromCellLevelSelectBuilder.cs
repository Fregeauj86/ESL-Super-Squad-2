#if UNITY_EDITOR
using FromCell.Core;
using FromCell.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FromCell.Editor
{
    /// <summary>
    /// Builds the _LevelSelect scene: a fixed 5x2 grid of LevelSelectEntry buttons (matching
    /// this project's "no runtime prefab instantiation" convention - built once here, then
    /// populated at runtime by LevelSelectController.Refresh()). Same construction style as
    /// FromCellFlowSceneBuilder's other menu scenes - reuses FromCellSetupMenu's internal
    /// CreateTmpText/CreateHudText/CreateMenuButton/CreateAnchoredPanel helpers.
    /// </summary>
    static class FromCellLevelSelectBuilder
    {
        const string LevelSelectScenePath = "Assets/_Project/Scenes/Menu/_LevelSelect.unity";
        const string ConfigPath = "Assets/_Project/ScriptableObjects/GameConfig.asset";
        const string SystemsPrefabPath = "Assets/_Project/Prefabs/Core/GameSystems.prefab";

        [MenuItem("From Cell/Setup/Create Level Select Scene")]
        public static void CreateLevelSelectScene()
        {
            FromCellSetupMenu.EnsureFoldersPublic();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cam = new GameObject("Main Camera");
            cam.AddComponent<Camera>().backgroundColor = new Color(0.08f, 0.1f, 0.14f);
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 0f, -10f);

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("LevelSelectCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            FromCellSetupMenu.CreateHudText(canvasGo.transform, "Title", "SELECT LEVEL", 42,
                TextAlignmentOptions.Center, new Vector2(0, -70), new Vector2(0.5f, 1f));

            var entries = new LevelSelectEntry[10];
            const int columns = 5;
            const float cellWidth = 340f;
            const float cellHeight = 220f;
            float startX = -(columns - 1) * cellWidth / 2f;

            for (int i = 0; i < 10; i++)
            {
                int col = i % columns;
                int row = i / columns;
                var pos = new Vector2(startX + col * cellWidth, 40f - row * cellHeight);
                entries[i] = CreateEntry(canvasGo.transform, i, pos);
            }

            var backBtn = FromCellSetupMenu.CreateMenuButton(canvasGo.transform, "BackButton", "BACK", new Vector2(0.5f, 0.08f));

            var controller = canvasGo.AddComponent<LevelSelectController>();
            controller.gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            controller.entries = entries;
            UnityEventTools.AddPersistentListener(backBtn.GetComponent<Button>().onClick, controller.OnBack);

            var systemsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SystemsPrefabPath);
            if (systemsPrefab != null && Object.FindAnyObjectByType<GameFlowSystem>() == null)
                PrefabUtility.InstantiatePrefab(systemsPrefab);

            EditorSceneManager.SaveScene(scene, LevelSelectScenePath);
            Debug.Log("From Cell: Level Select scene saved to " + LevelSelectScenePath);
        }

        static LevelSelectEntry CreateEntry(Transform parent, int index, Vector2 anchoredPos)
        {
            var go = new GameObject($"LevelEntry_{index}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300, 180);
            rect.anchoredPosition = anchoredPos;
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.4f, 0.95f);

            var label = FromCellSetupMenu.CreateTmpText("Label", go.transform, $"Level {index + 1}", 20, Color.white);
            label.rectTransform.offsetMin = new Vector2(12, 44);
            label.rectTransform.offsetMax = new Vector2(-12, -12);

            var rankLabel = FromCellSetupMenu.CreateTmpText("Rank", go.transform, "-", 30, new Color(1f, 0.85f, 0.3f));
            rankLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
            rankLabel.rectTransform.anchorMax = new Vector2(1f, 0f);
            rankLabel.rectTransform.offsetMin = new Vector2(12, 8);
            rankLabel.rectTransform.offsetMax = new Vector2(-12, 44);
            rankLabel.alignment = TextAlignmentOptions.Center;

            var lockedGo = new GameObject("Locked", typeof(RectTransform), typeof(Image));
            lockedGo.transform.SetParent(go.transform, false);
            var lockedRect = lockedGo.GetComponent<RectTransform>();
            lockedRect.anchorMin = Vector2.zero;
            lockedRect.anchorMax = Vector2.one;
            lockedRect.offsetMin = Vector2.zero;
            lockedRect.offsetMax = Vector2.zero;
            lockedGo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            FromCellSetupMenu.CreateTmpText("LockLabel", lockedGo.transform, "LOCKED", 20, Color.white);

            var entry = go.AddComponent<LevelSelectEntry>();
            entry.label = label;
            entry.rankLabel = rankLabel;
            entry.button = go.GetComponent<Button>();
            entry.lockedOverlay = lockedGo;

            UnityEventTools.AddPersistentListener(entry.button.onClick, entry.OnClick);

            return entry;
        }
    }
}
#endif
