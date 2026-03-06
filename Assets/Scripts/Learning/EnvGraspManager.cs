using System.Collections;
using Agents;
using Controllers;
using Learning.Rewards;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Learning
{
    public class EnvGraspManager : EnvManager
    {
        public Transform box;
        public GameObject pallet;
        public Transform holdTarget;
        public GameObject agentPrefab;
        [FormerlySerializedAs("agentCurrent")] public ArmRobotDiscreteInput current;
        private IRewardFunction armDistance;
        private float maxSteps;
        private bool grasped;

        public override void Initialize(int index)
        {
            if (current != null)
            {
                Destroy(current.gameObject);
                current = null;
            }

            transform.position += new Vector3(0, index * 10f, 0);
            RestartEnv(1);
        }

        public override ArmRobotDiscreteInput RestartEnv(float maxSteps)
        {
            grasped = false;

            var localPosition = new Vector3(0.940999985f, 0.005f, Random.Range(-1f, 1f));
            var rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            if (current == null)
                current = Instantiate(agentPrefab, transform.position + localPosition, rotation, transform)
                    .GetComponent<ArmRobotDiscreteInput>();
            current.Restart(transform.TransformPoint(localPosition), rotation, false);

            // holdTarget.transform.parent = agentCurrent.WheelController.transform;
            holdTarget.transform.localPosition = new Vector3(Random.Range(0.4f, 1.4f), Random.Range(0.25f, 0.5f), Random.Range(-0.5f, 0.5f));

            box.transform.localPosition = new Vector3(Random.Range(0.10f, 0.25f), 0.55f, Random.Range(-0.36f, 0.36f));
            box.transform.localRotation = Quaternion.Euler(new Vector3(180, Random.Range(0, 90), 0));
            box.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            box.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            pallet.transform.localScale = new Vector3(1, Random.Range(0.01f, 3f), 0.75f);

            armDistance = new PotentialReward(ComputeArmDistance, 2.5f);
            StartCoroutine(DelayedReset());

            this.maxSteps = maxSteps == 0 ? 1 : maxSteps;
            return current;
        }

        public IEnumerator DelayedReset()
        {
            yield return new WaitForFixedUpdate();
            armDistance.Reset();
        }

        public override float IncrementalReward()
        {
            return -0.01f / maxSteps
                   + 0.75f * armDistance.Compute();
        }

        public override float SuccessReward()
        {
            return 0.25f;
        }

        public override float FailureReward()
        {
            return 0.0f;
        }

        public override bool IsReset()
        {
            return current.transform.InverseTransformVector(box.transform.position - current.WheelController.transform.position).magnitude > 2.5 ||
                   current.transform.localPosition.magnitude > 2f;
        }

        public override bool IsFailure()
        {
            return false;
        }

        public override bool IsSuccess()
        {
            return current.HandController.GraspActive();
        }

        private float ComputeArmDistance()
        {
            return (current.EndpointController.endPoint.position - box.position).magnitude;
        }
        
        public override ArmRobotDiscreteInput GetCurrentAgent()
        {
            return current;
        }
    }
}