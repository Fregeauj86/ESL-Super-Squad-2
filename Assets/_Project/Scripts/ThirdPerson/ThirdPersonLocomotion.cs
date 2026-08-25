using FromCell.Core;
using FromCell.Evolution;
using UnityEngine;
using UnityEngine.AI;

namespace FromCell.ThirdPerson
{
    /// <summary>
    /// Adapts existing evolution movement data to a 3D NavMeshAgent without changing the
    /// source ScriptableObjects or the original 2D player implementation.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ThirdPersonLocomotion : MonoBehaviour
    {
        [SerializeField] float defaultSpeed = 3.5f;
        [SerializeField] float acceleration = 24f;
        [SerializeField] float turnSpeed = 720f;
        [SerializeField] ThirdPersonActorAnimation actorAnimation;

        NavMeshAgent agent;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.acceleration = acceleration;
            agent.angularSpeed = turnSpeed;
        }

        void OnEnable()
        {
            GameSignals.StageApplied += OnStageApplied;
        }

        void OnDisable()
        {
            GameSignals.StageApplied -= OnStageApplied;
        }

        void Start()
        {
            var evolution = FindFirstObjectByType<EvolutionSystem>();
            if (evolution != null && evolution.CurrentStageData != null)
                ApplyStageSettings(evolution.CurrentStageData);
            else
                agent.speed = defaultSpeed;
        }

        void Update()
        {
            if (actorAnimation != null)
                actorAnimation.SetSpeed(agent.velocity.magnitude);
        }

        void OnStageApplied(int stageIndex)
        {
            var evolution = FindFirstObjectByType<EvolutionSystem>();
            if (evolution != null && evolution.CurrentStageData != null)
                ApplyStageSettings(evolution.CurrentStageData);
        }

        public void ApplyStageSettings(EvolutionStageData data)
        {
            if (data == null || agent == null)
                return;

            agent.speed = Mathf.Max(0.5f, data.moveSpeed);
            agent.acceleration = Mathf.Max(1f, data.acceleration);
        }
    }
}