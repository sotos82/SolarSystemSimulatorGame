using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissileManager : MonoBehaviour
{
	private static MissileManager _instance;		//much better with singletons
	public static MissileManager instance {
		get {
			if (_instance == null)		//This will only happen the first time this reference is used.
				_instance = GameObject.FindObjectOfType<MissileManager> ();
			return _instance;
		}
	}

	private GameObject spaceCraft;
	private Vector3 velocity;
	private PlanetaryOrbit POEarth;
	private GameObject earth;
	private GameObject mars;
	private const int maxSpaceCraft = 10;
	private string numOfSpaceCraftsMessage = "You can not have more than " + maxSpaceCraft.ToString () + " Space Crafts at once. You can remove a Space Craft by selecting it and pressing Delete.";

	void Awake ()
	{
		_instance = this;
	}

	void Start ()
	{
		earth = GameObject.Find ("Earth");
		mars = GameObject.Find ("Mars");
		POEarth = earth.GetComponent<PlanetaryOrbit> ();
	}

	public void LaunchSpaceCraft (float speed, float direction, string name)
	{
		if (SpaceCraft.spaceCraftList.Count < maxSpaceCraft) {
			spaceCraft = GameObject.Instantiate (Resources.Load ("Prefabs/MissileHead")) as GameObject;
			spaceCraft.name = name;

			spaceCraft.tag = "SpaceCraft";

			spaceCraft.transform.Find ("MissileMesh").name = "Mesh" + spaceCraft.name;
			spaceCraft.transform.Find ("BB").name = "BB" + spaceCraft.name;
			spaceCraft.transform.parent = transform;

			velocity = POEarth.ParametricVelocity ();
			//print(velocity.magnitude*Scales.velmu2kms);
			velocity += speed * (Quaternion.Euler (0, direction, 0) * velocity).normalized;

			spaceCraft.GetComponent<SpaceCraftOrbit> ().Initialize (earth.transform.position, velocity, direction);
			SpaceCraft sc = spaceCraft.GetComponent<SpaceCraft> ();

			if (name == "Generic") {
				spaceCraft.AddComponent<SpaceCraftInfo> ();
				sc.minVelForCourseCorrection = 0;
				sc.maxVelForCourseCorrection = 16.3f * Scales.kms2velmu;
				sc.allowableNumberOfThrusts = int.MaxValue;
			} else if (name == "Viking") {
				spaceCraft.AddComponent<VikingInfo> ();
				sc.minVelForCourseCorrection = 0f;
				sc.maxVelForCourseCorrection = 2.5f * Scales.kms2velmu;
				sc.allowableNumberOfThrusts = 1;
			} else if (name == "Magellan") {
				spaceCraft.AddComponent<SpaceCraftInfo> ();
				sc.minVelForCourseCorrection = 0;
				sc.maxVelForCourseCorrection = 2.5f * Scales.kms2velmu;
				sc.allowableNumberOfThrusts = 1;
			} else if (name == "Galileo") {
				spaceCraft.AddComponent<SpaceCraftInfo> ();
				sc.minVelForCourseCorrection = 0;
				sc.maxVelForCourseCorrection = 5f * Scales.kms2velmu;
				sc.allowableNumberOfThrusts = 1;
			}
		} else {
			GUIClass.messageQueue.Enqueue (numOfSpaceCraftsMessage);
		}
	}
}
