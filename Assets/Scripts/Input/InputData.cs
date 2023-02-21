using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarProject.PlayerInput
{
    [CreateAssetMenu(menuName = "Car Project/Input/Input Data")]
    public class InputData : ScriptableObject
    {
        public float Horizontal;
        public float Vertical;
        public KeyCode HandBrakeKey;
    }
}