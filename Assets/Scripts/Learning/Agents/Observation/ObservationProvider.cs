using System.Collections.Generic;
using UnityEngine;

namespace Learning.Agents.Observation
{
    public interface ObservationProvider
    {
        public ObservationMessage GetObservation();

        public void Initialize();
    }

    public struct ObservationMessage
    {
        public List<bool> Bools;
        public List<Vector3> Vectors;

        public static ObservationMessage Create(List<bool> bools, List<Vector3> vectors)
        {
            var observationMessage = new ObservationMessage();
            
            observationMessage.Bools = new List<bool>();
            observationMessage.Bools.AddRange(bools);
            
            observationMessage.Vectors = vectors;
            observationMessage.Vectors.AddRange(vectors);
            
            return observationMessage;
        }
    }
}