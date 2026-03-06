using System;
using UnityEngine;

namespace Agents
{
    public class ArmSimpleIkController : MonoBehaviour
    {
        private Vector3 offSetFromTarget_y = new Vector3(0, 0.237391651f, 0); // 0.0684
        private Vector3 offSetFromTarget_xz = new Vector3(0, 0, -0.0850521f); //-0.07457244 x
        private ArticulationBody link_1;
        private ArticulationBody link_2;
        private ArticulationBody link_3;
        private ArticulationBody link_5;

        public ArticulationChainComponent chain;
        public Transform referenceFrame;
        public Transform assistFrame;
        public Transform targetFrame;

        public void Start()
        {
            link_1 = chain.bodyParts[1];
            link_2 = chain.bodyParts[2];
            link_3 = chain.bodyParts[3];
            link_5 = chain.bodyParts[5];
        }

        private void FixedUpdate()
        {
            UpdateArm(false);
        }

        public void UpdateArm(bool restart)
        {
            var arm2headGlobal = targetFrame.position + offSetFromTarget_y;
            assistFrame.forward = (arm2headGlobal - assistFrame.position).normalized;
            var forward = assistFrame.forward;
            forward.y = 0;
            arm2headGlobal -= forward.normalized * offSetFromTarget_xz.magnitude;
            assistFrame.forward = (arm2headGlobal - assistFrame.position).normalized;

            var arm1 = assistFrame.InverseTransformPoint(link_2.transform.position);
            arm1.x = 0;
            var arm12 = assistFrame.InverseTransformPoint(link_3.transform.position);
            arm12.x = 0;
            var head = assistFrame.InverseTransformPoint(arm2headGlobal);
            head.x = 0;


            var a = 0.4860f;
            var b = 0.4444f;
            var c = (arm1 - head).magnitude;

            var A = Math.Acos((b * b + c * c - a * a) / (2 * b * c)) * Mathf.Rad2Deg;
            var B = Math.Acos((c * c + a * a - b * b) / (2 * a * c)) * Mathf.Rad2Deg;
            var C = Math.Acos((b * b + a * a - c * c) / (2 * a * b)) * Mathf.Rad2Deg;


            //Debug.Log(A + " : " + B + " : " + C + " = " + (A + B + C));

            var f = assistFrame.localEulerAngles.x;
            var y1 = referenceFrame.localEulerAngles.y;
            if (f > 180) f -= 360;
            if (y1 > 180) y1 -= 360;
            
            if (!IsFinite(A) || !IsFinite(B) || !IsFinite(C) || !IsFinite(f) || !IsFinite(y1))
            {
                // Debug.Log("Detected non-finite:" + A + " : " + B + " : " + C + " = " + (A + B + C));
                return;
            }

            link_1.SetDriveTarget(ArticulationDriveAxis.X, Mathf.Clamp(-y1, link_1.xDrive.lowerLimit, link_1.xDrive.upperLimit));
            link_2.SetDriveTarget(ArticulationDriveAxis.X, Mathf.Clamp(90 + (float)(f - A), link_2.xDrive.lowerLimit, link_2.xDrive.upperLimit));
            link_3.SetDriveTarget(ArticulationDriveAxis.X, Mathf.Clamp(105 - (float)C, link_3.xDrive.lowerLimit, link_3.xDrive.upperLimit));
            link_5.SetDriveTarget(ArticulationDriveAxis.X, Mathf.Clamp((float)(75 - B - f), link_5.xDrive.lowerLimit, link_5.xDrive.upperLimit));
        }
        
        static bool IsFinite(double v) => !(double.IsNaN(v) || double.IsInfinity(v));
        static bool IsFinite(float v) => !(float.IsNaN(v) || float.IsInfinity(v));
    }
}