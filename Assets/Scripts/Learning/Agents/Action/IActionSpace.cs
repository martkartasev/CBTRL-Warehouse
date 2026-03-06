using System.Collections.Generic;
using Scripts.VecEnv.Message;

namespace Learning.Agents.Action
{
    public interface IActionSpace
    {
        public List<int> GetActions(AgentAction action);
        public void Heuristic(in AgentAction actionsOut);
    }
}