using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CautionObject : MonoBehaviour {
        
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var MarkerScript = transform.GetComponent<BoatMarker>();
        if(MarkerScript.getActive)
        {
            transform.parent.Find("CautionObject").gameObject.SetActive(true);
        }
        else
        {
            transform.parent.Find("CautionObject").gameObject.SetActive(false);
        }
    }
}
