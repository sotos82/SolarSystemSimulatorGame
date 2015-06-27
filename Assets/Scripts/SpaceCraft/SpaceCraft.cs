using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceCraft : MonoBehaviour
{
	public static List<SpaceCraft> spaceCraftList = new List<SpaceCraft> ();
	public bool approachingPlanetMessageDisplayed = false;
	public float angle;
	public float minVelForCourseCorrection;
	public float maxVelForCourseCorrection;
	public int allowableNumberOfThrusts;
	public float n = 0;      //These 2 are used to measure rotation around a planet
	public float i = 1;

	public float GetVelMagnitude {
		get { return sco.GetVelMagnitude; }
	}

	public Vector3 Velocity {
		get { return sco.Velocity; }
		set { sco.Velocity = value; }
	}

	public SpaceCraftOrbit sco;

	public float TimeActiveInYears {
		get { return timeActive * Scales.tmu2y; }
	}

	public float ObjectCamDistance {
		get { return objectCamDistance; }
	}

	private float timeActive = 0;
	private SpaceCraftInfo scInfo;
	public bool isCourseCorrecting;

	public bool IsSelected {
		get { return scInfo.isSelected; }
		set { scInfo.isSelected = value; }
	}

	private float spaceCraftlife = 10 * Scales.y2tmu;
	private float initialVelocity;
	private float initialDirection;
	private float objectCamDistance;

	private void Awake ()
	{
	}

	private void Start ()
	{
		scInfo = GetComponent<SpaceCraftInfo> ();
		sco = GetComponent<SpaceCraftOrbit> ();

		spaceCraftList.Add (this);
	}

	private void Update ()
	{
		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;

		if (timeActive > spaceCraftlife)
			Destroy (gameObject);

		if (Scales.Pause == false)
			timeActive += Time.deltaTime;
	}

	private void OnDestroy ()
	{
		spaceCraftList.Remove (this);
	}
}
