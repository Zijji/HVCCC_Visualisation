using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.RailroadSystem
{
    public class Wagon_v3 : MonoBehaviour
    {
        private bool _isGrounded;
        private Rigidbody _rigidbody;
        [Range(0f, 105f)]
        private float _maxSpeedKph = 65f;
        [Range(-1f, 1f)]
        private float _acceleration = 0f;
        [Range(0f, 1f)]
        private float _brake = 0f;
        private float _speed;
        private bool _shouldBeStatic = false;

        //Movement
        private Vector3 _targetVelocity;
        private float _targetSpeed;
        private float _currentSpeed;

        public SFX sfx;
        public List<TrainWheel_v3> wheelsScripts;
        public Sensors sensors;
        public List<Light> lights;
        public Rigidbody backJoint; //Must be rigibody for hinge connection
        public Rigidbody frontJoint;

        public bool IsGrounded
        {
            get { return _isGrounded; }
        }

        public float MaxSpeedKph
        {
            get { return _maxSpeedKph; }
            set { _maxSpeedKph = value; }
        }

        public float Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }

        public float Brake
        {
            get { return _brake; }
            set { _brake = value; }
        }

        /// <summary>
        /// Distance between front and back joints
        /// </summary>
        public float JoinDistance
        {
            get
            {
                if (frontJoint == null || backJoint == null)
                {
                    Debug.Log("Joints not set");
                    return 0f;
                }

                return Mathf.Abs(frontJoint.transform.localPosition.z) + Mathf.Abs(backJoint.transform.localPosition.z); ;
            }
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Physics
        /// </summary>
        void FixedUpdate()
        {
            _isGrounded = sensors.leftSensor.grounded || sensors.rightSensor.grounded;

            _speed = _rigidbody.velocity.magnitude;
            _shouldBeStatic = (_speed < 0.1f && _acceleration == 0);

            if (_shouldBeStatic != _rigidbody.isKinematic)
                _rigidbody.isKinematic = _shouldBeStatic;

            SharedMethods.UpdateWheels(wheelsScripts, _brake);

            SharedMethods.SpeedControl(_rigidbody, _isGrounded, _maxSpeedKph, Extension.Convert_MPS_To_KPH(_speed), _acceleration, _brake, _targetVelocity, out _targetVelocity, _currentSpeed, out _currentSpeed, _targetSpeed, out _targetSpeed);

            SharedMethods.PlaySFX(sfx, Extension.Convert_MPS_To_KPH(_speed), _brake, false, _isGrounded);
        }

        /// <summary>
        /// Turn lights on/off
        /// </summary>
        public void ToggleLights()
        {
            if (lights == null)
                return;

            foreach (Light light in lights)
            {
                light.enabled = !light.enabled;
            }
        }
    }
}
