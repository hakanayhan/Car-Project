using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CarProject.CarController;

public class GameManager : MonoBehaviour
{
    public CarController CC;
    public GameObject needle;
    [SerializeField] private Text _kphText;
    public Text _gearText;
    private float _startPos = 20f, _endPos = -200f, _desiredPos;

    public float carSpeed;

    private void FixedUpdate()
    {
        carSpeed = CC.KPH;
        _kphText.text = carSpeed.ToString("0");
        UpdateNeedle();
    }

    private void UpdateNeedle()
    {
        _desiredPos = _startPos - _endPos;
        //float temp = carSpeed / 180;
        float temp = CC.engineRPM / 10000;
        needle.transform.eulerAngles = new Vector3(0,0, (_startPos - temp * _desiredPos));
    }

    public void ChangeGear(string gear)
    {
        _gearText.text = gear;
    }
}
