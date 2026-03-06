using System;

namespace Learning.Rewards
{
    public class PotentialReward : IRewardFunction
    {
        private float _previous = 0;
        private readonly Func<float> _metric;
        private readonly float _maxDiff;


        public PotentialReward(Func<float> metricFunction, float maxDiff = 1)
        {
            _maxDiff = maxDiff;
            _metric = metricFunction;
        }


        public float Compute()
        {
            var current = _metric.Invoke();
          
            var progress = _previous - current;
            _previous = current;
            
            return progress / _maxDiff;
        }

        public void Reset()
        {
            _previous = _metric.Invoke();
        }
    }
}