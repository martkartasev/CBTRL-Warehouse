using UnityEngine;

namespace Controllers
{
    public class ArmEndpointController : MonoBehaviour
    {
        public Transform endPoint;
        public ArticulationBody baseLink;

        private Vector3 min = new Vector3(-45, -175, 0.25f);
        private Vector3 max = new Vector3(14, 175, 0.78f);
        public Transform referenceFrame;

        public float yaw_v = 0f;
        public float pitch_v = 0f;
        public float dist_v = 0f;

        public Vector3 GetObservations()
        {
            return new Vector3(yaw_v, pitch_v, dist_v);
        }

        public void ApplyControls(float pitch, float yaw, float distance)
        {
            ApplyAngleConstraints();

            var position = endPoint.position +
                           referenceFrame.forward * (distance * Time.fixedDeltaTime) +
                           referenceFrame.up * (pitch * Time.fixedDeltaTime) +
                           referenceFrame.right * (yaw * Time.fixedDeltaTime);

            endPoint.forward = referenceFrame.forward;
            var referencePoint = referenceFrame.InverseTransformPoint(position);
            referencePoint.z = Mathf.Max(Mathf.Min(referencePoint.z, max.z), min.z);
            dist_v = 2*Mathf.Clamp01(1 - (max.z - referencePoint.z) / (max.z - min.z))-1;

            endPoint.position = referenceFrame.TransformPoint(referencePoint);
            ApplyAngleConstraints();

            var eulerAnglesY = referenceFrame.transform.localEulerAngles.y;
            eulerAnglesY = eulerAnglesY > 180 ? eulerAnglesY - 360 : eulerAnglesY;
            yaw_v =2*(eulerAnglesY-min.y) / (max.y-min.y)-1;
          
            var eulerAnglesX = referenceFrame.transform.localEulerAngles.x;
            eulerAnglesX = eulerAnglesX > 180 ? eulerAnglesX - 360 : eulerAnglesX;
            pitch_v = 2*Mathf.Clamp01((max.x - eulerAnglesX) / (max.x - min.x))-1;
        }

        private void ApplyAngleConstraints()
        {
            referenceFrame.forward = (endPoint.position - referenceFrame.transform.position).normalized;
            var angles = referenceFrame.localEulerAngles;
            if (angles.y > 180) angles.y -= 360;
            if (angles.x > 180) angles.x -= 360;
            angles.x = Mathf.Max(Mathf.Min(angles.x, max.x), min.x);
            angles.y = Mathf.Max(Mathf.Min(angles.y, max.y), min.y);
            referenceFrame.localRotation = Quaternion.Euler(angles);

            var referencePoint = referenceFrame.InverseTransformPoint(endPoint.position);
            referencePoint.x = 0;
            referencePoint.y = 0;
            endPoint.position = referenceFrame.TransformPoint(referencePoint);
        }
    }
}