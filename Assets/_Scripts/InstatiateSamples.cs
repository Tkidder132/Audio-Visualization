using UnityEngine;

public class InstatiateSamples : MonoBehaviour
{
    public GameObject sampleCubePrefab;
    public float maxScale;

    GameObject[] sampleCubes = new GameObject[512];

	// Use this for initialization
	void Start ()
    {
		for(int i = 0; i < 512; i++)
        {
            GameObject instanceSampleCube = (GameObject)Instantiate(sampleCubePrefab);
            instanceSampleCube.transform.position = this.transform.position;
            instanceSampleCube.transform.parent = this.transform;
            instanceSampleCube.name = "SampleCube: " + i;
            float movementAmount = (float)360 / (float)sampleCubes.Length;
            this.transform.eulerAngles = new Vector3(0, (System.Math.Abs(movementAmount) * (-1)) * i, 0);
            instanceSampleCube.transform.position = Vector3.forward * 100;
            sampleCubes[i] = instanceSampleCube;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		for(int i = 0; i < sampleCubes.Length; i++)
        {
            if(sampleCubes[i])
            {
                sampleCubes[i].transform.localScale = new Vector3(10, (AudioPeer.samples[i] * maxScale) + 2, 10);
            }
        }
	}
}
