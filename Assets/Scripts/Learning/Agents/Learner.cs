using Agents;
using Controllers;
using Learning.Agents.Action;
using Learning.Agents.Observation;
using Scripts.VecEnv.Core;
using Scripts.VecEnv.Message;
using UnityEngine;

namespace Learning.Agents
{
    public class Learner : GymAgent
    {
        public EnvManager envManager;
        private ArmRobotDiscreteInput _current;
        private IActionSpace _actionSpaceManager = new DiscreteProductActionSpace();
        private ObservationProvider _observationProvider;

        private bool[] _latestMask;

        protected void Awake()
        {
            if (_observationProvider == null) _observationProvider = GetComponent<ObservationProvider>();
        }

        protected void OnEnable()
        {
            _current = null;
            envManager.Initialize(GetGymAgentIndex());
            _observationProvider.Initialize();
        }

        protected override void GymReset()
        {
            _current = envManager.RestartEnv(gymSteps);
        }

        protected override void CollectObservation(ref AgentObservation agentObservation)
        {
            var observationMessage = _observationProvider.GetObservation();
            foreach (var boolValue in observationMessage.Bools)
            {
                agentObservation.AppendContinuous(boolValue ? 1f : 0f);
            }

            foreach (var obs in observationMessage.Vectors)
            {
                agentObservation.AppendContinuous(obs.magnitude <= 1 ? obs : obs.normalized);
            }
        }

        protected override void SetAction(AgentAction agentAction)
        {
            var discActions = _actionSpaceManager.GetActions(agentAction);
            if (agentAction.Discrete[0] != 0)
            {
                _current.SetWheels(discActions[0], discActions[1]);
                _current.SetArmControls(discActions[2], discActions[3], discActions[4]);
                _current.SetHandEnabled(discActions[5] == 1);
            }
        }

        protected override float CollectReward()
        {
            var reward = envManager.IncrementalReward();
            if (envManager.IsSuccess()) reward += envManager.SuccessReward();
            if (envManager.IsFailure()) reward += envManager.FailureReward();
            if(Mathf.Abs(reward) > 1) Debug.Log($"Reward: {reward}");
            return Mathf.Clamp(reward, -1, 1);
        }

        protected override EnvironmentState GymStep()
        {
            if (envManager.IsSuccess()) return EnvironmentState.Done;
            if (envManager.IsFailure()) return EnvironmentState.Done;
            if (envManager.IsReset()) return EnvironmentState.Truncated;

            return EnvironmentState.Running;
        }

        protected override AgentAction ProduceDummyAction(AgentAction dummyAgentAction)
        {
            _actionSpaceManager.Heuristic(dummyAgentAction);
            return dummyAgentAction;
        }
    }
}