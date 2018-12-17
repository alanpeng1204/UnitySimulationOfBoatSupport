using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NavToTarget : MonoBehaviour {
    bool autonav = false;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        var BoatNav = GetComponent<NavMeshAgent>();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            autonav = !autonav;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            SceneManager.LoadScene("Main");
        }
        if (autonav == true)
        {
            if (BoatNav.remainingDistance <= BoatNav.stoppingDistance)
                autonav = false;
            BoatNav.speed = 20f;
            GameObject.Find("SoundSystem").GetComponent<SoundManager>().boatSpeed = 1;
            //this.transform.parent = this.transform;
            //GameObject.Find("MyBoat").transform.position = new Vector3(this.transform.position.x, GameObject.Find("MyBoat").transform.position.y, this.transform.position.z);
            //GameObject.Find("MyBoat").transform.rotation = this.transform.rotation;
        }
        else
        {
            BoatNav.speed = 0.0f;
        }
        //this.transform.position = GameObject.Find("Player").transform.position;
        //this.transform.rotation = GameObject.Find("Player").transform.rotation;
        GameObject.Find("Player").transform.position = this.transform.position;
        GameObject.Find("Player").transform.rotation = this.transform.rotation;
        var Target = GameObject.Find("Target");
        var StartPos = GameObject.Find("LineStartPoint").transform.position;
        var Line = GetComponent<LineRenderer>();
        BoatNav.destination = Target.transform.position;
        Line.positionCount = BoatNav.path.corners.Length;
        if (Line.positionCount > 0)
        {
            Line.SetPosition(0, StartPos);
            Line.startColor = new Color(1, 1, 0, 0.4f);
            Line.endColor = new Color(1, 1, 0, 0.4f);
            for (int i = 1; i < BoatNav.path.corners.Length; i++)
            {
                var nextPosition = BoatNav.path.corners[i];
                nextPosition.y = StartPos.y;
                Line.SetPosition(i, nextPosition);
            }
        }
    }
}
