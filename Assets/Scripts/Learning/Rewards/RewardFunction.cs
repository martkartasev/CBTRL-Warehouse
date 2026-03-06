namespace Learning.Rewards
{
    public interface IRewardFunction
    {
        public float Compute();

        public void Reset();
    }
}