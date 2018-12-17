using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveRadarInfo : MonoBehaviour {
    float interval = 0.25f;
    float timer = 0.0f;
    List<Transform> drawList;
    public GameObject RadarPoint;

    public float maximunDistance = 200.0f;
    public float radarDisplayLimitation = 50.0f;

	// Use this for initialization
	void Start () {
        drawList = new List<Transform>();
        var allShip = GameObject.Find("AllShip").transform;
        for (int i = 0; i < allShip.childCount; i++)
        {
            drawList.Add(allShip.GetChild(i));
        }
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        var LineManager = transform.Find("RadarLineManager");
        LineManager.transform.localEulerAngles = new Vector3(0, 0, LineManager.transform.localEulerAngles.z +  180* Time.deltaTime);
        if (timer < interval)
        {
            return;
        }
        timer = 0.0f;
        CleanAllPoints();
        foreach(var item in drawList)
        {
            DrawRadarPoint(item.position);
        }
	}



    void DrawRadarPoint(Vector3 pos)
    {
        var length = Vector3.Distance(pos, this.transform.position);
        if (length > maximunDistance)
            return;
        var angle = Vector3.Angle(this.transform.parent.forward, pos - this.transform.parent.position);
        if(Vector3.Cross(this.transform.parent.forward, pos - this.transform.parent.position).y < 0)
        {
            angle *= -1;
        }
        length = length / maximunDistance * radarDisplayLimitation;
        var posY = length * Mathf.Cos(angle * Mathf.Deg2Rad);
        var posX = length * Mathf.Sin(angle * Mathf.Deg2Rad);
        var point = Instantiate(RadarPoint, this.transform.Find("RadarScreen"));
        point.transform.localPosition = new Vector3(posX, posY, -0.001f);
    }

    void CleanAllPoints()
    {
        var RadarScreen = this.transform.Find("RadarScreen");
        for (int i = 0; i < RadarScreen.childCount; i++)
        {
            Destroy(RadarScreen.GetChild(i).gameObject);
        }
    }
}
