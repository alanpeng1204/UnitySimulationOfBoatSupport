using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 声音管理
public class SoundManager : MonoBehaviour {

    public AudioClip engineSound1;
    public AudioClip engineSound2;
    public AudioClip birdsSound1;
    public AudioClip birdsSound2;
    public AudioClip birdsSound3;
    public AudioClip boatSound;

    public AudioSource engineSrcSlow;
    public AudioSource engineSrcFast;
    public AudioSource birdsSrc;
    public AudioSource boatSrc; // 来自其他船的汽笛声

    [Range(0.0f,1.0f)]
    public float boatSpeed = 0f;
    private float maxSpeed = 1.0f;
    private List<AudioClip> birdsSounds = new List<AudioClip>();
    private float countBird = 0.0f;
    private float countBoat = 0.0f;

	// Use this for initialization
	void Start () {
        boatSrc.clip = boatSound;
        birdsSounds.Add(birdsSound1);
        birdsSounds.Add(birdsSound2);
        birdsSounds.Add(birdsSound3);
        engineSrcSlow.clip = engineSound1;
        engineSrcFast.clip = engineSound2;
        engineSrcSlow.Play();
        engineSrcFast.Play();

        countBird = Random.Range(30, 50);
        countBoat = Random.Range(90, 110);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateBoatSpeed();
        PlayEngineSound();

        // Play the birds sound once per minute.
        countBird += Time.deltaTime;
        if(countBird >= 60) {
            countBird = 0;
            PlayBirdSound();
        }

        // Play another boat sound once every two minutes.
        countBoat += Time.deltaTime;
        if(countBoat >= 120) {
            countBoat = 0;
            PlayBoatSound();
        }
	}

    // Play one of the three birds' sound randomly
    private void PlayBirdSound() {
        int foo = (int)Mathf.Floor(Random.Range(0.0f, 3.0f));
        birdsSrc.clip = birdsSounds[foo];
        birdsSrc.Play();
    }

    private void PlayEngineSound() {
        if(boatSpeed <= 0.01f) { // still
            engineSrcSlow.volume = 0.0f;
            engineSrcFast.volume = 0.0f;
        }
        else if(boatSpeed < 0.4f) { // slow
            engineSrcSlow.volume = 1.0f;
            engineSrcFast.volume = 0.0f;
        }
        else if(boatSpeed > 0.6f) { // fast
            engineSrcSlow.volume = 0.0f;
            engineSrcFast.volume = 1.0f;
        }
        else { // transform
            float rate = (boatSpeed - 0.4f) / (0.2f);
            engineSrcSlow.volume = 1 - rate;
            engineSrcFast.volume = rate;
        }
    }

    // Play boat sound(other boat).
    private void PlayBoatSound() {
        boatSrc.Play();
    }

    // TODO: get speed(Abs) of the boat.
    private void UpdateBoatSpeed() {
        //boatSpeed == 
    }
}
