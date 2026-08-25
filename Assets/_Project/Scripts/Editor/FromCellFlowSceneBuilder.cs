#if UNITY_EDITOR
using FromCell.Audio;
using FromCell.Core;
using FromCell.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FromCell.Editor
{
    static class FromCellFlowSceneBuilder
    {
        const string MenuScenePath = "Assets/_Project/Scenes/Menu/_MainMenu.unity";
        const string BootScenePath = "Assets/_Project/Scenes/Boot/_Boot.unity";
        const string CreditsScenePath = "Assets/_Project/Scenes/Menu/_Credits.unity";
        const string SystemsPrefabPath = "Assets/_Project/Prefabs/Core/GameSystems.prefab";
        const string ConfigPath = "Assets/_Project/ScriptableObjects/GameConfig.asset";

        public static void CreateGameSystemsPrefab()
        {
            FromCellSetupMenu.EnsureFoldersPublic();

            var root = new GameObject("GameSystems");
            root.AddComponent<SaveProgressService>();
            root.AddComponent<SaveProfile>();
            var flow = root.AddComponent<GameFlowSystem>();
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            var flowSo = new SerializedObject(flow);
            flowSo.FindProperty("gameConfig").objectReferenceValue = config;
            flowSo.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, SystemsPrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        public static void CreateBootScene()
        {
            FromCellSetupMenu.EnsureFoldersPublic();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var boot = new GameObject("GameBootstrap");
            var bootstrap = boot.AddComponent<GameBootstrap>();
            var bootSo = new SerializedObject(bootstrap);
            bootSo.FindProperty("gameConfig").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            bootSo.ApplyModifiedPropertiesWithoutUndo();

            var cam = new GameObject("Main Camera");
            var camera = cam.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.08f, 0.1f, 0.14f);
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 0f, -10f);

            EditorSceneManager.SaveScene(scene, BootScenePath);
        }

        public static void CreateMainMenuScene()
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

            var canvasGo = new GameObject("MainMenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            var title = CreateMenuText(canvasGo.transform, "Title", "ESL Squad\nRun", 64, new Vector2(0.5f, 0.72f));

            var menuController = canvasGo.AddComponent<MainMenuController>();

            var newBtn = CreateMenuButton(canvasGo.transform, "NewGameButton", "NEW GAME", new Vector2(0.5f, 0.52f));
            var continueBtn = CreateMenuButton(canvasGo.transform, "ContinueButton", "CONTINUE", new Vector2(0.5f, 0.42f));
            var levelsBtn = CreateMenuButton(canvasGo.transform, "LevelsButton", "LEVELS", new Vector2(0.5f, 0.32f));
            var quitBtn = CreateMenuButton(canvasGo.transform, "QuitButton", "QUIT", new Vector2(0.5f, 0.20f));

            WireButton(newBtn, menuController.OnNewGame);
            WireButton(continueBtn, menuController.OnContinue);
            WireButton(levelsBtn, menuController.OnLevelSelect);
            WireButton(quitBtn, menuController.OnQuit);

            var menuSo = new SerializedObject(menuController);
            menuSo.FindProperty("continueButtonRoot").objectReferenceValue = continueBtn;
            menuSo.ApplyModifiedPropertiesWithoutUndo();

            var systemsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SystemsPrefabPath);
            if (systemsPrefab != null && Object.FindAnyObjectByType<GameFlowSystem>() == null)
                PrefabUtility.InstantiatePrefab(systemsPrefab);

            if (Object.FindAnyObjectByType<AudioManager>() == null)
            {
                var audio = new GameObject("AudioManager");
                audio.AddComponent<AudioManager>();
            }

            EditorSceneManager.SaveScene(scene, MenuScenePath);
        }

        public static void CreateCreditsScene()
        {
            FromCellSetupMenu.EnsureFoldersPublic();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cam = new GameObject("Main Camera");
            cam.AddComponent<Camera>().backgroundColor = new Color(0.05f, 0.08f, 0.12f);
            cam.tag = "MainCamera";

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("CreditsCanvas");
            canvasGo.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            CreateMenuText(canvasGo.transform, "CreditsText",
                "You joined the Squad.\n\nESL Squad Run\n\nThanks for playing.",
                36, new Vector2(0.5f, 0.6f));

            var credits = canvasGo.AddComponent<CreditsController>();
            var backBtn = CreateMenuButton(canvasGo.transform, "MenuButton", "MAIN MENU", new Vector2(0.5f, 0.25f));
            WireButton(backBtn, credits.OnBackToMenu);

            EditorSceneManager.SaveScene(scene, CreditsScenePath);
        }

        static GameObject CreateMenuButton(Transform parent, string name, string label, Vector2 anchorY)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, anchorY.y);
            rect.anchorMax = new Vector2(0.5f, anchorY.y);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(320, 64);
            rect.anchoredPosition = Vector2.zero;
            go.GetComponent<Image>().color = new Color(0.25f, 0.55f, 0.45f, 0.95f);

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return go;
        }

        static TextMeshProUGUI CreateMenuText(Transform parent, string name, string content, float size, Vector2 anchorY)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, anchorY.y);
            rect.anchorMax = new Vector2(0.5f, anchorY.y);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900, 200);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = size;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.85f, 0.95f, 0.9f);
            return tmp;
        }

        static void WireButton(GameObject buttonGo, UnityEngine.Events.UnityAction action)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(buttonGo.GetComponent<Button>().onClick, action);
        }
    }
}
#endif
