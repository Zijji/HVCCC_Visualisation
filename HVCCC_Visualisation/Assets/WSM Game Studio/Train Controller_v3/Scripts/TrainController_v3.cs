using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.RailroadSystem
{
    public class TrainController_v3 : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private bool _isGrounded = false;
        private bool _onRails = false;
        private float _speed;
        private float _speed_KPH;
        private float _speed_MPH;
        private bool _shouldBeStatic = false;

        //Movement
        private Vector3 _targetVelocity;
        private float _targetSpeed;
        private float _currentSpeed;

        public bool enginesOn = false;
        [Range(0f, 105f)]
        public float maxSpeedKph = 65f;
        [Range(-1f, 1f)]
        public float acceleration = 0f;
        [Range(0f, 1f)]
        public float brake = 0f;
        public List<TrainWheel_v3> wheelsScripts;
        public Sensors sensors;
        public List<Wagon_v3> wagons;
        public List<Light> lights;
        public SFX sfx;
        public Rigidbody backJoint;

        /// <summary>
        /// Train speed at meters per second
        /// </summary>
        public float Speed_MPS
        {
            get { return _speed; }
        }

        /// <summary>
        /// Train speed at Kilometers per second
        /// </summary>
        public float Speed_KPH
        {
            get { return _speed_KPH; }
        }

        /// <summary>
        /// Train speed at Miles per hour
        /// </summary>
        public float Speed_MPH
        {
            get { return _speed_MPH; }
        }

        public bool IsGrounded
        {
            get { return _isGrounded; }
            set { _isGrounded = value; }
        }

        // Use this for initialization
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            ConnectWagons();
        }

        /// <summary>
        /// Fixed update
        /// </summary>
        void FixedUpdate()
        {
            IsGrounded = sensors.leftSensor.grounded || sensors.rightSensor.grounded;
            _onRails = sensors.leftSensor.onRails || sensors.rightSensor.onRails;
            _speed = _rigidbody.velocity.magnitude;
            _speed_MPH = Extension.Convert_MPS_To_MPH(_speed);
            _speed_KPH = Extension.Convert_MPS_To_KPH(_speed);

            SharedMethods.PlaySFX(sfx, _speed_KPH, brake, enginesOn, _isGrounded);

            if (enginesOn)
            {
                _shouldBeStatic = (_speed < 0.1f && acceleration == 0);

                if (_shouldBeStatic != _rigidbody.isKinematic)
                    _rigidbody.isKinematic = _shouldBeStatic;

                SharedMethods.UpdateWheels(wheelsScripts, brake);

                SharedMethods.SpeedControl(_rigidbody, _isGrounded, maxSpeedKph, _speed_KPH, acceleration, brake, _targetVelocity, out _targetVelocity, _currentSpeed, out _currentSpeed, _targetSpeed, out _targetSpeed);

                UpdateWagons();
            }
        }

        /// <summary>
        /// Connect wagons hinges
        /// </summary>
        private void ConnectWagons()
        {
            for (int i = 0; i < wagons.Count; i++)
            {
                if (i == 0)//Connect to train
                    wagons[i].GetComponent<HingeJoint>().connectedBody = backJoint;
                else//Connect to wagon
                    wagons[i].GetComponent<HingeJoint>().connectedBody = wagons[i - 1].backJoint;
            }
        }

        /// <summary>
        /// Update wagons acceleration, brake and max torque
        /// </summary>
        private void UpdateWagons()
        {
            foreach (var wagon in wagons)
            {
                wagon.Acceleration = _onRails ? acceleration : 0f;
                wagon.Brake = _onRails ? brake : 0f;
                wagon.MaxSpeedKph = _onRails ? maxSpeedKph : 0f;
            }
        }

        /// <summary>
        /// Turn lights on/off
        /// </summary>
        public void ToggleLights()
        {
            if (lights != null)
            {
                foreach (Light light in lights)
                {
                    light.enabled = !light.enabled;
                } 
            }

            if (wagons != null)
            {
                foreach (var wagon in wagons)
                {
                    wagon.ToggleLights();
                }
            }
        }

        /// <summary>
        /// play the train horn
        /// </summary>
        public void Honk()
        {
            sfx.hornSFX.Play();
        }
    }
}
