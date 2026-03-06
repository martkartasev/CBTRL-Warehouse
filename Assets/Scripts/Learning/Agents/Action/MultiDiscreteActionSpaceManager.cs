using System.Collections.Generic;
using Scripts.VecEnv.Message;
using UnityEngine;

namespace Learning.Agents.Action
{


    public class MultiDiscreteActionSpaceManager : IActionSpace
    {
        public List<int> GetActions(AgentAction actions)
        {
            return new List<int>()
            {
                actions.Discrete[0] - 2,
                actions.Discrete[1] - 2,
                actions.Discrete[2] - 2,
                actions.Discrete[3] - 2,
                actions.Discrete[4] - 2,
                actions.Discrete[5],
            };
        }

        public void Heuristic(in AgentAction actionsOut)
        {
            var discOut = actionsOut.Discrete;

            var pitch = 0;
            var yaw = 0;
            var distance = 0;
            var left = 0;
            var right = 0;
            var suctionEnabled = Input.GetKey("space");
            if (Input.GetKey("w"))
            {
                pitch = 1;
            }

            if (Input.GetKey("s"))
            {
                pitch = -1;
            }

            if (Input.GetKey("a"))
            {
                yaw = -1;
            }

            if (Input.GetKey("d"))
            {
                yaw = 1;
            }

            if (Input.GetKey("q"))
            {
                distance = 1;
            }

            if (Input.GetKey("e"))
            {
                distance = -1;
            }

            if (Input.GetKey(KeyCode.Keypad7)) left = 1;
            if (Input.GetKey(KeyCode.Keypad4)) left = -1;
            if (Input.GetKey(KeyCode.Keypad8)) right = 1;
            if (Input.GetKey(KeyCode.Keypad5)) right = -1;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                pitch = 2 * pitch;
                yaw = 2 * yaw;
                distance = 2 * distance;
                left = 2 * left;
                right = 2 * right;
            }

            discOut[0] = left + 2;
            discOut[1] = right + 2;
            discOut[2] = pitch + 2;
            discOut[3] = yaw + 2;
            discOut[4] = distance + 2;
            discOut[5] = suctionEnabled ? 1 : 0;
        }
    }
}