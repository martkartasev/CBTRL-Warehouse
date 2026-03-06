using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Learning.Agents.Observation
{
    public class PlaceObservationProvider : MonoBehaviour, ObservationProvider
    {
       
        public Transform _placeTarget;
        public Transform _box;
        [FormerlySerializedAs("_agentCurrent")] public ArmRobotDiscreteInput current;
        
        public ObservationMessage GetObservation()
        {
            if (current == null) current = GetComponent<EnvPlaceManager>().current;
            
            var observationMessage = new ObservationMessage
            {
                Bools = new List<bool>(),
                Vectors = new List<Vector3>()
            };

            observationMessage.Bools.Add(current.HandController.GraspActive());
            observationMessage.Vectors.AddRange(new[]
            {
                current.EndpointController.GetObservations(),
                current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().linearVelocity),
                current.transform.InverseTransformVector(current.GetComponent<ArticulationBody>().angularVelocity) / 2f,
                current.transform.InverseTransformVector(_box.transform.position - current.WheelController.transform.position) / 3f,
                current.transform.InverseTransformVector(_box.transform.position - current.EndpointController.endPoint.position) / 3f,
                current.transform.InverseTransformVector(_placeTarget.transform.position - _box.transform.position) / 3f,
            });


            return observationMessage;
        }

        public void Initialize()
        {
            
        }
    }
}