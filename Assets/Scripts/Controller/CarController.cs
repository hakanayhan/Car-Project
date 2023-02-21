using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarProject.PlayerInput;

namespace CarProject.Car
{

    public class CarController : MonoBehaviour
    {
        public float _steeringAngle;

        public WheelCollider frontRightW, frontLeftW;
        public WheelCollider rearRightW, rearLeftW;
        public Transform frontRightT, frontLeftT;
        public Transform rearRightT, rearLeftT;

        public bool isBrake, isHandBrake;

        [SerializeField] private CarSettings _carSettings;
        [SerializeField] private InputData _inputData;


        public float frontRpm;
        public float rearRpm;

        private void FixedUpdate()
        {
            Steer();
            Accelerate();
            UpdateWheelPoses();
            HandBrake();

            frontRpm = frontRightW.rpm;
            rearRpm = rearRightW.rpm;

        }

        private void Steer()
        {
            _steeringAngle = _carSettings.maxSteerAngle * _inputData.Horizontal;
            frontLeftW.steerAngle = _steeringAngle;
            frontRightW.steerAngle = _steeringAngle;
        }

        private void Accelerate()
        {
            checkBrake();
            if (!isBrake)
            {
                rearLeftW.motorTorque = _inputData.Vertical * _carSettings.motorForce;
                rearRightW.motorTorque = _inputData.Vertical * _carSettings.motorForce;
            }
            else
            {
                rearLeftW.brakeTorque = _carSettings.brakeForce;
                rearRightW.brakeTorque = _carSettings.brakeForce;
                frontLeftW.brakeTorque = _carSettings.brakeForce;
                frontRightW.brakeTorque = _carSettings.brakeForce;
            }
        }

        private void UpdateWheelPoses()
        {
            UpdateWheelPose(frontLeftW, frontLeftT);
            UpdateWheelPose(frontRightW, frontRightT);
            UpdateWheelPose(rearLeftW, rearLeftT);
            UpdateWheelPose(rearRightW, rearRightT);
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
            if (_inputData.Vertical < 0 && frontRightW.rpm > 10 || _inputData.Vertical > 0 && frontRightW.rpm < -10)
            {
                rearLeftW.motorTorque = 0;
                rearRightW.motorTorque = 0;
                isBrake = true;
            }
            else
            {
                isBrake = false;
                rearLeftW.brakeTorque = 0;
                rearRightW.brakeTorque = 0;
                frontLeftW.brakeTorque = 0;
                frontRightW.brakeTorque = 0;
            }
        }

        private void HandBrake()
        {
            isHandBrake = (Input.GetKey(_inputData.HandBrakeKey) == true) ? true : false;
            if (isHandBrake)
            {
                rearLeftW.brakeTorque = _carSettings.handBrakeForce;
                rearRightW.brakeTorque = _carSettings.handBrakeForce;
            }
            else if (!isBrake)
            {
                rearLeftW.brakeTorque = 0;
                rearRightW.brakeTorque = 0;
            }
        }

    }
}