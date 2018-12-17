using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDisplay : MonoBehaviour {
    public KinematicVehicleSystem.KinematicBasicVehicle MyBoat;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<UnityEngine.UI.Text>().text = MyBoat.CurrentSpeed.ToString("f2");
        GameObject.Find("SoundSystem").GetComponent<SoundManager>().boatSpeed = Mathf.Abs(MyBoat.CurrentSpeed);
    }
}
