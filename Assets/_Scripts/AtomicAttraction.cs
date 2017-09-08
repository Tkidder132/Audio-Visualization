using UnityEngine;

public class AtomicAttraction : MonoBehaviour
{
    public GameObject atom, attractor;
    public Gradient gradient;
    public Material material;
    private Material[] sharedMaterial;
    private Color[] sharedColor;

    public int[] attractPoints;
    public Vector3 spacingDirection;

    [Range(0, 20)]
    public float spacingBetweenAttractPoints;

    [Range(0, 10)]
    public float scaleAttractPoints;

    GameObject[] attractorArray, atomArray;

    [Range(1, 64)]
    public int amountOfAtomsPerPoint;
    public Vector2 atomScaleMinMax;
    float[] atomScaleSet;
    public float strengthOfAttraction, maxMagnitude, randomPositionDistance;
    public bool useGravity;

    public float audioScaleMultiplier, audioEmissionMultiplier;

    [Range(0.0f, 1.0f)]
    public float threshholdEmission;

    private float[] audioBandEmissionThreshold;
    private float[] audioBandEmissionColor;
    private float[] audioBandScale;

    public enum emissionThreshold { Buffered, NoBuffer };
    public emissionThreshold _emissionThreshold = new emissionThreshold();

    public enum emissionColor { Buffered, NoBuffer };
    public emissionColor _emissionColor = new emissionColor();

    public enum atomScale { Buffered, NoBuffer };
    public atomScale _atomScale = new atomScale();

    private void OnDrawGizmos()
    {
        for(int i = 0; i < attractPoints.Length; i++)
        {
            float evaluateStep = 1.0f / attractPoints.Length;
            //Color color = gradient.Evaluate(evaluateStep * i);
            //Gizmos.color = color;
            Gizmos.color = gradient.Evaluate(Mathf.Clamp(evaluateStep * attractPoints[i], 0, 7));

            Vector3 position = new Vector3(transform.position.x + (spacingBetweenAttractPoints * i * spacingDirection.x),
                                           transform.position.y + (spacingBetweenAttractPoints * i * spacingDirection.y),
                                           transform.position.z + (spacingBetweenAttractPoints * i * spacingDirection.z));

            Gizmos.DrawSphere(position, scaleAttractPoints);
        }
    }

    // Use this for initialization
    void Start ()
    {
        attractorArray = new GameObject[attractPoints.Length];
        atomArray = new GameObject[attractPoints.Length * amountOfAtomsPerPoint];
        atomScaleSet = new float[attractPoints.Length * amountOfAtomsPerPoint];

        audioBandEmissionThreshold = new float[8];
        audioBandEmissionColor = new float[8];
        audioBandScale = new float[8];

        sharedMaterial = new Material[8];
        sharedColor = new Color[8];

        int atomCount = 0;
        for(int i = 0; i < attractPoints.Length; i++)
        {
            GameObject attractorInstance = Instantiate(attractor);
            attractorArray[i] = attractorInstance;

            attractorInstance.transform.position = new Vector3(transform.position.x + (spacingBetweenAttractPoints * i * spacingDirection.x),
                                                               transform.position.y + (spacingBetweenAttractPoints * i * spacingDirection.y),
                                                               transform.position.z + (spacingBetweenAttractPoints * i * spacingDirection.z));

            attractorInstance.transform.parent = this.transform;

            attractorInstance.transform.localScale = new Vector3(scaleAttractPoints, scaleAttractPoints, scaleAttractPoints);

            Material matInstance = new Material(material);
            sharedMaterial[i] = matInstance;
            sharedColor[i] = gradient.Evaluate(0.125f * i);

            for(int j = 0; j < amountOfAtomsPerPoint; j++)
            {
                GameObject atomInstance = Instantiate(atom);
                atomArray[atomCount] = atomInstance;
                atomInstance.GetComponent<AttractTo>().attractedTo = attractorArray[i].transform;
                atomInstance.GetComponent<AttractTo>().attractionStrength = strengthOfAttraction;
                atomInstance.GetComponent<AttractTo>().maxMagnitude = maxMagnitude;

                atomInstance.GetComponent<Rigidbody>().useGravity = useGravity;
                atomInstance.transform.position = new Vector3(attractorArray[i].transform.position.x + Random.Range(-randomPositionDistance, randomPositionDistance),
                                                              attractorArray[i].transform.position.y + Random.Range(-randomPositionDistance, randomPositionDistance),
                                                              attractorArray[i].transform.position.z + Random.Range(-randomPositionDistance, randomPositionDistance));
                float randomScale = Random.Range(atomScaleMinMax.x, atomScaleMinMax.y);
                atomScaleSet[atomCount] = randomScale;
                atomInstance.transform.localScale = new Vector3(atomScaleSet[atomCount], atomScaleSet[atomCount], atomScaleSet[atomCount]);
                atomInstance.transform.parent = attractorInstance.transform;

                atomInstance.GetComponent<MeshRenderer>().material = sharedMaterial[i];

                atomCount++;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        SelectAudioValues();
        AtomBehavior();
	}

    void AtomBehavior()
    {
        int atomCount = 0;
        for(int i = 0; i < attractPoints.Length; i++)
        {
            if(audioBandEmissionThreshold[attractPoints[i]] >= threshholdEmission)
            {
                Color audioColor = new Color(sharedColor[i].r * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier,
                                             sharedColor[i].g * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier,
                                             sharedColor[i].b * audioBandEmissionColor[attractPoints[i]] * audioEmissionMultiplier,
                                             1);
                sharedMaterial[i].SetColor("_EmissionColor", audioColor);
            }
            else
            {
                Color audioColor = new Color(0, 0, 0, 1);
                sharedMaterial[i].SetColor("_EmissionColor", audioColor);
            }


            for(int j = 0; j < amountOfAtomsPerPoint; j++)
            {
                atomArray[atomCount].transform.localScale = new Vector3(atomScaleSet[atomCount] + audioBandScale[attractPoints[i]] * audioScaleMultiplier,
                                                                        atomScaleSet[atomCount] + audioBandScale[attractPoints[i]] * audioScaleMultiplier,
                                                                        atomScaleSet[atomCount] + audioBandScale[attractPoints[i]] * audioScaleMultiplier);
                atomCount++;
            }
        }
    }

    void SelectAudioValues()
    {
        for (int i = 0; i < 8; i++)
        {
            audioBandEmissionThreshold[i] = _emissionThreshold == emissionThreshold.Buffered ? AudioPeer.audioBandBuffer[i] : AudioPeer.audioBand[i];
            audioBandEmissionColor[i] = _emissionThreshold == emissionThreshold.Buffered ? AudioPeer.audioBandBuffer[i] : AudioPeer.audioBand[i];
            audioBandScale[i] = _atomScale == atomScale.Buffered ? AudioPeer.audioBandBuffer[i] : AudioPeer.audioBand[i];
        }
    }
}
