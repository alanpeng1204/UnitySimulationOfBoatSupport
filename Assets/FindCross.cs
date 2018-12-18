using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCross : MonoBehaviour {
    GameObject CrossPoint;
	// Use this for initialization
	void Start () {
        CrossPoint = transform.Find("CrossPoint").gameObject;
	}

    // Update is called once per frame
    void Update()
    {
        CrossPoint.transform.position = Vector3.zero;
        var Line = GetComponent<LineRenderer>();
        if (Line.positionCount < 2)
            return;
        Line.SetPosition(0, this.transform.position);
        Line.SetPosition(1, this.transform.position + this.transform.right * 1500);
        var point0 = Line.GetPosition(0);
        var point1 = Line.GetPosition(1);
        var k = (point1.z - point0.z) / (point1.x - point0.x);
        var b = point0.z - point0.x * k;

        var AIS_Nav = GameObject.Find("My_AIS_Nav").GetComponent<LineRenderer>();
        for (int i = 0; i < AIS_Nav.positionCount - 1; i++)
        {
            var npoint0 = AIS_Nav.GetPosition(i);
            var npoint1 = AIS_Nav.GetPosition(i + 1);
            var diff = CountSide(k, b, npoint0) * CountSide(k, b, npoint1);
            if (diff >= 0)
                continue;

            var nk = (npoint1.z - npoint0.z) / (npoint1.x - npoint0.x);
            var nb = npoint0.z - npoint0.x * nk;
            var ndiff = CountSide(nk, nb, point0) * CountSide(nk, nb, point1);
            if (ndiff >= 0)
                continue;

            var startSide = CountSide(k, b, npoint0);
            var endSide = CountSide(k, b, npoint1);
            var tarX = npoint0.x + (npoint1.x - npoint0.x) * Mathf.Abs(startSide) / (Mathf.Abs(endSide) + Mathf.Abs(startSide));
            var tarZ = nk * (npoint0.x + (npoint1.x - npoint0.x) * Mathf.Abs(startSide) / (Mathf.Abs(endSide) + Mathf.Abs(startSide))) + nb;
            CrossPoint.transform.position = new Vector3(tarX, 40, tarZ);
        }
    }

    float CountSide(float k, float b, Vector3 pos)
    {
        return pos.x * k + b - pos.z;
    }
}
