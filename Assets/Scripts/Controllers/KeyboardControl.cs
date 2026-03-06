using UnityEngine;

namespace Controllers
{
    public class KeyboardControl : MonoBehaviour
    {
        public ArmEndpointController arm;
        public SuctionCupController hand;
        public WheelController wheels;

        public void FixedUpdate()
        {
            var pitch = 0.0f;
            var yaw = 0.0f;
            var distance = 0.0f;
            var left = 0f;
            var right = 0f;
            var suctionEnabled = Input.GetKey("space");
            if (Input.GetKey("w"))
            {
                pitch = 1f;
            }

            if (Input.GetKey("s"))
            {
                pitch = -1f;
            }

            if (Input.GetKey("a"))
            {
                yaw = -1f;
            }

            if (Input.GetKey("d"))
            {
                yaw = 1f;
            }

            if (Input.GetKey("q"))
            {
                distance = 1f;
            }

            if (Input.GetKey("e"))
            {
                distance = -1f;
            }

            if (Input.GetKey(KeyCode.Keypad7)) left += 1000f;
            if (Input.GetKey(KeyCode.Keypad4)) left += -1000f;
            if (Input.GetKey(KeyCode.Keypad8)) right += 1000f;
            if (Input.GetKey(KeyCode.Keypad5)) right += -1000f;

            arm.ApplyControls(pitch, yaw, distance);
            hand.suctionEnabled = suctionEnabled;
            wheels.ApplyControls(left, right);
        }
    }
}