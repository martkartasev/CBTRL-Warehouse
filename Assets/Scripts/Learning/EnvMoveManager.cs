using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agents;
using Controllers;
using Learning.Rewards;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Learning
{
    public class EnvMoveManager : EnvManager
    {
        public Transform targetPosition;

        public GameObject agentPrefab;
        public GameObject envPrefab;
        public GameObject liftPrefab;

        [FormerlySerializedAs("agentCurrent")] public ArmRobotDiscreteInput current;
        public ArticulationBody lift;

        private IRewardFunction targetDistance;
        private float maxSteps;
        private IRewardFunction distanceReward;
        private CollisionDetector collisions;
        private bool resetBox;

        public override void Initialize(int index)
        {
            if (current != null)
            {
                Destroy(current.gameObject);
                current = null;
            }

            var nrOfIsolatedPhysicsLayers = 23;
            var distanceBetweenEnvs = 47.61f;
            var envIndex = (int)(index / nrOfIsolatedPhysicsLayers);
            var distanceToEnv = new Vector3(envIndex * distanceBetweenEnvs, 0, 0);

            transform.position += distanceToEnv;

            if (index % nrOfIsolatedPhysicsLayers == 0 && index != 0)
            {
                Instantiate(envPrefab, envPrefab.transform.position + distanceToEnv, envPrefab.transform.rotation);
                Instantiate(liftPrefab, liftPrefab.transform.position + distanceToEnv, envPrefab.transform.rotation);
            }

            lift = GameObject.FindGameObjectsWithTag("lift")
                .MinBy(go => (transform.position - go.transform.position).magnitude)
                .GetComponent<ArticulationBody>();

            RestartEnv(0);
            gameObject.SetLayerRecursively(9 + index % nrOfIsolatedPhysicsLayers);
            collisions = lift.GetComponent<CollisionDetector>();
        }

        public override ArmRobotDiscreteInput RestartEnv(float maxSteps)
        {
            var posRot = ComputeNonCollidingPositionRotation();
            var localPosition = posRot.Item1;
            var rotation = posRot.Item2;

            localPosition.y = 0.05f;
            if (current == null)
            {
                current = Instantiate(agentPrefab, transform.position + localPosition, rotation, transform).GetComponent<ArmRobotDiscreteInput>();
                current.SetPhysicsLayers(current.transform.parent.gameObject.layer);
            }

            current.Restart(transform.TransformPoint(localPosition), rotation, false);

            targetPosition.transform.localPosition = ComputeNonCollidingPositionRotation().Item1;
            while (ComputeDistance() < 4f) targetPosition.transform.localPosition = ComputeNonCollidingPositionRotation().Item1;
            targetPosition.transform.localPosition = new Vector3(targetPosition.transform.localPosition.x, 0.05f, targetPosition.transform.localPosition.z);

            distanceReward = new PotentialReward(ComputeDistance, 40);
            StartCoroutine(RestartReward());

            this.maxSteps = maxSteps == 0 ? 1 : maxSteps;
            return current;
        }

        private IEnumerator RestartReward()
        {
            yield return new WaitForFixedUpdate();
            distanceReward.Reset();
        }

        private (Vector3, Quaternion) ComputeNonCollidingPositionRotation()
        {
            Vector3 localPosition;
            Quaternion rotation;
            bool isColliding;

            do
            {
                localPosition = new Vector3(Random.Range(-17, 17), Random.Range(0.1f, 0.8f), Random.Range(-17, 17));
                rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                isColliding = Physics.CheckBox(transform.TransformPoint(localPosition), new Vector3(0.6f, 0.6f, 0.75f), rotation, LayerMask.GetMask("DynamicEnv", "Default"));
            } while (isColliding);

            return (localPosition, rotation);
        }

        public override float IncrementalReward()
        {
            return -0.01f / maxSteps +
                   distanceReward.Compute() * 0.5f;
        }

        public override float SuccessReward()
        {
            return 0.5f;
        }

        public override float FailureReward()
        {
            return -0.4f;
        }


        public override bool IsReset()
        {
            return false;
        }

        public override bool IsFailure()
        {
            return collisions.collidedWith.Any(coll => coll.gameObject.layer == current.gameObject.layer);
        }

        public override bool IsSuccess()
        {
            return (current.WheelController.transform.position - targetPosition.position).magnitude < 0.3f;
        }

        private float ComputeDistance()
        {
            return (current.WheelController.transform.position - targetPosition.position).magnitude;
        }

        public override ArmRobotDiscreteInput GetCurrentAgent()
        {
            return current;
        }
    }
}