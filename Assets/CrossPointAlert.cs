using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPointAlert : MonoBehaviour {
    bool k = true;
    float timer = 0.0f;
    public float flashTime = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timer < flashTime)
            timer += Time.deltaTime;
        else
        {
            timer = 0.0f;
            k = !k;
            var Sprite = GetComponent<SpriteRenderer>();
            if (k)
                Sprite.color = new Color(1, 1, 0, 0.5f);
            else
                Sprite.color = new Color(1, 1, 0, 1);
        }
    }
}
