using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIS_NavLine : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var Line = this.GetComponent<LineRenderer>();
        var BoatNavLine = GameObject.Find("Navigation").GetComponent<LineRenderer>();
        Line.positionCount = BoatNavLine.positionCount;
        Vector3[] buffer = new Vector3[Line.positionCount];
        BoatNavLine.GetPositions(buffer);
        Line.SetPositions(buffer);
	}
}
