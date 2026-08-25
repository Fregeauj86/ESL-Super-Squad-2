using UnityEngine;

namespace FromCell.Level
{
    public class CheckpointSystem : MonoBehaviour
    {
        public static CheckpointSystem Instance { get; private set; }

        Transform activeCheckpoint;
        Transform defaultSpawn;

        void Awake()
        {
            Instance = this;
            defaultSpawn = transform;
        }

        public void RegisterCheckpoint(Transform checkpoint)
        {
            activeCheckpoint = checkpoint;
        }

        public Transform GetRespawnPoint()
        {
            return activeCheckpoint != null ? activeCheckpoint : defaultSpawn;
        }
    }
}
