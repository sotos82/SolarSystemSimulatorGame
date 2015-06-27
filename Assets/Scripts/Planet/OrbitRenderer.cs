using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitRenderer : MonoBehaviour
{
	private PlanetaryOrbit PO;
	private LineRenderer line;
	private int lineRendererLength;
	private float camPlaneDist = 0;
	private float thresDist;

	void Start ()
	{
		thresDist = 70;

		PO = transform.gameObject.GetComponent<PlanetaryOrbit> ();
		
		lineRendererLength = 250;
		if (gameObject.name == "MoonSystem")
			lineRendererLength = 30;

		line = gameObject.AddComponent <LineRenderer> () as LineRenderer;
		line.material = Resources.Load ("Materials/Line") as Material;
		line.SetWidth (2f * PO.Par [3], 2f * PO.Par [3]);
		line.SetVertexCount (lineRendererLength);
		
		if (gameObject.tag == "Moon")
			line.material.mainTextureScale = new Vector2 (25, 1);

		for (int i = 0; i < lineRendererLength; i++)
			line.SetPosition (i, transform.position + PO.ParametricOrbit (2 * Mathf.PI / (lineRendererLength - 1) * i));

		line.GetComponent<Renderer> ().enabled = true;
	}

	void LateUpdate ()
	{
		camPlaneDist = Camera.main.transform.position.y;
	
		if (tag == "Moon")
			for (int i = 0; i < lineRendererLength; i++)
				line.SetPosition (i, transform.parent.position + PO.ParametricOrbit (2 * Mathf.PI / (lineRendererLength - 1) * i));
			
		float scaleLR = Mathf.Abs ((new Vector3 (camPlaneDist, camPlaneDist, camPlaneDist) / thresDist).x);
		float width = Mathf.Min (65f, 1.5f * PO.Par [3] * scaleLR);
		line.SetWidth (width, width);
	}
}
