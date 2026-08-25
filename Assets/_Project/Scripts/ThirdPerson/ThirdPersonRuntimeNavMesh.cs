using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Small runtime NavMesh baker for the isolated conversion scene. It uses physics colliders
    /// so generated graybox buildings and environment pieces need no extra authoring package.
    /// A later production pass can replace this with pre-baked navigation data per scene.
    /// </summary>
    public class ThirdPersonRuntimeNavMesh : MonoBehaviour
    {
        [SerializeField] LayerMask geometryMask = ~0;
        [SerializeField] Vector3 navMeshSize = new Vector3(60f, 10f, 60f);
        [SerializeField] float agentRadius = 0.35f;
        [SerializeField] float agentHeight = 1.8f;
        [SerializeField] float maxSlope = 45f;
        [SerializeField] bool buildOnStart = true;

        NavMeshDataInstance dataInstance;
        NavMeshData data;

        void Start()
        {
            if (buildOnStart)
                Build();
        }

        public void Build()
        {
            RemoveExistingData();

            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(
                transform,
                geometryMask,
                NavMeshCollectGeometry.PhysicsColliders,
                0,
                markups,
                sources);

            NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
            settings.agentRadius = agentRadius;
            settings.agentHeight = agentHeight;
            settings.agentClimb = 0.4f;
            settings.agentSlope = maxSlope;

            Bounds bounds = new Bounds(transform.position, navMeshSize);
            data = NavMeshBuilder.BuildNavMeshData(
                settings,
                sources,
                bounds,
                transform.position,
                transform.rotation);

            if (data != null)
                dataInstance = NavMesh.AddNavMeshData(data);
            else
                Debug.LogError("From Cell 3D: Runtime NavMesh build returned no data.");
        }

        void OnDestroy()
        {
            RemoveExistingData();
        }

        void RemoveExistingData()
        {
            if (dataInstance.valid)
                dataInstance.Remove();

            data = null;
        }
    }
}