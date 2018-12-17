using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMarker : MonoBehaviour {
    public float offset = 10.0f;
    public float yOffset = 0.0f;
    public float alertDistance = 80.0f;
    public float minDistance = 0.0f;
    private Vector3 initScale;

    private bool isActive = false;
    public bool getActive { get{ return isActive; } }
	// Use this for initialization
	void Start () {
        initScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        var userBoatPosition = GameObject.Find("Main Camera").transform.position;
        var thisBoatPosition = transform.parent.position;
        var userBoatDir = GameObject.Find("Main Camera").transform.forward;
        var distance = Vector3.Distance(userBoatPosition, thisBoatPosition);
        var angle = Vector3.Angle(userBoatDir, thisBoatPosition - userBoatPosition);
        var uiDistanceText = transform.Find("Canvas").Find("Distance Text").GetComponent<UnityEngine.TextMesh>();
        var uiImageIcon = transform.Find("Canvas").Find("Boat Icon").GetComponent<UnityEngine.UI.Image>();

        transform.position = thisBoatPosition + (userBoatPosition - thisBoatPosition).normalized * offset;
        transform.position += new Vector3(0, yOffset, 0);
        transform.rotation = Quaternion.LookRotation(thisBoatPosition - userBoatPosition);

        if (distance <= alertDistance && angle <= 60.0f)
        {
            transform.Find("Canvas").gameObject.SetActive(true);
            isActive = true;
            uiDistanceText.text = (distance - minDistance).ToString("f1") + " m";

            var greenPercentage = (distance - minDistance) / (alertDistance - minDistance);
            var redPercentage = 1 - greenPercentage;

            uiDistanceText.color = new Color(redPercentage, greenPercentage, 0.0f);
            //uiImageIcon.color = new Color(redPercentage, greenPercentage, 0.0f);
            uiImageIcon.material.color = new Color(redPercentage, greenPercentage, 0.0f);  
            if (greenPercentage >= 0.5f)
                this.transform.localScale = initScale * greenPercentage;
            else
                this.transform.localScale = initScale * 0.5f;

        }
        else
        {
            transform.Find("Canvas").gameObject.SetActive(false);
            isActive = false;
        }
    }
}
