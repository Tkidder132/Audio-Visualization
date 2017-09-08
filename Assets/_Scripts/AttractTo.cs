using UnityEngine;

public class AttractTo : MonoBehaviour
{
    Rigidbody rigidbody;

    public Transform attractedTo;
    public float attractionStrength, maxMagnitude;

	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(attractedTo)
        {
            Vector3 direction = attractedTo.position - transform.position;
            rigidbody.AddForce(attractionStrength * direction);
            if(rigidbody.velocity.magnitude > maxMagnitude)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * maxMagnitude;
            }
        }
	}
}
