using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarProject.Car;

public class GameManager : MonoBehaviour
{
    public CarController CC;
    public GameObject needle;
    private float _startPos = 220f, _endPos = -43f, _desiredPos;

    public float carSpeed;

    private void FixedUpdate()
    {
        carSpeed = CC.KPH;
        updateNeedle();
    }

    private void updateNeedle()
    {
        _desiredPos = _startPos - _endPos;
        float temp = carSpeed / 180;
        needle.transform.eulerAngles = new Vector3(0,0, (_startPos - temp * _desiredPos));
    }
}
