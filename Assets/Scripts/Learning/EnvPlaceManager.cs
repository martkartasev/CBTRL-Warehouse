using System.Collections;
using Agents;
using Controllers;
using Learning.Rewards;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Learning
{
    public class EnvPlaceManager : EnvManager
    {
        public Transform placeTarget;
        public Transform box;
        public CollisionDetector boxCollisionDetector;

        public GameObject agentPrefab;
        [FormerlySerializedAs("agentCurrent")] public ArmRobotDiscreteInput current;
        private float maxSteps;
        private IRewardFunction distance;

        public override void Initialize(int index)
        {
            if (current != null)
            {
                Destroy(current.gameObject);
                current = null;
            }
            
            transform.position += new Vector3(0, index * 10f, 0);
            RestartEnv(0);
        }

        public override ArmRobotDiscreteInput RestartEnv(float maxSteps)
        {
            var vector3 = new Vector3(1.3f, 0.005f, 0);
            var quaternion = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

            if (current == null)
                current = Instantiate(agentPrefab, transform.TransformPoint(vector3), quaternion, transform).GetComponent<ArmRobotDiscreteInput>();
            current.HandController.UpdateController(false);
            current.Restart(transform.TransformPoint(vector3), quaternion, false);

            StartCoroutine(RestartBoxPos());
            
            this.maxSteps = maxSteps == 0 ? 1 : maxSteps;
            return current;
        }

        private IEnumerator RestartBoxPos()
        {
            yield return new WaitForFixedUpdate();

            box.gameObject.SetActive(false);
            var pos = current.Chain.bodyParts[7].transform;
            boxCollisionDetector.Clear();
            box.rotation = pos.rotation;
            box.position = pos.position + pos.up * 0.03f;
            var rbDy = box.GetComponent<Rigidbody>();
            box.gameObject.SetActive(true);
            current.SetHandEnabled(true);
            current.HandController.UpdateController(true);
            rbDy.linearVelocity = Vector3.zero;
            rbDy.angularVelocity = Vector3.zero;

            do
            {
                placeTarget.localPosition = new Vector3(Random.Range(0, 0.3f), Random.Range(0.2f, 0.6f), Random.Range(-0.5f, 0.5f));
            } while ((box.transform.position - placeTarget.transform.position).magnitude < 0.3f);

            distance = new PotentialReward(ComputeDistance, 1.5f);
            distance.Reset();
        }

        public override float IncrementalReward()
        {
            return 0.75f * distance.Compute() +
                   -0.01f / maxSteps;
        }

        public override float SuccessReward()
        {
            return 0.25f;
        }

        public override float FailureReward()
        {
            return -0.25f;
        }

        public override bool IsFailure()
        {
            return box.GetComponent<FixedJoint>() == null && (boxCollisionDetector.collidedWithStr.Contains("Floor") || boxCollisionDetector.collidedWithStr.Contains("Pallet"));
        }

        public override bool IsReset()
        {
            return current.transform.InverseTransformVector(placeTarget.transform.position - box.transform.position).magnitude > 2.5 ||
                   current.transform.localPosition.magnitude > 2f;
        }

        public override bool IsSuccess()
        {
            return (box.transform.position - placeTarget.transform.position).magnitude < 0.15f;
        }

        private float ComputeDistance()
        {
            return (placeTarget.position - box.position).magnitude;
        }
        
        public override ArmRobotDiscreteInput GetCurrentAgent()
        {
            return current;
        }
    }
}