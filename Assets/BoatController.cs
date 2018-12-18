using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour {
    [Range(0, 1)]
    public float MotorAcceleration = 0.4f;
    [Range(0, 1)]
    public float MotorDeceleration = 1f;
    [Range(0, 1)]
    public float SteerAcceleration = 0.4f;
    [Range(0, 1)]
    public float SteerDeceleration = 0.5f;
    [Range(0, 1)]
    public float MotorPower = 0.5f;
    [Range(0, 1)]
    public float SteerPower = 0.6f;
    float Motor = 0.0f;
    float Steer = 0.0f;
	// Use this for initialization
	void Start () {
		
	}

    public float CurrentSpeed
    {
        get { return Motor; }
    }

    // Update is called once per frame
    void Update () {
        var Vaxis = Input.GetAxis("Vertical");
        var Haxis = Input.GetAxis("Horizontal");
        if (Vaxis > 0f)
        {
            Motor = Mathf.Lerp(Motor, Vaxis, MotorAcceleration * Time.deltaTime);
        }
        else if (Vaxis < 0f)
        {
            Motor = Mathf.Lerp(Motor, Vaxis, MotorAcceleration * Time.deltaTime);
        }
        else
        {
            Motor = Mathf.Lerp(Motor, Vaxis, MotorDeceleration * Time.deltaTime);
        }

        if (Haxis == 0f)
        {
            Steer = Mathf.Lerp(Steer, Haxis, SteerDeceleration * Time.deltaTime);
        }
        else
        {
            Steer = Mathf.Lerp(Steer, Haxis, SteerAcceleration * Time.deltaTime);
        }

        this.transform.position = this.transform.position + this.transform.forward * Motor * MotorPower;

        var steer = Steer * (Motor * SteerPower);
        this.transform.Rotate(new Vector3(0f, steer, 0f));
        //this.transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y + steer, 0f));
    }
}
