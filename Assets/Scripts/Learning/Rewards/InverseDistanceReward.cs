using System;
using UnityEngine;

namespace Learning.Rewards
{
    public class InverseDistanceReward : IRewardFunction
    {
        private float initial;
        private float lowest;
        private readonly Func<float> reward;
        private readonly float modifier;

        public InverseDistanceReward(Func<float> rewardFunction, float lowestInitialMod = 1f)
        {
            reward = rewardFunction;
            modifier = lowestInitialMod;
        }

        private void Initialize()
        {
            initial = reward.Invoke() * modifier;
        }

        public float Compute()
        {
            if (initial == 0.0f) Initialize();
            var currentMeasure = reward.Invoke();
            var value = 1 - Math.Max(Mathf.Min(currentMeasure / initial, 1), 0);
            return value;
        }

        public void Reset()
        {
            
        }
    }
}