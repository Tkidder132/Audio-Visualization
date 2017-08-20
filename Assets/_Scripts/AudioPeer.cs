 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
	AudioSource audioSource;
	public float[] samples = new float[512];
	// Use this for initialization
	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetSpectrumAudioSource ();
	}

	void GetSpectrumAudioSource()
	{
		audioSource.GetSpectrumData (samples, 0, FFTWindow.Blackman);
	}
}
