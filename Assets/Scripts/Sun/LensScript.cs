using UnityEngine;
using System.Collections;

public class LensScript : MonoBehaviour
{
    public LensFlare lensFlare;
	public float strength;

	// Use this for initialization
	void Start ()
	{
		strength = 3500f;
        lensFlare = GetComponent<LensFlare>();
		Vector3 heading = gameObject.transform.position - Camera.main.transform.position;
		float dist = heading.magnitude;
        lensFlare.brightness = strength / dist;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 heading = gameObject.transform.position - Camera.main.transform.position;
		float dist = heading.magnitude;
        lensFlare.brightness = Mathf.Clamp(strength / dist, 1, Mathf.Infinity);
	}
}