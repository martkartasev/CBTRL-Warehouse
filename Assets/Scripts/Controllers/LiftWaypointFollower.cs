using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class LiftWaypointFollower : MonoBehaviour
    {
        public List<Transform> waypoints;
        public ArticulationBody body;
        public ArticulationBody frontWheel_L_Vel;
        public ArticulationBody frontWheel_R_Vel;
        public ArticulationBody backWheel_Angle;
        public ArticulationBody backWheel_Vel;
        public float maxTime = 30;
        public int restartWaypoint = -1;
        private int currentWaypoint = 0;

        private void Start()
        {
            waypoints.Clear();
            foreach (Transform child in transform.parent)
            {
                var contains = child.transform.name.Contains("Waypoint");
                if (contains) waypoints.Add(child);
            }

            if (restartWaypoint == -1) RestartAtRandomWaypoint();
        }

        public Transform CurrentWayPoint => waypoints[currentWaypoint];

        public Transform NextWayPoint
        {
            get
            {
                if (currentWaypoint < waypoints.Count - 1) return waypoints[currentWaypoint + 1];
                return waypoints[0];
            }
        }

        private float wayPointTime = 0;

        private void FixedUpdate()
        {
            var signedAngle = Vector3.SignedAngle(body.transform.forward, CurrentWayPoint.position - body.transform.position, Vector3.up);
            var reverseAngle = Vector3.SignedAngle(body.transform.forward, body.transform.position - CurrentWayPoint.position, Vector3.up);

            var controlAngle = -signedAngle;
            var controlSpeed = 800;

            if (Mathf.Abs(signedAngle) > Mathf.Abs(reverseAngle))
            {
                controlAngle = reverseAngle;
                controlSpeed = -controlSpeed;
            }

            backWheel_Angle.SetDriveTarget(ArticulationDriveAxis.X, 2 * controlAngle);
            backWheel_Vel.SetDriveTargetVelocity(ArticulationDriveAxis.X, controlSpeed);
            frontWheel_L_Vel.SetDriveTargetVelocity(ArticulationDriveAxis.X, controlSpeed);
            frontWheel_R_Vel.SetDriveTargetVelocity(ArticulationDriveAxis.X, controlSpeed);

            if ((body.transform.position - CurrentWayPoint.position).magnitude < 2f)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Count) currentWaypoint = 0;
                wayPointTime = Time.fixedTime;
            }

            if (Time.fixedTime - wayPointTime > maxTime)
            {
                RestartAtLastWayPoint();
            }
        }

        private void RestartAtLastWayPoint()
        {
            var waypoint = waypoints[waypoints.Count - 1];
            currentWaypoint = 0;
            RestartAtTransform(waypoint);
        }

        public void RestartAtRandomWaypoint()
        {
            var range = restartWaypoint < 0 ? Random.Range(0, waypoints.Count - 1) : restartWaypoint;
            currentWaypoint = range + 1;

            if (currentWaypoint >= waypoints.Count) currentWaypoint = 0;

            var waypoint = waypoints[range];
            RestartAtTransform(waypoint);
        }

        public void RestartAtTransform(Transform waypoint)
        {
            GetComponent<ArticulationChainComponent>().Restart(waypoint.position + Vector3.up * 0.5f, waypoint.rotation);
            wayPointTime = Time.fixedTime;
        }
    }
}