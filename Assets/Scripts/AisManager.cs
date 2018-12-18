using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AisManager : MonoBehaviour {

    public GameObject RadarSystem;
    public GameObject AisSystem;

    private int state = 0; // 0 -- Radar only; 1 -- AIS only; 2 -- Radar&AIS;

	// Use this for initialization
	void Start () {
        RadarSystem.SetActive(true);
        AisSystem.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha0)) {
            ChangeState(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeState(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeState(2);
        }
    }

    private void ChangeState(int targetState) {
        if(targetState == 0) {
            RadarSystem.SetActive(true);
            AisSystem.SetActive(false);
        }
        else if(targetState == 1) {
            RadarSystem.SetActive(false);
            AisSystem.SetActive(true);
        }
        else {
            RadarSystem.SetActive(true);
            AisSystem.SetActive(true);
        }
    }
}
