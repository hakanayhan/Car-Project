using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarProject.PlayerInput;

namespace CarProject.CarController
{

    public class CarController : MonoBehaviour
    {
        public float _steeringAngle;
        public float KPH;
        public float wheelsRPM;
        public float totalPower;

        public WheelCollider[] wheelsCollider = new WheelCollider[4];
        public Transform[] wheelsTransform = new Transform[4];
        [SerializeField] private Rigidbody rb;

        public bool isBrake, isHandBrake;

        [SerializeField] private CarSettings _carSettings;
        [SerializeField] private InputData _inputData;
        [SerializeField] private GameManager _gm;

        public float engineRPM;
        public float smoothTime = 0.01f;
        public float[] gears;
        public int gearNum = 0;

        public bool reverse;

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
            CalculateEnginePower();
            Accelerate();
            HandBrake();
            UpdateWheelPoses();
            AddDownForce();
            GetFriction();
            
            Shifter();
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

        private void CalculateEnginePower()
        {
            WheelRPM();
            if (!isBrake)
            {
                totalPower = (_carSettings.enginePower.Evaluate(engineRPM) * (gears[gearNum]) * _inputData.Vertical);
            }
            float velocity = 0.0f;
            engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * gears[gearNum]), ref velocity, smoothTime);
            

        }

        private void WheelRPM()
        {
            float sum = 0;
            int R = 0;
            for (int i = 0; i < 4; i++)
            {
                sum += wheelsCollider[i].rpm;
                R++;
            }
            wheelsRPM = (R != 0) ? sum / R : 0;

            if(wheelsRPM < -15 && !reverse)
            {
                reverse = true;
                _gm.ChangeGear("R");
            }else if (wheelsRPM > 15 && reverse)
            {
                reverse = false;
                _gm.ChangeGear(gearNum.ToString());
            }

        }

        private void Accelerate()
        {
            CheckBrake();
            if (!isBrake)
            {
                if (_carSettings.drive == CarSettings.DriveType.rearWheelDrive)
                {
                    for (int i = 2; i < wheelsCollider.Length; i++)
                    {
                        wheelsCollider[i].motorTorque = totalPower / 2;
                    }
                }
                else if (_carSettings.drive == CarSettings.DriveType.frontWheelDrive)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        wheelsCollider[i].motorTorque = totalPower / 2;
                    }
                }
                else if (_carSettings.drive == CarSettings.DriveType.allWheelDrive)
                {
                    for (int i = 0; i < wheelsCollider.Length; i++)
                    {
                        wheelsCollider[i].motorTorque = totalPower / 4;
                    }
                }
            }
            else
            {
                for (int i = 0; i < wheelsCollider.Length; i++)
                {
                    wheelsCollider[i].brakeTorque = _carSettings.brakeForce;
                }
            }
            KPH = rb.velocity.magnitude * 3.6f;
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

        private void CheckBrake()
        {
            if (_inputData.Vertical < 0 && wheelsRPM > 10 || _inputData.Vertical > 0 && wheelsRPM < -10)
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

        private void AddDownForce()
        {
            rb.AddForce(downForceValue * rb.velocity.magnitude * -transform.up);
        }

        private void GetFriction()
        {
            for (int i = 0; i < wheelsCollider.Length; i++){
                WheelHit wheelHit;
                wheelsCollider[i].GetGroundHit(out wheelHit);
            }
        }

        private void Shifter()
        {
            if (!IsGrounded()) return;
            if (_carSettings.gearBox == CarSettings.GearBox.automatic)
            {
                //automatic
                if (engineRPM > _carSettings.maxRPM && gearNum < gears.Length - 1 && !reverse)
                {
                    gearNum++;
                    _gm.ChangeGear(gearNum.ToString());
                }
            }
            else
            {
                //manuel
                if (Input.GetKeyDown(_inputData.GearUpKey))
                {
                    gearNum++;
                    _gm.ChangeGear(gearNum.ToString());
                }
            }

            if (engineRPM < _carSettings.minRPM && gearNum > 1)
            {
                gearNum--;
                _gm.ChangeGear(gearNum.ToString());
            }

        }

        private bool IsGrounded()
        {
            return (wheelsCollider[0].isGrounded && wheelsCollider[1].isGrounded && wheelsCollider[2].isGrounded && wheelsCollider[3].isGrounded);
        }

    }
}