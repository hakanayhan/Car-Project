using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarProject.PlayerInput;

namespace CarProject.Car
{

    public class CarController : MonoBehaviour
    {
        public float _steeringAngle;

        public WheelCollider[] wheelsCollider = new WheelCollider[4];
        public Transform[] wheelsTransform = new Transform[4];
        [SerializeField] private Rigidbody rb;

        public bool isBrake, isHandBrake;

        [SerializeField] private CarSettings _carSettings;
        [SerializeField] private InputData _inputData;

        public float downForceValue = 50;
        public GameObject centerOfMass;
        public float frontRpm;
        public float rearRpm;

        private void Start()
        {
            rb.centerOfMass = centerOfMass.transform.localPosition;
        }

        private void FixedUpdate()
        {
            Steer();
            Accelerate();
            HandBrake();
            UpdateWheelPoses();
            addDownForce();
            getFriction();

            frontRpm = wheelsCollider[0].rpm;
            rearRpm = wheelsCollider[2].rpm;

        }

        private void Steer()
        {
            _steeringAngle = _carSettings.maxSteerAngle * _inputData.Horizontal;
            for(int i = 0; i < 2; i++) {
                wheelsCollider[i].steerAngle = _steeringAngle;
            }
            
        }

        private void Accelerate()
        {
            checkBrake();
            if (!isBrake)
            {
                for (int i = 2; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].motorTorque = _inputData.Vertical * _carSettings.motorForce;
                }
            }
            else
            {
                for (int i = 0; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].brakeTorque = _carSettings.brakeForce;
                }
            }
        }

        private void UpdateWheelPoses()
        {
            for (int i = 0; i < wheelsCollider.Length; i++)
            {
                UpdateWheelPose(wheelsCollider[i], wheelsTransform[i]);
            }
        }

        private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
        {
            Vector3 _pos = _transform.position;
            Quaternion _quat = _transform.rotation;

            _collider.GetWorldPose(out _pos, out _quat);

            _transform.position = _pos;
            _transform.rotation = _quat;
        }

        private void checkBrake()
        {
            if (_inputData.Vertical < 0 && wheelsCollider[0].rpm > 10 || _inputData.Vertical > 0 && wheelsCollider[0].rpm < -10)
            {
                for (int i = 2; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].motorTorque = 0;
                }
                isBrake = true;
            }
            else
            {
                isBrake = false;
                for (int i = 0; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].brakeTorque = 0;
                }
            }
        }

        private void HandBrake()
        {
            isHandBrake = (Input.GetKey(_inputData.HandBrakeKey) == true) ? true : false;
            if (isHandBrake)
            {
                for (int i = 2; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].brakeTorque = _carSettings.handBrakeForce;
                }
            }
        }

        private void addDownForce()
        {
            rb.AddForce(downForceValue * rb.velocity.magnitude * -transform.up);
        }

        private void getFriction()
        {
            for (int i = 0; i < wheelsCollider.Length; i++){
                WheelHit wheelHit;
                wheelsCollider[i].GetGroundHit(out wheelHit);
            }
        }
    }
}