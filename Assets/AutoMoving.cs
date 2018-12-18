using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoving : MonoBehaviour {
    [Range(0, 30)]
    public float Speed = 0.5f;
    public float WaterSize = 800f;
    float offset = 15f;
    Vector3 startPos;
    public Transform ChildShip;
    Transform ChildStart;
	// Use this for initialization
	void Start () {
        startPos = this.transform.position;
        ChildStart = ChildShip;        
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position += this.transform.forward * Speed * Time.deltaTime;
        if (OutOfMap(this.transform.position))
        {
            this.transform.position = startPos;
            ChildShip.position = ChildStart.position;
            ChildShip.rotation = ChildStart.rotation;
        }
	}

    bool OutOfMap(Vector3 pos)
    {
        if (pos.x >= WaterSize - offset || pos.x <= 0 + offset || pos.z >= WaterSize - offset || pos.z <= 0 + offset)
            return true;
        return false;
    }
}
