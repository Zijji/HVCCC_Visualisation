using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WSMGameStudio.RailroadSystem
{
    public class TrainWheel_v3 : MonoBehaviour
    {
        [Range(0f, 1f)]
        private float _brake = 0f;
        private Rigidbody _rigidbody;

        public float Brake
        {
            get { return _brake; }
            set { _brake = value; }
        }

        /// <summary>
        /// Initialize wheel
        /// </summary>
        void Start()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
            _rigidbody.maxAngularVelocity = GeneralSettings.WheelsMaxAngularVelocity;
            _rigidbody.angularDrag = GeneralSettings.IdleDrag;
        }

        /// <summary>
        /// Fixed update
        /// </summary>
        void FixedUpdate()
        {
            SharedMethods.ApplyBrakes(_rigidbody, _brake, 0f);
            ApplyDownForce();
        }

        /// <summary>
        /// Applies downforce
        /// </summary>
        private void ApplyDownForce()
        {
            //_rigidbody.AddForce((Physics.gravity + _rigidbody.velocity).sqrMagnitude * -transform.up);
            //_rigidbody.AddForce(GeneralSettings.DownForceFactor * _rigidbody.velocity.magnitude * -transform.up);
            _rigidbody.AddForceAtPosition(GeneralSettings.DownForceFactor * _rigidbody.velocity.magnitude * -transform.up, transform.position);
        }
    }
}
