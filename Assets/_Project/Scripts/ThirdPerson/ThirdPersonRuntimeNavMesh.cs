using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Runtime NavMesh provider for converted scenes. Production scenes attach a pre-baked
    /// NavMeshData asset; graybox scenes keep an asynchronous update fallback while source
    /// collection remains limited to the editor/test path.
    /// </summary>
    public class ThirdPersonRuntimeNavMesh : MonoBehaviour
    {
        [SerializeField] LayerMask geometryMask = ~0;
        [SerializeField] Vector3 navMeshSize = new Vector3(60f, 10f, 60f);
        [SerializeField] float agentRadius = 0.35f;
        [SerializeField] float agentHeight = 1.8f;
        [SerializeField] float maxSlope = 45f;
        [SerializeField] bool buildOnStart = true;
        [SerializeField] bool buildAsynchronously = true;
        [SerializeField] NavMeshData prebuiltNavMesh;

        NavMeshDataInstance dataInstance;
        NavMeshData data;
        AsyncOperation buildOperation;
        bool rebuildRequested;
        bool ownsRuntimeData;

        public bool IsReady =>
            dataInstance.valid &&
            (buildOperation == null || buildOperation.isDone);
        public bool IsBuilding => buildOperation != null && !buildOperation.isDone;
        public bool UsesPrebakedData => prebuiltNavMesh != null;

        void Start()
        {
            if (buildOnStart)
                Build();
        }

        public void Build()
        {
            if (prebuiltNavMesh != null)
            {
                AttachPrebuiltData();
                return;
            }

            // Keep the existing NavMesh serving while its replacement updates. Gate-open
            // rebuilds therefore do not leave agents temporarily off the NavMesh.
            if (IsBuilding)
            {
                rebuildRequested = true;
                return;
            }

            var sources = CollectSources();
            NavMeshBuildSettings settings = GetBuildSettings();
            Bounds bounds = GetBuildBounds();
            if (buildAsynchronously)
            {
                EnsureRuntimeData(settings);
                UnityEngine.Profiling.Profiler.BeginSample("FromCell.NavMesh.StartAsyncUpdate");
                buildOperation = NavMeshBuilder.UpdateNavMeshDataAsync(
                    data,
                    settings,
                    sources,
                    bounds);
                UnityEngine.Profiling.Profiler.EndSample();
                buildOperation.completed += OnBuildCompleted;
                return;
            }

            UnityEngine.Profiling.Profiler.BeginSample("FromCell.NavMesh.BuildSync");
            var replacement = NavMeshBuilder.BuildNavMeshData(
                settings, sources, bounds, transform.position, transform.rotation);
            UnityEngine.Profiling.Profiler.EndSample();
            if (replacement == null)
            {
                Debug.LogError("From Cell 3D: Runtime NavMesh build returned no data.");
                return;
            }

            RemoveExistingData();
            data = replacement;
            ownsRuntimeData = true;
            dataInstance = NavMesh.AddNavMeshData(data, transform.position, transform.rotation);
        }

        /// <summary>
        /// Editor-facing bake entry point. Converted production scenes store this result as a
        /// NavMeshData asset so they attach navigation without scanning colliders at launch.
        /// </summary>
        public NavMeshData BuildPrebakedData()
        {
            return NavMeshBuilder.BuildNavMeshData(
                GetBuildSettings(),
                CollectSources(),
                GetBuildBounds(),
                transform.position,
                transform.rotation);
        }

        void OnBuildCompleted(AsyncOperation operation)
        {
            if (operation != buildOperation)
                return;

            buildOperation = null;
            if (data == null || !dataInstance.valid)
                return;

            if (rebuildRequested)
            {
                rebuildRequested = false;
                Build();
                return;
            }

            Debug.Log("From Cell 3D: asynchronous NavMesh ready.");
        }

        void OnDestroy()
        {
            RemoveExistingData();
        }

        void RemoveExistingData()
        {
            AsyncOperation activeOperation = buildOperation;
            NavMeshData dataToRelease = data;
            bool releaseRuntimeData = ownsRuntimeData && dataToRelease != null;

            if (activeOperation != null)
                activeOperation.completed -= OnBuildCompleted;

            if (dataInstance.valid)
                dataInstance.Remove();

            // UpdateNavMeshDataAsync cannot be cancelled. Keep its target alive until the native
            // operation completes, even if this component/scene is being torn down. The static
            // lambda deliberately captures only the data object, not this component.
            if (releaseRuntimeData)
            {
                if (activeOperation != null && !activeOperation.isDone)
                    activeOperation.completed += _ => UnityEngine.Object.Destroy(dataToRelease);
                else
                    UnityEngine.Object.Destroy(dataToRelease);
            }

            buildOperation = null;
            rebuildRequested = false;
            ownsRuntimeData = false;
            data = null;
        }

        void AttachPrebuiltData()
        {
            if (dataInstance.valid && data == prebuiltNavMesh)
                return;

            RemoveExistingData();
            data = prebuiltNavMesh;
            ownsRuntimeData = false;
            dataInstance = NavMesh.AddNavMeshData(data, transform.position, transform.rotation);
        }

        void EnsureRuntimeData(NavMeshBuildSettings settings)
        {
            if (dataInstance.valid && data != null && ownsRuntimeData)
                return;

            RemoveExistingData();
            data = new NavMeshData(settings.agentTypeID);
            ownsRuntimeData = true;
            dataInstance = NavMesh.AddNavMeshData(data, transform.position, transform.rotation);
        }

        List<NavMeshBuildSource> CollectSources()
        {
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            UnityEngine.Profiling.Profiler.BeginSample("FromCell.NavMesh.CollectSources");
            NavMeshBuilder.CollectSources(
                transform,
                geometryMask,
                NavMeshCollectGeometry.PhysicsColliders,
                0,
                markups,
                sources);
            UnityEngine.Profiling.Profiler.EndSample();
            return sources;
        }

        NavMeshBuildSettings GetBuildSettings()
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
            settings.agentRadius = agentRadius;
            settings.agentHeight = agentHeight;
            settings.agentClimb = 0.4f;
            settings.agentSlope = maxSlope;
            return settings;
        }

        Bounds GetBuildBounds()
        {
            return new Bounds(transform.position, navMeshSize);
        }
    }
}