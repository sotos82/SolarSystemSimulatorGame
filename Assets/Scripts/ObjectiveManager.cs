using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
	private static ObjectiveManager _instance;		//much better with singletons
	public static ObjectiveManager instance {
		get {
			if (_instance == null)		//This will only happen the first time this reference is used.
				_instance = GameObject.FindObjectOfType<ObjectiveManager> ();
			return _instance;
		}
	}

	void Awake ()
	{
		_instance = this;
	}

	private string objectivesCompletedMessage = "Great. You completed all objectives!!!";
	bool objectivesCompletedMessageDisplayed = false;
	private float deltaVelThres = 1.5f;
	private string approachingPlanet = "Your Space Craft is approaching its target but it is moving too fast (|δv|>1.5km/s) relative to the planet!";
	static public bool fiveFirstObjectivesCompleted = false;
	private Queue<string> mQueue;
	private const float shortMarsTripTime = 200; /*days*/

	private List<SpaceCraft> allSpaceCrafts;
	private List<Planet> planetList;

	public delegate bool CheckDelegate (Objective o);

	private const int numberOfObjectives = 7;
	private Objective[] objsList = new Objective[numberOfObjectives];

	public Objective[] ObjsList {
		get { return objsList; }
	}

	public CheckDelegate[] ObjectiveFunction = new CheckDelegate[numberOfObjectives];
	private string[] objectiveGoal = new string[numberOfObjectives] { 
        "Launch a Space Craft",
        "Sent a Viking Space Craft to Mars",
        "Sent a Magellan Space Craft to Venus",
        "Sent a Viking to Mars in less than " + shortMarsTripTime.ToString () + " days",
        "Sent a Galileo Space Craft to Jupiter",
        "Sent a Space Craft beyond Neptune",
        "Put a Galileo Space Craft in orbit around Jupiter"
    };

	public class Objective
	{
		public string ObjectiveGoal { get; set; }

		public string objective { get; set; }

		public CheckDelegate Check;
		public bool isCompleted = false;
		public bool messageDisplayed = false;
		public bool messageIsBeingDisplayed = false;
	}

	void Start ()
	{
		mQueue = GUIClass.messageQueue;

		allSpaceCrafts = SpaceCraft.spaceCraftList;
		planetList = Planet.planetList;

		ObjectiveFunction [0] = Obj1Check;
		ObjectiveFunction [1] = Obj2Check;
		ObjectiveFunction [2] = Obj3Check;
		ObjectiveFunction [3] = Obj4Check;
		ObjectiveFunction [4] = Obj5Check;
		ObjectiveFunction [5] = Obj6Check;
		ObjectiveFunction [6] = Obj7Check;

		for (int i = 0; i < numberOfObjectives; i++) {
			objsList [i] = new Objective ();
			objsList [i].ObjectiveGoal = objectiveGoal [i];
			objsList [i].Check = ObjectiveFunction [i];
		}
	}

	private void CheckObjectives ()
	{
		if (fiveFirstObjectivesCompleted == false) {
			for (int i = 0; i < 5; i++) {
				fiveFirstObjectivesCompleted = true;
				if (objsList [i].isCompleted == false) {
					fiveFirstObjectivesCompleted = false;
					break;
				}
			}
		}
		//mQueue.Enqueue("You unlock the Generic Space Craft which has unlimited fuel!!!");

		bool allObjCompleted = true;
		foreach (Objective o in objsList) {
			if (o.isCompleted == false) {
				allObjCompleted = false;
				o.isCompleted = o.Check (o);
			}
		}

		if (allObjCompleted == true && objectivesCompletedMessageDisplayed == false) {
			mQueue.Enqueue (objectivesCompletedMessage);
			objectivesCompletedMessageDisplayed = true;
		}
	}

	void Update ()
	{
		CheckObjectives ();
	}

	private bool Obj1Check (Objective o)
	{
		if (allSpaceCrafts.Count != 0) {
			mQueue.Enqueue ("You launched your first Space Craft.\n You can click on it to receive info.\n" +
				"Space Crafts older than 10 years will be removed.");
			return true;
		} else
			return false;
	}

	private bool Obj2Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.name == "Viking") {
				float velDifference = planetList [3].GetVelMagnitude - sc.GetVelMagnitude;
				if ((sc.transform.position - planetList [3].transform.position).sqrMagnitude < 380 && velDifference > deltaVelThres && sc.approachingPlanetMessageDisplayed == false) {
					mQueue.Enqueue (approachingPlanet);
					sc.approachingPlanetMessageDisplayed = true;
				}

				if ((sc.transform.position - planetList [3].transform.position).sqrMagnitude < 280) {
					if (sc.TimeActiveInYears * 365 > shortMarsTripTime) {
						if (velDifference < deltaVelThres) {
							mQueue.Enqueue ("Your Space Craft managed to reach Mars safely after a journey of " + Mathf.Round (sc.TimeActiveInYears * 365) + " days.");
							Destroy (sc.gameObject);
							return true;
						} else {
							mQueue.Enqueue ("Your Space Craft approached the Red Planet but It was going fast relatively to the Planet and crashed. Try again!");
							Destroy (sc.gameObject);
							return false;
						}
					}
				}
			}
		}
		return false;
	}

	private bool Obj3Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.name == "Magellan") {
				float velDifference = Mathf.Abs (planetList [1].GetVelMagnitude - sc.GetVelMagnitude);
				if ((sc.transform.position - planetList [1].transform.position).sqrMagnitude < 200 && velDifference > deltaVelThres && sc.approachingPlanetMessageDisplayed == false) {
					mQueue.Enqueue (approachingPlanet);
					sc.approachingPlanetMessageDisplayed = true;
				}
				if ((sc.transform.position - planetList [1].transform.position).sqrMagnitude < 90) {
					if (velDifference < deltaVelThres) {
						mQueue.Enqueue ("Your Space Craft managed to reach Venus after a journey of " +
							Mathf.Round (sc.TimeActiveInYears * 365).ToString () + " days.");
						Destroy (sc.gameObject);
						return true;
					} else {
						mQueue.Enqueue ("Your Space Craft approached Venus but It was going fast relatively to the Planet and crashed. Try again!");
						Destroy (sc.gameObject);
						return false;
					}
				}
			}
		}
		return false;
	}

	private bool Obj4Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.name == "Viking") {
				float velDifference = planetList [3].GetVelMagnitude - sc.GetVelMagnitude;
				if ((sc.transform.position - planetList [3].transform.position).sqrMagnitude < 380 && velDifference > deltaVelThres && sc.approachingPlanetMessageDisplayed == false) {
					mQueue.Enqueue (approachingPlanet);
					sc.approachingPlanetMessageDisplayed = true;
				}
				if ((sc.transform.position - planetList [3].transform.position).sqrMagnitude < 280) {
					if (sc.TimeActiveInYears * 365 <= shortMarsTripTime) {
						if (velDifference < deltaVelThres) {
							o.isCompleted = true;
							objsList [1].isCompleted = true;

							mQueue.Enqueue ("Your Space Craft reached Mars in less than " + shortMarsTripTime.ToString () + " days");
							Destroy (sc.gameObject);
							return true;
						} else {
							mQueue.Enqueue ("Your Space Craft approached the Red Planet in time but It was going fast relatively to the Planet and crashed. Try again!");
							Destroy (sc.gameObject);
							return false;
						}
					}
				}
			}
		}
		return false;
	}

	private bool Obj5Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.name == "Galileo") {
				if ((sc.transform.position - planetList [4].transform.position).sqrMagnitude < 400) {
					o.isCompleted = true;
					mQueue.Enqueue ("Your Galileo Space Craft reached Jupiter");
					Destroy (sc.gameObject);
					return true;
				}
			}
		}
		return false;
	}

	private bool Obj6Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.transform.position.sqrMagnitude > (Scales.solarSystemEdge * Scales.solarSystemEdge)) {
				mQueue.Enqueue ("Your Space Craft managed to reach the boundaries of our Solar System with velocity "
					+ (Mathf.Round (sc.GetVelMagnitude * 100f) / 100f + " km/s").ToString () + " after a long journey of "
					+ (Mathf.Round (sc.TimeActiveInYears * 100f) / 100f).ToString () + " years");
				return true;
			}
		}
		return false;
	}

	private bool Obj7Check (Objective o)
	{
		foreach (SpaceCraft sc in allSpaceCrafts) {
			if (sc.name == "Galileo") {
				if ((sc.transform.position - planetList [4].transform.position).sqrMagnitude < 10000) {
					float i = Mathf.Sign (Vector3.Dot (Vector3.up, Vector3.Cross (sc.transform.position - planetList [4].transform.position, Vector3.right)));
					if (i * sc.i < 0) {
						sc.i = i;
						sc.n++;
					}
					if (sc.n > 4) {
						mQueue.Enqueue ("A couple more...");
					}
					if (sc.n > 8) {
						o.isCompleted = true;
						mQueue.Enqueue ("Great! Your Galileo Space Craft orbits Jupiter");
						return true;
					}
				}
			}
		}
		return false;
	}
}
