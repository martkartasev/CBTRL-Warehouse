using Agents;
using Controllers;
using UnityEngine;

namespace Learning
{
    public abstract class EnvManager : MonoBehaviour
    {
        public abstract bool IsFailure();
        public abstract bool IsSuccess();
        public abstract bool IsReset();
        
        
        public abstract float IncrementalReward();
        public abstract float SuccessReward();
        public abstract float FailureReward();

        public abstract void Initialize(int index);
        public abstract ArmRobotDiscreteInput RestartEnv(float maxSteps);

        public abstract ArmRobotDiscreteInput GetCurrentAgent();
    }
}