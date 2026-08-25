#if UNITY_EDITOR
using System.IO;
using FromCell.Core;
using FromCell.Evolution;
using FromCell.Input;
using FromCell.Level;
using FromCell.ThirdPerson;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FromCell.Editor
{
    /// <summary>
    /// Creates an isolated 3D conversion playground. It is deliberately separate from the
    /// existing 2D level builders so the original game remains a working comparison path.
    /// </summary>
    public static class FromCellThirdPersonSceneBuilder
    {
        const string SceneDirectory = "Assets/_Project/Scenes/ThirdPerson";
        const string ScenePath = SceneDirectory + "/3D_Conversion_Test.unity";
        const string Level1ScenePath = SceneDirectory + "/3D_Level_01_FirstSteps.unity";
        const string ConfigPath = "Assets/_Project/ScriptableObjects/GameConfig.asset";

        [MenuItem("From Cell/3D Conversion/Create 3D Conversion Test Scene", false, 0)]
        public static void CreateConversionTestScene()
        {
            EnsureDirectory(SceneDirectory);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("ThirdPersonConversion");
            var environmentRoot = new GameObject("Environment");
            environmentRoot.transform.SetParent(root.transform);

            CreateLighting(root.transform);
            CreateEnvironment(environmentRoot.transform);
            CreateNavigationRoot(environmentRoot.transform);

            GameObject player = CreatePlayer(root.transform);
            CreateCamera(root.transform, player.transform);
            CreateNpc(root.transform, environmentRoot.transform);
            CreateInteractableObject(environmentRoot.transform);
            CreateMobileHud(player);
            CreateEvolutionSystem(root.transform);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(ScenePath);
            Selection.activeGameObject = player;
            Debug.Log(
                "From Cell: 3D conversion test scene created. Press Play, then tap/click the ground to move.");
        }

        [MenuItem("From Cell/3D Conversion/Create 3D Level 1 - First Steps", false, 1)]
        public static void CreateLevel1Scene()
        {
            EnsureDirectory(SceneDirectory);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("ThirdPersonLevel01");
            var environmentRoot = new GameObject("Environment");
            environmentRoot.transform.SetParent(root.transform);
            var contentRoot = new GameObject("LevelContent");
            contentRoot.transform.SetParent(root.transform);

            LevelBlueprint blueprint = Level01.Build();
            CreateLighting(root.transform);
            CreateLevel1Environment(environmentRoot.transform, blueprint);
            CreateNavigationRoot(environmentRoot.transform, new Vector3(360f, 12f, 30f));

            GameObject player = CreatePlayer(
                root.transform,
                new Vector3(blueprint.spawn.x, 0f, 0f));
            CreateCamera(root.transform, player.transform);

            GameObject hud = CreateMobileHud(player);
            var status = CreateText(
                hud.transform,
                "LevelStatus",
                "Preparing Level 1...",
                24,
                new Vector2(0f, -126f),
                new Vector2(1240f, 58f),
                new Vector2(0.5f, 1f),
                TextAnchor.UpperCenter);
            status.color = new Color(0.8f, 0.94f, 1f);

            var inputGate = new GameObject("InputGate");
            inputGate.transform.SetParent(root.transform);
            inputGate.AddComponent<InputGate>();
            root.AddComponent<SaveProgressService>();

            // Reuse the existing authoring tool and ESL catalog so Echo Fox presents the
            // same A1 exercise as the 2D level's gate.
            FromCellEslUiBuilder.BuildEslChallengeOverlay(hud);

            var levelFlow = root.AddComponent<ThirdPersonLevel1Flow>();
            ThirdPersonVillainGate villainGate = CreateLevel1Content(
                contentRoot.transform,
                environmentRoot.transform,
                blueprint,
                levelFlow);
            levelFlow.Configure(villainGate, status, blueprint.requiredCollectibles);

            CreateEvolutionSystem(root.transform);

            EditorSceneManager.SaveScene(scene, Level1ScenePath);
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(Level1ScenePath);
            Selection.activeGameObject = player;
            Debug.Log(
                "From Cell: 3D Level 1 created. Collect all vocabulary gems, clear Echo Fox, then reach the exit.");
        }

        static void CreateLighting(Transform parent)
        {
            var lightGo = new GameObject("Sun");
            lightGo.transform.SetParent(parent);
            lightGo.transform.rotation = Quaternion.Euler(48f, -30f, 0f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.95f, 0.85f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.38f, 0.43f, 0.55f);
        }

        static void CreateEnvironment(Transform parent)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "Ground",
                parent,
                new Vector3(0f, -0.5f, 0f),
                new Vector3(30f, 1f, 22f),
                new Color(0.18f, 0.48f, 0.36f));

            CreatePrimitive(
                PrimitiveType.Cube,
                "Path",
                parent,
                new Vector3(0f, 0.03f, 0f),
                new Vector3(26f, 0.08f, 5f),
                new Color(0.82f, 0.67f, 0.43f));

            CreatePrimitive(
                PrimitiveType.Cube,
                "VillageHouse",
                parent,
                new Vector3(-7f, 1.5f, 5.5f),
                new Vector3(4f, 3f, 3.5f),
                new Color(0.54f, 0.35f, 0.62f));

            CreatePrimitive(
                PrimitiveType.Cube,
                "LessonTower",
                parent,
                new Vector3(7f, 2f, -4.5f),
                new Vector3(3.5f, 4f, 3.5f),
                new Color(0.22f, 0.45f, 0.72f));

            CreatePrimitive(
                PrimitiveType.Cube,
                "GardenWall",
                parent,
                new Vector3(-1f, 0.9f, 8f),
                new Vector3(9f, 1.8f, 0.7f),
                new Color(0.85f, 0.45f, 0.34f));

            for (int i = 0; i < 5; i++)
            {
                float x = -10f + i * 5f;
                CreatePrimitive(
                    PrimitiveType.Cylinder,
                    "Tree_" + (i + 1),
                    parent,
                    new Vector3(x, 1.25f, -7f),
                    new Vector3(0.65f, 1.25f, 0.65f),
                    new Color(0.22f, 0.32f, 0.22f));
                CreatePrimitive(
                    PrimitiveType.Sphere,
                    "TreeCanopy_" + (i + 1),
                    parent,
                    new Vector3(x, 3.1f, -7f),
                    new Vector3(1.8f, 1.8f, 1.8f),
                    new Color(0.3f, 0.68f, 0.38f));
            }
        }

        static void CreateNavigationRoot(Transform environmentRoot, Vector3? sizeOverride = null)
        {
            var navigation = environmentRoot.gameObject.AddComponent<ThirdPersonRuntimeNavMesh>();
            var so = new SerializedObject(navigation);
            so.FindProperty("navMeshSize").vector3Value = sizeOverride ?? new Vector3(40f, 12f, 34f);
            so.FindProperty("agentRadius").floatValue = 0.45f;
            so.FindProperty("agentHeight").floatValue = 1.8f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static GameObject CreatePlayer(Transform parent, Vector3? startPosition = null)
        {
            var player = new GameObject("Player3D");
            player.transform.SetParent(parent);
            player.transform.position = startPosition ?? new Vector3(-9f, 0f, 0f);
            TrySetTag(player, "Player");

            var collider = player.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.9f, 0f);
            collider.height = 1.8f;
            collider.radius = 0.42f;

            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            var agent = player.AddComponent<NavMeshAgent>();
            agent.radius = 0.42f;
            agent.height = 1.8f;
            agent.baseOffset = 0f;
            agent.speed = 3.5f;
            agent.acceleration = 24f;
            agent.angularSpeed = 720f;
            agent.stoppingDistance = 0.1f;
            agent.updateRotation = false;

            var body = CreatePrimitive(
                PrimitiveType.Capsule,
                "Body",
                player.transform,
                new Vector3(0f, 0.9f, 0f),
                new Vector3(0.82f, 0.9f, 0.82f),
                new Color(0.95f, 0.78f, 0.28f));
            RemoveCollider(body);

            var animation = player.AddComponent<ThirdPersonActorAnimation>();
            SetObjectReference(animation, "visualRoot", body.transform);
            var locomotion = player.AddComponent<ThirdPersonLocomotion>();
            SetObjectReference(locomotion, "actorAnimation", animation);
            var interaction = player.AddComponent<ThirdPersonInteractionSystem>();
            var movement = player.AddComponent<ThirdPersonTapToMove>();
            SetObjectReference(movement, "interactionSystem", interaction);

            return player;
        }

        static void CreateCamera(Transform parent, Transform target)
        {
            var cameraGo = new GameObject("ThirdPersonCamera");
            cameraGo.transform.SetParent(parent);
            cameraGo.transform.position = new Vector3(9f, 10f, -9f);
            cameraGo.transform.rotation = Quaternion.Euler(42f, -45f, 0f);

            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.48f, 0.68f, 0.88f);
            camera.fieldOfView = 55f;
            cameraGo.tag = "MainCamera";

            var follow = cameraGo.AddComponent<ThirdPersonCamera>();
            SetObjectReference(follow, "target", target);
            SetVector3(follow, "followOffset", new Vector3(8f, 10f, -8f));
            SetFloat(follow, "zoom", 9f);
        }

        static void CreateNpc(Transform root, Transform environmentRoot)
        {
            var npc = new GameObject("WanderingNPC");
            npc.transform.SetParent(root);
            npc.transform.position = new Vector3(4f, 0f, 1.5f);

            var collider = npc.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.75f, 0f);
            collider.height = 1.5f;
            collider.radius = 0.38f;

            var agent = npc.AddComponent<NavMeshAgent>();
            agent.radius = 0.38f;
            agent.height = 1.5f;
            agent.baseOffset = 0f;
            agent.speed = 1.6f;
            agent.acceleration = 12f;
            agent.angularSpeed = 540f;
            agent.stoppingDistance = 0.15f;
            agent.updateRotation = false;

            var body = CreatePrimitive(
                PrimitiveType.Capsule,
                "Body",
                npc.transform,
                new Vector3(0f, 0.75f, 0f),
                new Vector3(0.74f, 0.75f, 0.74f),
                new Color(0.48f, 0.86f, 0.72f));
            RemoveCollider(body);

            var animation = npc.AddComponent<ThirdPersonActorAnimation>();
            SetObjectReference(animation, "visualRoot", body.transform);
            var wanderer = npc.AddComponent<ThirdPersonNpc>();
            SetObjectReference(wanderer, "actorAnimation", animation);
            var interactable = npc.AddComponent<ThirdPersonInteractable>();
            SetString(interactable, "displayName", "the wandering guide");
            SetString(interactable, "actionLabel", "talk");
            SetFloat(interactable, "interactionRadius", 2.4f);

            var points = new Transform[3];
            Vector3[] positions =
            {
                new Vector3(4f, 0f, 1.5f),
                new Vector3(8f, 0f, 1.5f),
                new Vector3(8f, 0f, 4.5f),
            };
            for (int i = 0; i < positions.Length; i++)
            {
                var point = new GameObject("NPC_PatrolPoint_" + (i + 1));
                point.transform.SetParent(environmentRoot);
                point.transform.position = positions[i];
                points[i] = point.transform;
            }

            var npcSo = new SerializedObject(wanderer);
            var pointsProperty = npcSo.FindProperty("patrolPoints");
            pointsProperty.arraySize = points.Length;
            for (int i = 0; i < points.Length; i++)
                pointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = points[i];
            npcSo.ApplyModifiedPropertiesWithoutUndo();
        }

        static void CreateInteractableObject(Transform parent)
        {
            var sign = CreatePrimitive(
                PrimitiveType.Cube,
                "VocabularySign",
                parent,
                new Vector3(-2f, 1.1f, 2.2f),
                new Vector3(2.2f, 2.2f, 0.3f),
                new Color(0.96f, 0.45f, 0.25f));

            var interactable = sign.AddComponent<ThirdPersonInteractable>();
            SetString(interactable, "displayName", "the vocabulary sign");
            SetString(interactable, "actionLabel", "read");
            SetFloat(interactable, "interactionRadius", 2.8f);
        }

        static GameObject CreateMobileHud(GameObject player)
        {
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("ThirdPersonMobileHUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            var title = CreateText(
                canvasGo.transform,
                "Title",
                "FROM CELL  •  3D CONVERSION",
                28,
                new Vector2(36f, -28f),
                new Vector2(650f, 55f),
                new Vector2(0f, 1f),
                TextAnchor.UpperLeft);
            title.color = new Color(1f, 0.95f, 0.76f);

            var instruction = CreateText(
                canvasGo.transform,
                "Instruction",
                "Tap / click the ground to move  •  Tap a character or object to interact",
                22,
                new Vector2(36f, -78f),
                new Vector2(1000f, 45f),
                new Vector2(0f, 1f),
                TextAnchor.UpperLeft);
            instruction.color = Color.white;

            var prompt = CreateText(
                canvasGo.transform,
                "InteractionPrompt",
                "Tap the ground to move",
                26,
                new Vector2(0f, 34f),
                new Vector2(900f, 58f),
                new Vector2(0.5f, 0f),
                TextAnchor.MiddleCenter);
            prompt.color = new Color(1f, 0.95f, 0.7f);

            var interaction = player.GetComponent<ThirdPersonInteractionSystem>();
            SetObjectReference(interaction, "promptText", prompt);
            return canvasGo;
        }

        static void CreateLevel1Environment(Transform parent, LevelBlueprint blueprint)
        {
            foreach (PlatformDef platform in blueprint.platforms)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    platform.name,
                    parent,
                    new Vector3(platform.position.x, -0.5f, 0f),
                    new Vector3(platform.size.x, 1f, 12f),
                    new Color(0.16f, 0.46f, 0.34f));
            }

            CreatePrimitive(
                PrimitiveType.Cube,
                "DriftPath",
                parent,
                new Vector3(68.5f, 0.03f, 0f),
                new Vector3(153f, 0.08f, 4.5f),
                new Color(0.78f, 0.64f, 0.4f));

            for (int i = 0; i < 16; i++)
            {
                float x = -5f + i * 10f;
                float side = i % 2 == 0 ? -1f : 1f;
                CreatePrimitive(
                    PrimitiveType.Cylinder,
                    "RouteMarker_" + (i + 1),
                    parent,
                    new Vector3(x, 0.75f, side * 5.4f),
                    new Vector3(0.38f, 0.75f, 0.38f),
                    new Color(0.25f, 0.57f, 0.42f));
                CreatePrimitive(
                    PrimitiveType.Sphere,
                    "RouteMarkerCanopy_" + (i + 1),
                    parent,
                    new Vector3(x, 2.1f, side * 5.4f),
                    new Vector3(1.35f, 1.35f, 1.35f),
                    new Color(0.34f, 0.72f, 0.42f));
            }
        }

        static ThirdPersonVillainGate CreateLevel1Content(
            Transform contentRoot,
            Transform environmentRoot,
            LevelBlueprint blueprint,
            ThirdPersonLevel1Flow levelFlow)
        {
            foreach (WindZoneDef wind in blueprint.windZones)
            {
                var zone = new GameObject(wind.name);
                zone.transform.SetParent(contentRoot);
                zone.transform.position = new Vector3(wind.position.x, 1.5f, 0f);
                var trigger = zone.AddComponent<BoxCollider>();
                trigger.size = new Vector3(wind.size.x, 3f, 7f);
                var current = zone.AddComponent<ThirdPersonWindZone>();
                current.Configure(new Vector3(wind.force.x, 0f, wind.force.y));

                for (float x = -wind.size.x * 0.4f; x <= wind.size.x * 0.4f; x += 8f)
                {
                    CreatePrimitive(
                        PrimitiveType.Cube,
                        "WindArrow",
                        zone.transform,
                        new Vector3(x, 0f, 0f),
                        new Vector3(4f, 0.12f, 0.7f),
                        new Color(0.3f, 0.78f, 0.95f));
                }
            }

            foreach (CollectibleDef collectible in blueprint.collectibles)
            {
                var gem = CreatePrimitive(
                    PrimitiveType.Sphere,
                    collectible.name,
                    contentRoot,
                    new Vector3(collectible.position.x, 1.05f, 0f),
                    new Vector3(0.75f, 0.75f, 0.75f),
                    new Color(1f, 0.76f, 0.18f));
                gem.AddComponent<ThirdPersonCollectible>();
            }

            foreach (CheckpointDef checkpoint in blueprint.checkpoints)
            {
                var beacon = CreatePrimitive(
                    PrimitiveType.Cylinder,
                    checkpoint.name,
                    contentRoot,
                    new Vector3(checkpoint.position.x, 1.2f, 0f),
                    new Vector3(0.6f, 1.2f, 0.6f),
                    new Color(0.46f, 0.78f, 1f));
                var trigger = beacon.GetComponent<Collider>();
                trigger.isTrigger = true;
                beacon.AddComponent<ThirdPersonCheckpoint>();
            }

            ThirdPersonVillainGate villainGate = null;
            foreach (VillainGateDef gate in blueprint.villainGates)
            {
                var blocker = CreatePrimitive(
                    PrimitiveType.Cube,
                    "EchoFoxBarrier",
                    environmentRoot,
                    new Vector3(gate.position.x, 1.5f, 0f),
                    new Vector3(0.7f, 3f, 13f),
                    new Color(0.62f, 0.18f, 0.28f));

                var gateRoot = new GameObject("EchoFoxGate");
                gateRoot.transform.SetParent(contentRoot);
                gateRoot.transform.position = new Vector3(gate.position.x - 1.5f, 1.5f, 0f);
                var gateTrigger = gateRoot.AddComponent<BoxCollider>();
                gateTrigger.size = new Vector3(2.5f, 3f, 12f);
                villainGate = gateRoot.AddComponent<ThirdPersonVillainGate>();
                villainGate.Configure(
                    gate.encounterId,
                    blocker.GetComponent<Collider>(),
                    environmentRoot.GetComponent<ThirdPersonRuntimeNavMesh>());

                var fox = CreatePrimitive(
                    PrimitiveType.Capsule,
                    "EchoFox",
                    gateRoot.transform,
                    new Vector3(0f, 0f, 2.6f),
                    new Vector3(1f, 1f, 1f),
                    new Color(0.92f, 0.3f, 0.28f));
                RemoveCollider(fox);
            }

            var finish = new GameObject("GlowingExit");
            finish.transform.SetParent(contentRoot);
            finish.transform.position = new Vector3(blueprint.finish.x, 1.5f, 0f);
            var finishTrigger = finish.AddComponent<BoxCollider>();
            finishTrigger.size = new Vector3(2f, 3f, 13f);
            var finishZone = finish.AddComponent<ThirdPersonFinishZone3D>();
            finishZone.Configure(levelFlow);
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "ExitBeacon",
                finish.transform,
                Vector3.zero,
                new Vector3(1.1f, 1.5f, 1.1f),
                new Color(1f, 0.92f, 0.34f));

            return villainGate;
        }

        static Text CreateText(
            Transform parent,
            string name,
            string content,
            int fontSize,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 anchor,
            TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.text = content;
            return text;
        }

        static GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string name,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            var go = GameObject.CreatePrimitive(primitiveType);
            go.name = name;
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            go.transform.localScale = scale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                // The imported project includes URP resources but does not currently assign
                // an active render pipeline in GraphicsSettings. Prefer a shader compatible
                // with the active pipeline so generated primitives do not render magenta.
                var shader = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null
                    ? Shader.Find("Universal Render Pipeline/Lit")
                    : Shader.Find("Standard");
                if (shader == null)
                    shader = Shader.Find("Unlit/Color");

                var material = new Material(shader) { color = color };
                renderer.sharedMaterial = material;
            }

            return go;
        }

        static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        static void CreateEvolutionSystem(Transform parent)
        {
            var config = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            if (config == null)
                return;

            var evolutionGo = new GameObject("EvolutionSystem");
            evolutionGo.transform.SetParent(parent);
            var evolution = evolutionGo.AddComponent<EvolutionSystem>();
            SetObjectReference(evolution, "gameConfig", config);
        }

        static void EnsureDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string folder = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureDirectory(parent);
            AssetDatabase.CreateFolder(parent, folder);
        }

        static void TrySetTag(GameObject go, string tag)
        {
            try
            {
                go.tag = tag;
            }
            catch (UnityException)
            {
                Debug.LogWarning($"From Cell 3D: tag '{tag}' is not defined. Run the project setup menu first.");
            }
        }

        static void SetObjectReference(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void SetString(Object target, string propertyName, string value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void SetFloat(Object target, string propertyName, float value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            if (property != null)
            {
                property.floatValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static void SetVector3(Object target, string propertyName, Vector3 value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(propertyName);
            if (property != null)
            {
                property.vector3Value = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
#endif