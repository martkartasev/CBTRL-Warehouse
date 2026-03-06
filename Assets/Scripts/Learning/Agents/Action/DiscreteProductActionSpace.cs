using System.Collections.Generic;
using Scripts.VecEnv.Message;
using UnityEngine;

namespace Learning.Agents.Action
{
    public class DiscreteProductActionSpace : IActionSpace
    {
        public List<int> GetActions(AgentAction action)
        {
            var product = action.Discrete[0];
            var suction = product / 3125;
            product -= suction * 3125;
            var dist = product / 625;
            product -= dist * 625;
            var yaw = product / 125;
            product -= yaw * 125;
            var pitch = product / 25;
            product -= pitch * 25;
            var rightW = product / 5;
            product -= rightW * 5;
            var leftW = product;
            return new List<int>()
            {
                leftW - 2,
                rightW - 2,
                pitch - 2,
                yaw - 2,
                dist - 2,
                suction,
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

            discOut[0] = (left + 2) +
                         (right + 2) * 5 +
                         (pitch + 2) * 25 +
                         (yaw + 2) * 125 +
                         (distance + 2) * 625 +
                         (suctionEnabled ? 1 : 0) * 3125;
        }
    }
}