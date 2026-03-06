using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Learning.Agents.Observation
{
    public class GraspObservationProvider : MonoBehaviour, ObservationProvider
    {
        [FormerlySerializedAs("_agentCurrent")]
        public ArmRobotDiscreteInput current;

        public Transform _box;
        public Transform _holdTarget;


        public ObservationMessage GetObservation()
        {
            if (current == null) current = GetComponent<EnvGraspManager>().current;
            var observationMessage = new ObservationMessage
            {
                Bools = new List<bool>(),
                Vectors = new List<Vector3>()
            };

            observationMessage.Bools.Add(current.HandController.GraspActive()); // 0
            observationMessage.Vectors.AddRange(
                new[]
                {
                    current.EndpointController.GetObservations(), // 1 2 3
                    current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().linearVelocity), // 4 5 6
                    current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().angularVelocity) / 2f, // 7 8 9
                    current.transform.InverseTransformVector(_box.transform.position - current.WheelController.transform.position) / 3f, // 10, 11, 12
                    current.transform.InverseTransformVector(_box.transform.position - current.EndpointController.endPoint.position) / 3f,
                    current.transform.InverseTransformVector(_holdTarget.transform.position - _box.transform.position) / 3f,
                });

            return observationMessage;
        }

        public void Initialize()
        {
        }
    }
}