using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarProject.CarController
{
    [CreateAssetMenu(menuName = "Car Project/Car/Car Settings")]
    public class CarSettings : ScriptableObject
    {
        public enum DriveType
        {
            frontWheelDrive,
            rearWheelDrive,
            allWheelDrive
        }

        public enum GearBox
        {
            automatic,
            manuel
        }
        public float maxRPM, minRPM;

        public DriveType drive;
        public GearBox gearBox;

        public AnimationCurve enginePower;

        public float maxSteerAngle = 30;

        public float brakeForce = 2000;

        public bool isHandBrake = false;
        public float handBrakeForce = 5000;
    }
}