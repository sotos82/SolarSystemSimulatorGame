using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour
{
	public static List<Planet> planetList = new List<Planet> ();
	public PlanetaryOrbit po;
	private PlanetInfo plInfo;
	private float planetCameraDistance;

	public float PlanetCameraDistance {
		get { return planetCameraDistance; }
	}

	public float GetVelMagnitude {
		get { return po.GetVelMagnitude (); }
	}

	public bool IsSelected {
		get { return plInfo.isSelected; }
		set { plInfo.isSelected = value; }
	}

	void Awake ()
	{
		plInfo = GetComponent<PlanetInfo> ();
		po = GetComponent<PlanetaryOrbit> ();

		planetList.Add (this);
	}

	void Update ()
	{
		if (GetComponent<Rigidbody> () != null)
			gameObject.GetComponent<Rigidbody> ().mass = Scales.massScale * po.Par [7];

		planetCameraDistance = (Camera.main.transform.position - transform.position).magnitude;
		//sphereCollider
		//Scales.massScale * par[7]
		//Scales.gravityColliderMult * Scales.massScale * par[7]
	}

	private void OnDestroy ()
	{
		planetList.Remove (this);
	}
}
