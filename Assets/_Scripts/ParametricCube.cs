using UnityEngine;

public class ParametricCube : MonoBehaviour
{
    public int band;
    public bool useBuffer;

    public float startScale, scaleMultiplier;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.bandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.frequencyBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
        }
	}
}
