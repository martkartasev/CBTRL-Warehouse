using UnityEngine;

namespace Controllers
{
    public class WheelController : MonoBehaviour
    {
        public ArticulationBody leftWheel;
        public ArticulationBody rightWheel;
        public ArticulationBody robotBase;

        public void ApplyControls(float left, float right)
        {
            // robotBase.AddForceAtPosition(robotBase.transform.forward * left/4, leftWheel.transform.position, ForceMode.Force);
            // robotBase.AddForceAtPosition(robotBase.transform.forward * right/4, rightWheel.transform.position, ForceMode.Force);

            if (left > 0) left *= 2;
            if (right > 0) right *= 2;
            leftWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, left);
            rightWheel.SetDriveTargetVelocity(ArticulationDriveAxis.X, right);
        }
    }
}