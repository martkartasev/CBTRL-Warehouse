using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
    public class SuctionCupController : MonoBehaviour
    {
        public List<CollisionDetector> collisionDetectors;
        public ArticulationBody armAb;
        public Rigidbody armRb;
        public bool suctionEnabled;
        public bool suctionPossible;
        private FixedJoint _connectionJoint;

        private void FixedUpdate()
        {
            UpdateController(suctionEnabled);
        }

        public void UpdateController(bool suction)
        {
            suctionEnabled = suction;

            if (collisionDetectors[0].collidedWithC.Count > 0)
            {
                var body = collisionDetectors[0].collidedWithC[0];
                suctionPossible = body != null && collisionDetectors.All(det => det.collidedWithC.Count > 0 && body == det.collidedWithC[0]);
                if (suctionPossible && body.GetComponent<Rigidbody>() != null && _connectionJoint == null)
                {
                    _connectionJoint = body.AddComponent<FixedJoint>();
                    if (armAb != null) _connectionJoint.connectedArticulationBody = armAb;
                    if (armRb != null) _connectionJoint.connectedBody = armRb;
                }
            }
            // else 
            // {
            //     suctionPossible = false; //TODO: Temp not lose connection if colliders dont touch
            // }


            if (!suction && _connectionJoint != null)
            {
                DestroyImmediate(_connectionJoint);
                _connectionJoint = null;
            }
        }

        public bool GraspActive()
        {
            return _connectionJoint != null;
        }
    }
}