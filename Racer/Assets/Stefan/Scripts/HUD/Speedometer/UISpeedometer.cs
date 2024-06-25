using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpeedometer : MonoBehaviour
{
    [Header ("References")]
    private CarData _carData;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI gearText;
    public Transform meterPivot;
    public Image redLineImg;

    [Header ("Settings")]
    public float minMeterRotation;
    public float maxMeterRotation;
    public float meterSmoothSpeed;

    private float meterVelocity;

    [Header ("Car Settings")]
    public int gear;
    public float rpm;
    public float redLine;
    public float maxRpm;
    public float speed;

    public CarData CarData
    {
        get
        {
            if(_carData == null )
            {
                _carData = FindObjectOfType<CarData> ( );
            }
            return _carData;
        }
    }

    private void Update ( )
    {
        FetchInformation ( );
        UpdateSpeedometer ( );
    }

    void FetchInformation ( )
    {
        if ( CarData == null )
            return;

        gear = CarData.gear;
        speed = CarData.speed;
        //maxRpm = CarData.maxRPM;
        rpm = CarData.rpm;
    }

    void UpdateSpeedometer ( )
    {
        //Set speed
        speedText.text = Mathf.RoundToInt (speed).ToString ( );

        //Set Gear
        gearText.text = gear.ToString ( );

        //Set RPMMeter angle
        var newAngle = meterPivot.localEulerAngles;

        float rpmProgress = Mathf.InverseLerp (0, maxRpm, rpm);

        float targetAngle = Mathf.Lerp (minMeterRotation, maxMeterRotation, rpmProgress);

        newAngle.z = Mathf.SmoothDamp (newAngle.z, targetAngle, ref meterVelocity, meterSmoothSpeed);

        meterPivot.localEulerAngles = newAngle;

        //Set Redline


    }
}
