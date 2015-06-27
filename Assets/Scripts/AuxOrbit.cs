using UnityEngine;
using System.Collections;

public class AuxOrbit : MonoBehaviour
{
	public float Speed { get; set; }

	public float Direction { get; set; }

	private Vector2 initialPos2;
	private Vector2 initialVel2;
	private LineRenderer line;
	private int lineRendererLength;
	private SpaceCraftOrbit AdaptiveLeapfrog;
	private GameObject earth;
	private GameObject planet;
	private PlanetaryOrbit poEarth;
	private PlanetaryOrbit poPlanet;    //planetary orbit

	public GameObject Planet {
		get { return planet; }
		set { planet = value; }
	}

	public PlanetaryOrbit POPlanet {
		get { return poPlanet; }
		set { poPlanet = value; }
	}

	public bool isActive;
	int numOfHelpers = 6;
	float helperSize = 20;
	GameObject[] spaceCraftHelper;
	GameObject[] planetHelper;

	void Start ()
	{
		isActive = false;

		spaceCraftHelper = new GameObject[numOfHelpers];
		planetHelper = new GameObject[numOfHelpers];

		for (int i = 0; i < numOfHelpers; i++) {
			spaceCraftHelper [i] = Instantiate (Resources.Load ("Prefabs/Helper") as GameObject) as GameObject;
			spaceCraftHelper [i].transform.localScale = new Vector3 (helperSize, helperSize, helperSize);
			spaceCraftHelper [i].SetActive (isActive);
		}
		for (int i = 0; i < numOfHelpers; i++) {
			planetHelper [i] = Instantiate (Resources.Load ("Prefabs/Helper") as GameObject) as GameObject;
			planetHelper [i].transform.localScale = new Vector3 (helperSize, helperSize, helperSize);
			planetHelper [i].SetActive (isActive);
		}

		earth = GameObject.Find ("Earth");
		poEarth = earth.GetComponent<PlanetaryOrbit> ();

		lineRendererLength = 40;
		line = gameObject.GetComponent<LineRenderer> () as LineRenderer;
		line.material = Resources.Load ("Materials/LineAux") as Material;
		line.SetWidth (5f, 5f);
		line.SetVertexCount (lineRendererLength);
	}
	
	void Update ()
	{
		line.enabled = isActive;

		foreach (GameObject go in spaceCraftHelper)
			go.SetActive (isActive);

		foreach (GameObject go in planetHelper)
			go.SetActive (isActive);

		if (line.enabled) {
			float t = 0;
			float deltaTime = Mathf.Floor (POPlanet.Par [1] / 90);
			Vector3 velocity = poEarth.ParametricVelocity ();
            
			velocity += Speed * (Quaternion.Euler (0, Direction, 0) * velocity).normalized;

			initialVel2.Set (velocity.x, velocity.z);
			initialPos2.Set (earth.transform.position.x, earth.transform.position.z);

			for (int i = 0; i < lineRendererLength; i++) {
				t += deltaTime;
				line.SetPosition (i, new Vector3 (initialPos2.x, 0, initialPos2.y));
				Integrator.AdaptiveLeapfrog (ref initialPos2, ref initialVel2, 1f, deltaTime);

				for (int j = 0; j < numOfHelpers; j++) {
					if (t == 6f * (j + 1) * deltaTime)
						spaceCraftHelper [j].transform.position = new Vector3 (initialPos2.x, 0, initialPos2.y);
				}

				for (int j = 0; j < numOfHelpers; j++) {
					if (t == 6f * (j + 1) * deltaTime)
						planetHelper [j].transform.position = poPlanet.GetPositionAfterTime (t);
				}
			}
		}
	}
}
