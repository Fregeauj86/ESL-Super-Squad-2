#if UNITY_EDITOR
using System.IO;
using FromCell.Abilities;
using FromCell.Core;
using FromCell.Evolution;
using FromCell.Input;
using FromCell.Level;
using FromCell.Player;
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
    public static class FromCellSetupMenu
    {
        const string Root = "Assets/_Project";
        const string SoRoot = Root + "/ScriptableObjects";
        const string SceneRoot = Root + "/Scenes/Levels";
        const string PrefabRoot = Root + "/Prefabs";
        const string PlayerPrefabPath = PrefabRoot + "/Player/Player.prefab";
        const string MobileUiPrefabPath = PrefabRoot + "/UI/MobileControls.prefab";
        const string BootScenePath = "Assets/_Project/Scenes/Boot/_Boot.unity";
        const string MenuScenePath = "Assets/_Project/Scenes/Menu/_MainMenu.unity";
        const string CreditsScenePath = "Assets/_Project/Scenes/Menu/_Credits.unity";
        const string LevelSelectScenePath = "Assets/_Project/Scenes/Menu/_LevelSelect.unity";

        [MenuItem("From Cell/Open Main Menu Scene", false, -99)]
        public static void OpenMainMenuScene()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += OpenMainMenuScene;
                return;
            }

            if (!File.Exists(MenuScenePath))
            {
                Debug.LogError("From Cell: _MainMenu is missing. Run From Cell → Setup → Create Boot + Main Menu + Credits.");
                return;
            }

            EditorSceneManager.OpenScene(MenuScenePath);
            Debug.Log("From Cell: Opened _MainMenu. Press Play.");
        }

        [MenuItem("From Cell/Open Boot Scene", false, -100)]
        public static void OpenBootScene()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += OpenBootScene;
                Debug.LogWarning("From Cell: Stopped Play Mode so _Boot can open. Wait a moment.");
                return;
            }

            if (!File.Exists(BootScenePath))
            {
                Debug.LogError("From Cell: _Boot is missing. Run From Cell → Setup → Create Boot + Main Menu + Credits.");
                return;
            }

            EditorSceneManager.OpenScene(BootScenePath);
            if (Object.FindFirstObjectByType<Camera>() == null)
            {
                CreateMainCamera();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }

            ConfigureFullBuildSettings();

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
            if (sceneAsset != null)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = sceneAsset;
                EditorGUIUtility.PingObject(sceneAsset);
            }

            Debug.Log("From Cell: Opened _Boot (camera on). Press Play — it should load the main menu.");
        }

        [MenuItem("From Cell/Setup/Import TMP Essentials", false, 1)]
        public static void ImportTmpEssentialsIfMissing()
        {
            if (File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                Debug.Log("From Cell: TMP Essential Resources are already in the project. Close the TMP Importer window.");
                return;
            }

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += ImportTmpEssentialsIfMissing;
                Debug.LogWarning("From Cell: Play Mode was on. Import cannot run in Play Mode. Pressed Stop — wait a second, or run Import TMP Essentials again after the Game view stops.");
                return;
            }

            AssetDatabase.importPackageCompleted -= OnTmpEssentialsImported;
            AssetDatabase.importPackageCompleted += OnTmpEssentialsImported;
            TMP_PackageResourceImporter.ImportResources(true, false, false);
        }

        static void OnTmpEssentialsImported(string packageName)
        {
            AssetDatabase.importPackageCompleted -= OnTmpEssentialsImported;
            Debug.Log("From Cell: " + packageName + " imported. Close the TMP Importer window, then run From Cell → Setup → Run Full Prototype Setup. Do not press Play until that finishes.");
        }

        [MenuItem("From Cell/Setup/Run Full Prototype Setup", false, 0)]
        public static void RunFullPrototypeSetup()
        {
            EnsureFolders();
            FromCellEditorTags.EnsureProjectTagsAndLayers();
            ApplyAndroidDefaults();
            CreateDefaultGameData();
            FromCellArtBaker.BakeAll();
            CreatePlayerPrefab();
            CreateMobileUiPrefab();
            FromCellFlowSceneBuilder.CreateGameSystemsPrefab();
            FromCellLevelBuildersV2.CreateAllBlueprintLevels();
            FromCellFlowSceneBuilder.CreateBootScene();
            FromCellFlowSceneBuilder.CreateMainMenuScene();
            FromCellFlowSceneBuilder.CreateCreditsScene();
            FromCellLevelSelectBuilder.CreateLevelSelectScene();
            ConfigureFullBuildSettings();
            FromCell.Diagnostics.FromCellSelfTest.EditorCheck();
            Debug.Log("From Cell: Full game setup complete. Open _Boot or _MainMenu and press Play.");
        }

        [MenuItem("From Cell/Setup/Create All Graybox Levels (1-10)")]
        public static void CreateAllGrayboxLevels()
        {
            CreateGrayboxLevel01();
            CreateGrayboxLevel02();
            FromCellLevelBuilders.BuildLevel03Organism();
            FromCellLevelBuilders.BuildLevel04Primitive();
            FromCellLevelBuilders.BuildLevel05Embryo();
            FromCellLevelBuilders.BuildLevel06Nervous();
            FromCellLevelBuilders.BuildLevel07Newborn();
            FromCellLevelBuilders.BuildLevel08Child();
            FromCellLevelBuilders.BuildLevel09Teen();
            FromCellLevelBuilders.BuildLevel10Adult();
            Debug.Log("From Cell: All 10 graybox levels created.");
        }

        [MenuItem("From Cell/Setup/Create Boot + Main Menu + Credits")]
        public static void CreateFlowScenes()
        {
            FromCellFlowSceneBuilder.CreateGameSystemsPrefab();
            FromCellFlowSceneBuilder.CreateBootScene();
            FromCellFlowSceneBuilder.CreateMainMenuScene();
            FromCellFlowSceneBuilder.CreateCreditsScene();
            FromCellLevelSelectBuilder.CreateLevelSelectScene();
            ConfigureFullBuildSettings();
        }

        [MenuItem("From Cell/Setup/Create Default Game Data")]
        public static void CreateDefaultGameData()
        {
            EnsureFolders();

            var stages = new EvolutionStageData[10];
            for (int i = 0; i < 10; i++)
            {
                string path = $"{SoRoot}/Evolution/Stage_{i:00}_{(EvolutionStageId)i}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<EvolutionStageData>(path);
                stages[i] = existing != null ? existing : CreateStageAsset(path, i);
            }

            var levels = new LevelData[10];
            for (int i = 0; i < 10; i++)
            {
                string path = $"{SoRoot}/Levels/Level_{i + 1:00}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
                if (existing != null)
                {
                    levels[i] = existing;
                    levels[i].sceneName = GetSceneNameForLevel(i);
                    ApplyLevelMetadata(levels[i], i);
                    EditorUtility.SetDirty(levels[i]);
                }
                else
                {
                    levels[i] = CreateLevelAsset(path, i, stages[i]);
                }
            }

            string configPath = $"{SoRoot}/GameConfig.asset";
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(configPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<GameConfig>();
                AssetDatabase.CreateAsset(config, configPath);
            }

            config.evolutionStages = stages;
            config.levels = levels;
            EditorUtility.SetDirty(config);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("From Cell: GameConfig and 10 stages/levels created at " + SoRoot);
        }

        [MenuItem("From Cell/Setup/Create Player Prefab")]
        public static void CreatePlayerPrefab()
        {
            EnsureFolders();
            var temp = CreatePlayerGameObject(Vector3.zero);
            PrefabUtility.SaveAsPrefabAsset(temp, PlayerPrefabPath);
            Object.DestroyImmediate(temp);
            AssetDatabase.SaveAssets();
            Debug.Log("From Cell: Player prefab saved to " + PlayerPrefabPath);
        }

        [MenuItem("From Cell/Setup/Create Mobile UI Prefab")]
        public static void CreateMobileUiPrefab()
        {
            EnsureFolders();
            var uiRoot = BuildMobileUiHierarchy();
            PrefabUtility.SaveAsPrefabAsset(uiRoot, MobileUiPrefabPath);
            Object.DestroyImmediate(uiRoot);
            AssetDatabase.SaveAssets();
            Debug.Log("From Cell: Mobile UI prefab saved to " + MobileUiPrefabPath);
        }

        [MenuItem("From Cell/Setup/Create Graybox Level 01 Scene")]
        public static void CreateGrayboxLevel01()
        {
            BuildGrayboxLevel(
                levelIndex: 0,
                sceneFileName: "Level_01_FirstSteps",
                spawnPosition: new Vector3(-6f, -0.5f, 0f),
                finishPosition: new Vector3(12f, 2.5f, 0f),
                setupEnvironment: scene =>
                {
                    CreateGroundPlatform("Ground_Start", new Vector3(-4f, -2f, 0f), new Vector3(6f, 1f, 1f));
                    CreateGroundPlatform("Ground_Mid", new Vector3(3f, -0.5f, 0f), new Vector3(4f, 1f, 1f));
                    CreateGroundPlatform("Ground_End", new Vector3(10f, 1f, 0f), new Vector3(4f, 1f, 1f));
                    CreateWindZone("Current_Right", new Vector3(0f, 0f, 0f), new Vector3(8f, 6f, 1f), new Vector2(8f, 0f));
                });
        }

        [MenuItem("From Cell/Setup/Create Graybox Level 02 Scene")]
        public static void CreateGrayboxLevel02()
        {
            BuildGrayboxLevel(
                levelIndex: 1,
                sceneFileName: "Level_02_SteadyScout",
                spawnPosition: new Vector3(-8f, 0f, 0f),
                finishPosition: new Vector3(14f, 3f, 0f),
                setupEnvironment: scene =>
                {
                    CreateGroundPlatform("Ground_Start", new Vector3(-6f, -1.5f, 0f), new Vector3(5f, 1f, 1f));
                    CreateGroundPlatform("Ground_Branch_A", new Vector3(-1f, 0.5f, 0f), new Vector3(3f, 0.6f, 1f));
                    CreateGroundPlatform("Ground_Branch_B", new Vector3(4f, 1.5f, 0f), new Vector3(4f, 0.6f, 1f));
                    CreateGroundPlatform("Ground_End", new Vector3(12f, 2.5f, 0f), new Vector3(4f, 1f, 1f));

                    CreateKillZone("Pit_Hazard", new Vector3(2f, -3f, 0f), new Vector3(6f, 1f, 1f));

                    var checkpoint = new GameObject("Checkpoint_Mid");
                    checkpoint.transform.position = new Vector3(4f, 2.5f, 0f);
                    var cpCol = checkpoint.AddComponent<BoxCollider2D>();
                    cpCol.isTrigger = true;
                    cpCol.size = new Vector2(1.5f, 2f);
                    checkpoint.AddComponent<Checkpoint>();

                    for (int i = 0; i < 3; i++)
                        CreateCollectible($"VocabGem_{i + 1}", new Vector3(-2f + i * 3f, 1.5f, 0f));
                });
        }

        [MenuItem("From Cell/Setup/Configure Build Settings (Levels 1-2)")]
        public static void ConfigureBuildSettings() => ConfigureFullBuildSettings();

        [MenuItem("From Cell/Setup/Configure Full Build Settings")]
        public static void ConfigureFullBuildSettings()
        {
            var scenes = new[]
            {
                BootScenePath,
                MenuScenePath,
                $"{SceneRoot}/Level_01_FirstSteps.unity",
                $"{SceneRoot}/Level_02_SteadyScout.unity",
                $"{SceneRoot}/Level_03_ShellGuard.unity",
                $"{SceneRoot}/Level_04_GentleGiant.unity",
                $"{SceneRoot}/Level_05_DeepDiver.unity",
                $"{SceneRoot}/Level_06_RisingWings.unity",
                $"{SceneRoot}/Level_07_FastTrack.unity",
                $"{SceneRoot}/Level_08_PowerHop.unity",
                $"{SceneRoot}/Level_09_MasterMentor.unity",
                $"{SceneRoot}/Level_10_SquadChampion.unity",
                CreditsScenePath,
                LevelSelectScenePath
            };

            var buildScenes = new EditorBuildSettingsScene[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
                buildScenes[i] = new EditorBuildSettingsScene(scenes[i], AssetDatabase.LoadAssetAtPath<SceneAsset>(scenes[i]) != null);

            EditorBuildSettings.scenes = buildScenes;
            Debug.Log("From Cell: Full Build Settings configured (Boot → Menu → Levels 1-10 → Credits).");
        }

        [MenuItem("From Cell/Setup/Apply Android Defaults (ARM64)")]
        public static void ApplyAndroidDefaults()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            Debug.Log("From Cell: Android defaults applied (IL2CPP, ARM64, landscape).");
        }

        public static void EnsureFoldersPublic() => EnsureFolders();

        public static void BuildGrayboxLevelPublic(
            int levelIndex,
            string sceneFileName,
            Vector3 spawnPosition,
            Vector3 finishPosition,
            System.Action<UnityEngine.SceneManagement.Scene> setupEnvironment)
        {
            BuildGrayboxLevel(levelIndex, sceneFileName, spawnPosition, finishPosition, setupEnvironment);
        }

        public static void CreateGroundPlatformPublic(string name, Vector3 position, Vector3 scale) =>
            CreateGroundPlatform(name, position, scale);

        public static void CreateKillZonePublic(string name, Vector3 position, Vector3 scale) =>
            CreateKillZone(name, position, scale);

        public static GameObject CreateRolePad(string name, Vector3 position, PlayerRoleState.SquadRole role)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 0.5f);
            var pad = go.AddComponent<RoleSwitchPad>();
            var so = new SerializedObject(pad);
            so.FindProperty("role").enumValueIndex = (int)role;
            so.ApplyModifiedPropertiesWithoutUndo();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = role == PlayerRoleState.SquadRole.Nerve
                ? new Color(0.5f, 0.6f, 0.95f, 0.8f)
                : new Color(0.95f, 0.5f, 0.45f, 0.8f);
            go.transform.localScale = new Vector3(1.5f, 0.4f, 1f);
            return go;
        }

        public static void CreateRoleGate(string name, Vector3 position, PlayerRoleState.SquadRole requiredRole)
        {
            var gateRoot = new GameObject(name);
            gateRoot.transform.position = position;

            var triggerCol = gateRoot.AddComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;
            triggerCol.size = new Vector2(2f, 2f);

            var blocker = new GameObject("Blocker");
            blocker.transform.SetParent(gateRoot.transform);
            blocker.transform.localPosition = Vector3.zero;
            var blockCol = blocker.AddComponent<BoxCollider2D>();
            blockCol.size = new Vector2(0.5f, 2f);
            var blockSr = blocker.AddComponent<SpriteRenderer>();
            blockSr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            blockSr.color = new Color(0.6f, 0.3f, 0.7f, 0.9f);
            blocker.transform.localScale = new Vector3(0.5f, 2f, 1f);

            var gate = gateRoot.AddComponent<RoleGate>();
            var gateSo = new SerializedObject(gate);
            gateSo.FindProperty("requiredRole").enumValueIndex = (int)requiredRole;
            gateSo.FindProperty("blockingCollider").objectReferenceValue = blockCol;
            gateSo.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void CreateGrowthPickupPublic(string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;
            go.AddComponent<GrowthPickup>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            sr.color = new Color(0.95f, 0.55f, 0.75f);
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }

        static void BuildGrayboxLevel(
            int levelIndex,
            string sceneFileName,
            Vector3 spawnPosition,
            Vector3 finishPosition,
            System.Action<UnityEngine.SceneManagement.Scene> setupEnvironment)
        {
            EnsureFolders();
            CreateDefaultGameData();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = CreateMainCamera();
            cameraGo.AddComponent<FromCell.Cameras.CameraFollow2D>();
            cameraGo.AddComponent<FromCell.Cameras.CameraDirector>();

            var fxSystem = new GameObject("FxSystem");
            fxSystem.AddComponent<FromCell.Feel.FxPool>();
            fxSystem.AddComponent<FromCell.Feel.ScreenShake>();
            fxSystem.AddComponent<FromCell.Feel.HitStop>();
            fxSystem.AddComponent<FromCell.Feel.FxBinder>();
            fxSystem.AddComponent<FromCell.Audio.AudioSignalsHook>();

            setupEnvironment?.Invoke(scene);

            var spawn = new GameObject("PlayerSpawn");
            spawn.transform.position = spawnPosition;

            var checkpointRoot = new GameObject("CheckpointSystem");
            checkpointRoot.AddComponent<CheckpointSystem>();
            checkpointRoot.transform.position = spawnPosition;

            var finish = new GameObject("FinishZone");
            finish.transform.position = finishPosition;
            var finishCol = finish.AddComponent<BoxCollider2D>();
            finishCol.isTrigger = true;
            finishCol.size = new Vector2(2f, 3f);
            finish.AddComponent<FinishZone>();

            var config = AssetDatabase.LoadAssetAtPath<GameConfig>($"{SoRoot}/GameConfig.asset");
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);

            var bootstrap = new GameObject("LevelBootstrap");
            var bootstrapComp = bootstrap.AddComponent<LevelBootstrap>();
            var bootstrapSo = new SerializedObject(bootstrapComp);
            bootstrapSo.FindProperty("gameConfig").objectReferenceValue = config;
            bootstrapSo.FindProperty("levelIndex").intValue = levelIndex;
            bootstrapSo.FindProperty("playerSpawn").objectReferenceValue = spawn.transform;
            bootstrapSo.FindProperty("playerPrefab").objectReferenceValue = playerPrefab;
            bootstrapSo.ApplyModifiedPropertiesWithoutUndo();

            if (GameObject.FindFirstObjectByType<GameFlowSystem>() == null)
            {
                var flow = new GameObject("GameFlowSystem");
                var flowComp = flow.AddComponent<GameFlowSystem>();
                var flowSo = new SerializedObject(flowComp);
                flowSo.FindProperty("gameConfig").objectReferenceValue = config;
                flowSo.ApplyModifiedPropertiesWithoutUndo();
            }

            var evolution = new GameObject("EvolutionSystem");
            evolution.AddComponent<EvolutionSystem>();

            var completion = new GameObject("LevelCompletionSystem");
            completion.AddComponent<LevelCompletionSystem>();

            var runTracker = new GameObject("LevelRunTracker");
            runTracker.AddComponent<LevelRunTracker>();

            InstantiateMobileUiIfAvailable();
            InstantiatePlayerIfNoPrefab(spawnPosition, playerPrefab);

            string scenePath = $"{SceneRoot}/{sceneFileName}.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"From Cell: Graybox level saved to {scenePath}");
        }

        static void InstantiateMobileUiIfAvailable()
        {
            var mobilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MobileUiPrefabPath);
            if (mobilePrefab != null)
                PrefabUtility.InstantiatePrefab(mobilePrefab);
            else
                BuildMobileUiHierarchy();
        }

        static void InstantiatePlayerIfNoPrefab(Vector3 position, GameObject playerPrefab)
        {
            if (playerPrefab != null)
            {
                var instance = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
                if (instance != null)
                    instance.transform.position = position;
                return;
            }

            CreatePlayerGameObject(position);
        }

        static GameObject BuildMobileUiHierarchy()
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("MobileControls");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasGo.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var touchInput = canvasGo.AddComponent<TouchInputManager>();

            var joystickRoot = CreateUiRect("Joystick", canvasGo.transform, new Vector2(220, 220), new Vector2(180, 180));
            var joystickBg = CreateUiImage("Background", joystickRoot, new Color(1f, 1f, 1f, 0.25f), Vector2.zero, new Vector2(220, 220));
            var joystickHandle = CreateUiImage("Handle", joystickBg, new Color(1f, 1f, 1f, 0.85f), Vector2.zero, new Vector2(80, 80));

            var joystick = joystickRoot.gameObject.AddComponent<Joystick>();
            joystick.background = joystickBg;
            joystick.handle = joystickHandle;

            // Anchored bottom-right (unlike the joystick, which is bottom-left) - the
            // negative offsets below mean "180/360px left of the right edge." Previously
            // these used CreateUiRect's default bottom-left anchor, which put both buttons
            // off-screen to the left of the canvas entirely.
            var jumpRoot = CreateUiRect("JumpButton", canvasGo.transform, new Vector2(160, 160), new Vector2(-180, 180), new Vector2(1f, 0f));
            var jumpImage = CreateUiImage("Background", jumpRoot, new Color(0.3f, 0.85f, 0.55f, 0.9f), Vector2.zero, new Vector2(160, 160));
            var jumpButton = jumpImage.gameObject.AddComponent<Button>();
            CreateUiText("Label", jumpImage, "JUMP", 28, Color.white);
            var jumpHook = jumpRoot.gameObject.AddComponent<JumpButton>();
            UnityEventTools.AddPersistentListener(jumpButton.onClick, jumpHook.OnJumpPressed);

            var dashRoot = CreateUiRect("DashButton", canvasGo.transform, new Vector2(140, 140), new Vector2(-360, 180), new Vector2(1f, 0f));
            var dashImage = CreateUiImage("Background", dashRoot, new Color(0.35f, 0.65f, 0.95f, 0.9f), Vector2.zero, new Vector2(140, 140));
            var dashButton = dashImage.gameObject.AddComponent<Button>();
            CreateUiText("Label", dashImage, "DASH", 24, Color.white);
            var dashHook = dashRoot.gameObject.AddComponent<DashButton>();
            UnityEventTools.AddPersistentListener(dashButton.onClick, dashHook.OnDashPressed);
            dashRoot.gameObject.SetActive(false);

            var visibility = canvasGo.AddComponent<AbilityButtonVisibility>();
            var visSo = new SerializedObject(visibility);
            visSo.FindProperty("dashButtonRoot").objectReferenceValue = dashRoot.gameObject;
            visSo.ApplyModifiedPropertiesWithoutUndo();

            // Centered on screen - was also defaulting to the bottom-left anchor, which put
            // 3/4 of this panel off-screen in the corner whenever it appeared.
            var overlay = CreateUiRect("EvolutionOverlay", canvasGo.transform, new Vector2(900, 320), Vector2.zero, new Vector2(0.5f, 0.5f));
            var overlayImage = CreateUiImage("Panel", overlay, new Color(0.05f, 0.08f, 0.12f, 0.92f), Vector2.zero, new Vector2(900, 320));
            var title = CreateTmpText("Title", overlayImage, "Powered Up!", 42, new Color(0.55f, 0.95f, 0.8f));
            var humor = CreateTmpText("Humor", overlayImage, "...", 26, Color.white);
            humor.rectTransform.anchoredPosition = new Vector2(0, -50);

            var presenter = canvasGo.AddComponent<EvolutionPresenter>();
            var presenterSo = new SerializedObject(presenter);
            presenterSo.FindProperty("overlayRoot").objectReferenceValue = overlay.gameObject;
            presenterSo.FindProperty("titleText").objectReferenceValue = title;
            presenterSo.FindProperty("humorText").objectReferenceValue = humor;
            presenterSo.ApplyModifiedPropertiesWithoutUndo();

            var touchSo = new SerializedObject(touchInput);
            touchSo.FindProperty("joystick").objectReferenceValue = joystick;
            touchSo.ApplyModifiedPropertiesWithoutUndo();

            overlay.gameObject.SetActive(false);

            BuildHud(canvasGo);
            BuildPauseMenu(canvasGo);
            BuildTutorialBanner(canvasGo);
            BuildDashCooldown(canvasGo, dashRoot.gameObject);
            FromCellEslUiBuilder.BuildEslChallengeOverlay(canvasGo);
            BuildHudTimer(canvasGo);
            BuildResultsScreen(canvasGo);

            return canvasGo;
        }

        static void BuildHud(GameObject canvasGo)
        {
            var hudBar = CreateAnchoredPanel(canvasGo.transform, "HUD", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(900, 70));
            var stage = CreateHudText(hudBar, "StageLabel", "Recruit", 26, TextAlignmentOptions.Left, new Vector2(20, -35), new Vector2(0, 1f));
            var level = CreateHudText(hudBar, "LevelLabel", "Level 1", 22, TextAlignmentOptions.Center, new Vector2(0, -35), new Vector2(0.5f, 1f));
            var nutrients = CreateHudText(hudBar, "CollectiblesLabel", "Vocabulary Gems: 0", 22, TextAlignmentOptions.Right, new Vector2(-20, -35), new Vector2(1f, 1f));

            var hud = canvasGo.AddComponent<GameplayHUD>();
            var hudSo = new SerializedObject(hud);
            hudSo.FindProperty("stageLabel").objectReferenceValue = stage;
            hudSo.FindProperty("levelLabel").objectReferenceValue = level;
            hudSo.FindProperty("collectiblesLabel").objectReferenceValue = nutrients;
            hudSo.ApplyModifiedPropertiesWithoutUndo();
        }

        static void BuildPauseMenu(GameObject canvasGo)
        {
            var pauseBtnRect = CreateAnchoredPanel(canvasGo.transform, "PauseButton", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(70, -70), new Vector2(90, 90));
            pauseBtnRect.gameObject.AddComponent<Image>().color = new Color(0.15f, 0.2f, 0.25f, 0.9f);
            var pauseBtn = pauseBtnRect.gameObject.AddComponent<Button>();
            CreateUiText("Label", pauseBtnRect, "II", 30, Color.white);

            var panel = CreateAnchoredPanel(canvasGo.transform, "PausePanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460, 400));
            panel.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.96f);

            var pause = canvasGo.AddComponent<PauseManager>();
            var resume = CreateMenuButton(panel, "ResumeButton", "RESUME", new Vector2(0.5f, 0.68f));
            var restart = CreateMenuButton(panel, "RestartButton", "RESTART", new Vector2(0.5f, 0.52f));
            var menu = CreateMenuButton(panel, "MenuButton", "MAIN MENU", new Vector2(0.5f, 0.36f));

            UnityEventTools.AddPersistentListener(pauseBtn.onClick, pause.TogglePause);
            UnityEventTools.AddPersistentListener(resume.GetComponent<Button>().onClick, pause.OnResume);
            UnityEventTools.AddPersistentListener(restart.GetComponent<Button>().onClick, pause.OnRestartLevel);
            UnityEventTools.AddPersistentListener(menu.GetComponent<Button>().onClick, pause.OnQuitToMenu);

            var pauseSo = new SerializedObject(pause);
            pauseSo.FindProperty("pausePanel").objectReferenceValue = panel.gameObject;
            pauseSo.ApplyModifiedPropertiesWithoutUndo();
            panel.gameObject.SetActive(false);
        }

        static void BuildTutorialBanner(GameObject canvasGo)
        {
            var banner = CreateAnchoredPanel(canvasGo.transform, "TutorialBanner", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 100), new Vector2(880, 80));
            banner.gameObject.AddComponent<Image>().color = new Color(0.1f, 0.14f, 0.18f, 0.9f);
            var text = CreateHudText(banner, "Message", "Tip", 24, TextAlignmentOptions.Center, Vector2.zero, new Vector2(0.5f, 0.5f));

            var tutorial = canvasGo.AddComponent<TutorialBanner>();
            var so = new SerializedObject(tutorial);
            so.FindProperty("root").objectReferenceValue = banner.gameObject;
            so.FindProperty("messageText").objectReferenceValue = text;
            so.ApplyModifiedPropertiesWithoutUndo();
            banner.gameObject.SetActive(false);
        }

        static void BuildDashCooldown(GameObject canvasGo, GameObject dashButtonRoot)
        {
            var cd = CreateAnchoredPanel(canvasGo.transform, "DashCooldown", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-360, 250), new Vector2(100, 10));
            var img = cd.gameObject.AddComponent<Image>();
            img.color = new Color(0.25f, 0.55f, 0.95f, 0.85f);
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillAmount = 1f;

            var cdUi = canvasGo.AddComponent<DashCooldownUI>();
            var so = new SerializedObject(cdUi);
            so.FindProperty("fillImage").objectReferenceValue = img;
            so.FindProperty("root").objectReferenceValue = cd.gameObject;
            so.ApplyModifiedPropertiesWithoutUndo();
            cd.gameObject.SetActive(false);
        }

        static void BuildHudTimer(GameObject canvasGo)
        {
            var timerLabel = CreateHudText(canvasGo.transform, "TimerLabel", "00:00", 22, TextAlignmentOptions.Center, new Vector2(0, -110), new Vector2(0.5f, 1f));

            var timer = canvasGo.AddComponent<HudTimer>();
            timer.timerLabel = timerLabel;
        }

        static void BuildResultsScreen(GameObject canvasGo)
        {
            var panel = CreateAnchoredPanel(canvasGo.transform, "ResultsPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -280), new Vector2(520, 180));
            panel.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.08f, 0.12f, 0.92f);
            var statsText = CreateTmpText("StatsText", panel, "Time: 00:00\nDeaths: 0", 26, new Color(0.85f, 0.95f, 0.9f));

            var results = canvasGo.AddComponent<ResultsScreen>();
            results.overlayRoot = panel.gameObject;
            results.statsText = statsText;

            panel.gameObject.SetActive(false);
        }

        internal static RectTransform CreateAnchoredPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;
            return rect;
        }

        internal static TextMeshProUGUI CreateHudText(Transform parent, string name, string text, float size, TextAlignmentOptions align, Vector2 anchoredPos, Vector2 anchor)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(280, 40);
            rect.anchoredPosition = anchoredPos;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = new Color(0.9f, 0.95f, 0.92f);
            return tmp;
        }

        internal static GameObject CreateMenuButton(Transform parent, string name, string label, Vector2 anchorY)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, anchorY.y);
            rect.anchorMax = new Vector2(0.5f, anchorY.y);
            rect.sizeDelta = new Vector2(300, 56);
            go.GetComponent<Image>().color = new Color(0.25f, 0.55f, 0.45f, 0.95f);
            CreateUiText("Label", go.transform, label, 24, Color.white);
            return go;
        }

        public static void CreateUiTextPublic(string name, Transform parent, string text, int fontSize, Color color) =>
            CreateUiText(name, parent, text, fontSize, color);

        static void CreateWindZone(string name, Vector3 position, Vector3 scale, Vector2 force)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = scale;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            var wind = go.AddComponent<FromCell.Level.WindZone>();
            var so = new SerializedObject(wind);
            so.FindProperty("force").vector2Value = force;
            so.ApplyModifiedPropertiesWithoutUndo();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = new Color(0.4f, 0.7f, 0.95f, 0.25f);
        }

        static GameObject CreatePlayerGameObject(Vector3 position)
        {
            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = position;

            // Sprite lives on a child "Visual" object, never the root - PlayerJuice's
            // squash-and-stretch scales this transform, and scaling the root instead would
            // also distort the CapsuleCollider2D and GroundCheck's child offset below.
            var visualGo = new GameObject("Visual");
            visualGo.transform.SetParent(player.transform, false);
            var sr = visualGo.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            sr.color = new Color(0.4f, 0.85f, 0.95f);

            var rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = player.AddComponent<CapsuleCollider2D>();
            col.size = new Vector2(0.8f, 0.8f);

            var groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0f, -0.45f, 0f);

            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerMovement>();
            var gc = player.AddComponent<GroundChecker>();
            var gcSo = new SerializedObject(gc);
            gcSo.FindProperty("groundCheckPoint").objectReferenceValue = groundCheck.transform;
            gcSo.ApplyModifiedPropertiesWithoutUndo();

            player.AddComponent<AbilityManager>();
            player.AddComponent<PlayerHealth>();
            player.AddComponent<PlayerRoleState>();
            player.AddComponent<PlayerFacing>();
            var visual = player.AddComponent<PlayerVisual>();
            var visSo = new SerializedObject(visual);
            visSo.FindProperty("spriteRenderer").objectReferenceValue = sr;
            visSo.FindProperty("bodyCollider").objectReferenceValue = col;
            visSo.ApplyModifiedPropertiesWithoutUndo();

            player.AddComponent<PlayerGroundEvents>();
            var juice = player.AddComponent<PlayerJuice>();
            juice.visualTransform = visualGo.transform;

            var afterimage = player.AddComponent<FromCell.Feel.Afterimage>();
            afterimage.sourceRenderer = sr;

            return player;
        }

        static GameObject CreateMainCamera()
        {
            var cam = new GameObject("Main Camera");
            var camera = cam.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            // Without this, clearFlags defaults to Skybox and Unity's default skybox
            // material renders through as a sky-to-ground gradient behind the actual level
            // geometry - easy to mistake for real ground/background art. A flat sky color is
            // what every level actually wants here (no skybox/backdrop system exists).
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.55f, 0.75f, 0.92f);
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 0f, -10f);
            return cam;
        }

        static void CreateGroundPlatform(string name, Vector3 position, Vector3 scale)
        {
            var go = new GameObject(name);
            go.tag = "Ground";
            go.transform.position = position;
            go.transform.localScale = scale;
            go.AddComponent<BoxCollider2D>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = new Color(0.35f, 0.55f, 0.4f);
            sr.drawMode = SpriteDrawMode.Sliced;
        }

        static void CreateKillZone(string name, Vector3 position, Vector3 scale)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = scale;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            go.AddComponent<KillZone>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = new Color(0.75f, 0.2f, 0.25f, 0.45f);
        }

        static void CreateCollectible(string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
            go.AddComponent<Collectible>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            sr.color = new Color(0.95f, 0.85f, 0.3f);
            sr.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }

        static RectTransform CreateUiRect(string name, Transform parent, Vector2 size, Vector2 anchoredPos, Vector2? anchor = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            var a = anchor ?? Vector2.zero;
            rect.anchorMin = a;
            rect.anchorMax = a;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPos;
            return rect;
        }

        static RectTransform CreateUiImage(string name, Transform parent, Color color, Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            go.GetComponent<Image>().color = color;
            return rect;
        }

        static Text CreateUiText(string name, Transform parent, string text, int fontSize, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var uiText = go.GetComponent<Text>();
            uiText.text = text;
            uiText.fontSize = fontSize;
            uiText.color = color;
            uiText.alignment = TextAnchor.MiddleCenter;
            uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return uiText;
        }

        internal static TextMeshProUGUI CreateTmpText(string name, Transform parent, string text, float fontSize, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(24, 24);
            rect.offsetMax = new Vector2(-24, -24);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        internal static string GetSceneNameForLevel(int index)
        {
            switch ((EvolutionStageId)index)
            {
                case EvolutionStageId.Cell: return "Level_01_FirstSteps";
                case EvolutionStageId.Cluster: return "Level_02_SteadyScout";
                case EvolutionStageId.Organism: return "Level_03_ShellGuard";
                case EvolutionStageId.Primitive: return "Level_04_GentleGiant";
                case EvolutionStageId.Embryo: return "Level_05_DeepDiver";
                case EvolutionStageId.Nervous: return "Level_06_RisingWings";
                case EvolutionStageId.Newborn: return "Level_07_FastTrack";
                case EvolutionStageId.Child: return "Level_08_PowerHop";
                case EvolutionStageId.Teen: return "Level_09_MasterMentor";
                case EvolutionStageId.Adult: return "Level_10_SquadChampion";
                default: return $"Level_{index + 1:00}";
            }
        }

        // Display identity for each ability tier - the Squad Hero the player becomes
        // when they reach that tier. EvolutionStageId's own enum VALUES stay as they
        // are (Cell/Cluster/.../Adult - internal identifiers only, never shown to the
        // player); this is what's actually shown in the HUD/evolution overlay.
        static string GetTierDisplayName(EvolutionStageId id)
        {
            switch (id)
            {
                case EvolutionStageId.Cell: return "Milo Mouse — First Steps";
                case EvolutionStageId.Cluster: return "Milo Mouse — Steady Scout";
                case EvolutionStageId.Organism: return "Timmy Turtle — Shell Guard";
                case EvolutionStageId.Primitive: return "Max Elephant — Gentle Giant";
                case EvolutionStageId.Embryo: return "Finn Whale — Deep Diver";
                case EvolutionStageId.Nervous: return "Sky Eagle — Rising Wings";
                case EvolutionStageId.Newborn: return "Dash Cheetah — Fast Track";
                case EvolutionStageId.Child: return "Big Tick — Power Hop";
                case EvolutionStageId.Teen: return "Dr Imperfecto — Master Mentor";
                case EvolutionStageId.Adult: return "King Leo — Squad Champion";
                default: return id.ToString();
            }
        }

        static EvolutionStageData CreateStageAsset(string path, int index)
        {
            var stage = ScriptableObject.CreateInstance<EvolutionStageData>();
            var id = (EvolutionStageId)index;
            stage.stageId = id;
            stage.displayName = GetTierDisplayName(id);
            stage.humorLine = GetHumorLine(id);
            ApplyStageDefaults(stage, id);
            AssetDatabase.CreateAsset(stage, path);
            return stage;
        }

        static LevelData CreateLevelAsset(string path, int index, EvolutionStageData stage)
        {
            var level = ScriptableObject.CreateInstance<LevelData>();
            level.levelIndex = index;
            level.sceneName = GetSceneNameForLevel(index);
            level.stageId = stage.stageId;
            level.displayName = $"Level {index + 1} — {stage.displayName}";
            level.targetDurationMinutes = 3;
            ApplyLevelMetadata(level, index);
            AssetDatabase.CreateAsset(level, path);
            return level;
        }

        static void ApplyLevelMetadata(LevelData level, int index)
        {
            level.requiredCollectibles = index == 1 ? 3 : 0;
            level.tutorialPrompts = new[] { GetTutorialLine(index) };
        }

        static string GetTutorialLine(int index)
        {
            switch (index)
            {
                case 0: return "Drift toward the glowing exit. Jump unlocks soon.";
                case 1: return "Collect 3 vocabulary gems before you leave.";
                case 2: return "Step on colored pads to switch Squad roles.";
                case 3: return "Weak jump unlocked. Climb toward the outpost.";
                case 4: return "Grab confidence stars to get stronger.";
                case 5: return "Precision jumps. Don't fall.";
                case 6: return "Standard platforming begins here.";
                case 7: return "Double jump is online. Go explore upward.";
                case 8: return "Dash through the gaps. Mind the pit.";
                case 9: return "Final gauntlet. All abilities unlocked.";
                default: return string.Empty;
            }
        }

        static void ApplyStageDefaults(EvolutionStageData stage, EvolutionStageId id)
        {
            switch (id)
            {
                case EvolutionStageId.Cell:
                    stage.movementMode = MovementMode.Float;
                    stage.moveSpeed = 2f;
                    stage.jumpForce = 0f;
                    stage.canJump = false;
                    stage.gravityScale = 1.5f;
                    stage.colliderSize = new Vector2(0.6f, 0.6f);
                    stage.paletteTint = new Color(0.55f, 0.9f, 0.75f);
                    break;
                case EvolutionStageId.Cluster:
                    stage.movementMode = MovementMode.Float;
                    stage.moveSpeed = 2.5f;
                    stage.jumpForce = 0f;
                    stage.canJump = false;
                    stage.gravityScale = 2f;
                    stage.paletteTint = new Color(0.5f, 0.85f, 0.7f);
                    break;
                case EvolutionStageId.Organism:
                    stage.movementMode = MovementMode.Float;
                    stage.moveSpeed = 3f;
                    stage.jumpForce = 6f;
                    stage.canJump = true;
                    stage.gravityScale = 2.5f;
                    stage.paletteTint = new Color(0.65f, 0.8f, 0.55f);
                    break;
                case EvolutionStageId.Primitive:
                    stage.movementMode = MovementMode.Crawl;
                    stage.moveSpeed = 3f;
                    stage.jumpForce = 8f;
                    stage.gravityScale = 3f;
                    stage.paletteTint = new Color(0.75f, 0.7f, 0.5f);
                    break;
                case EvolutionStageId.Embryo:
                    stage.movementMode = MovementMode.Crawl;
                    stage.moveSpeed = 3.2f;
                    stage.jumpForce = 8f;
                    stage.gravityScale = 3f;
                    stage.paletteTint = new Color(0.9f, 0.75f, 0.7f);
                    break;
                case EvolutionStageId.Nervous:
                    stage.movementMode = MovementMode.Walk;
                    stage.moveSpeed = 4f;
                    stage.jumpForce = 9f;
                    stage.acceleration = 60f;
                    stage.paletteTint = new Color(0.85f, 0.8f, 0.9f);
                    break;
                case EvolutionStageId.Newborn:
                    stage.movementMode = MovementMode.Walk;
                    stage.moveSpeed = 4.5f;
                    stage.jumpForce = 10f;
                    stage.acceleration = 45f;
                    stage.paletteTint = new Color(0.95f, 0.85f, 0.8f);
                    break;
                case EvolutionStageId.Child:
                    stage.movementMode = MovementMode.Walk;
                    stage.moveSpeed = 5f;
                    stage.jumpForce = 11f;
                    stage.canDoubleJump = true;
                    stage.paletteTint = new Color(0.9f, 0.88f, 0.75f);
                    break;
                case EvolutionStageId.Teen:
                    stage.movementMode = MovementMode.Walk;
                    stage.moveSpeed = 6f;
                    stage.jumpForce = 11f;
                    stage.airControl = 0.75f;
                    stage.canDoubleJump = true;
                    stage.canDash = true;
                    stage.paletteTint = new Color(0.8f, 0.85f, 0.95f);
                    break;
                case EvolutionStageId.Adult:
                    stage.movementMode = MovementMode.Walk;
                    stage.moveSpeed = 7f;
                    stage.jumpForce = 12f;
                    stage.canDoubleJump = true;
                    stage.canDash = true;
                    stage.paletteTint = new Color(0.95f, 0.9f, 0.85f);
                    break;
            }
        }

        static string GetHumorLine(EvolutionStageId id)
        {
            switch (id)
            {
                case EvolutionStageId.Cell: return "You joined the Squad. Try not to panic.";
                case EvolutionStageId.Cluster: return "Milo's getting faster. The paperwork is not.";
                case EvolutionStageId.Organism: return "Timmy's shell says hello. Jump unlocked.";
                case EvolutionStageId.Primitive: return "Congratulations. Max never forgets a step.";
                case EvolutionStageId.Embryo: return "Finn's diving in. Mood: buoyant.";
                case EvolutionStageId.Nervous: return "Sky's wings are online. Overthinking begins shortly.";
                case EvolutionStageId.Newborn: return "Dash has legs. Please use them responsibly.";
                case EvolutionStageId.Child: return "Big Tick's double jump unlocked. Supervision not included.";
                case EvolutionStageId.Teen: return "Dr. Imperfecto's dash: maximum velocity, minimum planning.";
                case EvolutionStageId.Adult: return "Leo's fully trained. Please use stairs like everyone else.";
                default: return string.Empty;
            }
        }

        static void EnsureFolders()
        {
            foreach (var folder in new[]
            {
                Root,
                SoRoot,
                SoRoot + "/Evolution",
                SoRoot + "/Levels",
                Root + "/Scenes",
                SceneRoot,
                Root + "/Scenes/Boot",
                Root + "/Scenes/Menu",
                PrefabRoot,
                PrefabRoot + "/Player",
                PrefabRoot + "/UI",
                PrefabRoot + "/Core",
                Root + "/Scripts/Editor"
            })
            {
                if (AssetDatabase.IsValidFolder(folder))
                    continue;

                var parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
                var child = Path.GetFileName(folder);
                if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(child))
                    AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
#endif
