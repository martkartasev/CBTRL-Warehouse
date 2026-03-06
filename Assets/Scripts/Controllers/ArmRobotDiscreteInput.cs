using System.Collections;
using System.Linq;
using UnityEngine;

namespace Controllers
{
    public class ArmRobotDiscreteInput : MonoBehaviour
    {
        public ArmEndpointController EndpointController;
        public SuctionCupController HandController;
        public WheelController WheelController;
        public ArticulationChainComponent Chain;
        public Collider boxArea;
        
        private bool _suction;
        private int _left;
        private int _right;
        private float _pitch;
        private float _yaw;
        private float _distance;

        public void Restart(Vector3 position, Quaternion rotation, bool suction = false)
        {
            EndpointController.endPoint.localPosition = new Vector3(0, 0.567f, 0.563f);
            HandController.UpdateController(suction);
            Chain.Restart(position, rotation);
            Chain.GetRoot().immovable = true;
            StartCoroutine(DelayedMovable());
            
            _left = 0;
            _right = 0;
            _pitch = 0;
            _yaw = 0;
            _distance = 0;
            _suction = suction;
        }

        private IEnumerator DelayedMovable()
        {
            yield return new WaitForFixedUpdate();
            Chain.GetRoot().immovable = false;
        }

        public void FixedUpdate()
        {
            WheelController.ApplyControls(_left * 1000, _right * 1000);
            EndpointController.ApplyControls(_pitch * 0.25f, _yaw * 0.25f, _distance * 0.25f);
            HandController.suctionEnabled = _suction;
        }

        public void SetHandEnabled(bool suction)
        {
            _suction = suction;
        }

        public void SetWheels(int left, int right)
        {
            _left = left;
            _right = right;
        }

        public void SetArmControls(int pitch, int yaw, int distance)
        {
            _pitch = pitch;
            _yaw = yaw;
            _distance = distance;
        }

        public void SetPhysicsLayers(int gameObjectLayer)
        {
            Chain.bodyParts.SelectMany(ab => ab.GetComponentsInChildren<Transform>(includeInactive: true).ToList()).ToList()
                .FindAll(trns => !trns.name.ToLower().Contains("wheel"))
                .ForEach(ab => ab.gameObject.layer = gameObjectLayer);
        }
    }
}