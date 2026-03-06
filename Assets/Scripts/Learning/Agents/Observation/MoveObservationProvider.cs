using System.Collections.Generic;
using System.Linq;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Learning.Agents.Observation
{
    public class MoveObservationProvider : MonoBehaviour, ObservationProvider
    {
        [FormerlySerializedAs("_agentCurrent")]
        public ArmRobotDiscreteInput current;

        public ArticulationBody _lift;
        public Transform _targetPosition;
        public GameObject _box;

        public void Initialize()
        {
            _lift = GameObject.FindGameObjectsWithTag("lift")
                .MinBy(go => (transform.position - go.transform.position).magnitude)
                .GetComponent<ArticulationBody>();
        }

        public ObservationMessage GetObservation()
        {
            if (current == null) current = GetComponent<EnvManager>().GetCurrentAgent();

            var observationMessage = new ObservationMessage
            {
                Bools = new List<bool>(),
                Vectors = new List<Vector3>()
            };

            observationMessage.Bools.Add(current.HandController.GraspActive()); // 0
            observationMessage.Vectors.AddRange(
                new[]
                {
                    current.EndpointController.GetObservations(), // 1, 2 ,3
                    current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().linearVelocity), // 4, 5, 6
                    current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().angularVelocity) / 2f, // 7, 8, 9
                    current.transform.InverseTransformVector(_box != null && _box.activeSelf ? (_box.transform.position - current.WheelController.transform.position) / 3f : Vector3.zero), // 10, 11, 12
                    current.transform.InverseTransformVector(_box != null && _box.activeSelf ? (_box.transform.position - current.EndpointController.endPoint.position) / 3f : Vector3.zero), // 13, 14, 15
                    current.transform.InverseTransformVector(current.WheelController.transform.position - _lift.transform.position) / 37f, // 16, 17, 18
                    current.transform.InverseTransformVector(_lift.linearVelocity) / 2f, // 19, 20, 21
                    current.transform.InverseTransformVector(_lift.angularVelocity), // 22, 23, 24
                    current.transform.InverseTransformVector(_targetPosition.transform.position - current.WheelController.transform.position) / 37f, // 25, 26, 27
                    current.transform.InverseTransformVector(_targetPosition.transform.position - current.EndpointController.endPoint.position) / 37f, //  28, 29, 30
                    current.transform.localPosition / 18, //  31, 32, 33
                    current.transform.localRotation.eulerAngles / 360, //  34, 35, 36
                });

            return observationMessage;
        }
    }
}