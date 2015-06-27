using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceCraftInfo : MonoBehaviour
{
	public bool isSelected = false;
	private GUISkin customSkin;
	private SpaceCraftOrbit AdaptiveLeapfrog;
	private SpaceCraft spaceCraft;
	private List<SpaceCraft> allSpaceCrafts;

	protected virtual void Start ()
	{
		customSkin = Resources.Load ("GUISkin") as GUISkin;

		allSpaceCrafts = SpaceCraft.spaceCraftList;
		AdaptiveLeapfrog = GetComponent<SpaceCraftOrbit> ();
		spaceCraft = GetComponent<SpaceCraft> ();
	}

	protected virtual void Update ()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		Rect rect = new Rect (screenPos.x - 100 / 2, screenPos.y - 100 / 2, 100, 100);
		if (rect.Contains (Input.mousePosition) && Input.GetMouseButtonDown (0)) {
			foreach (SpaceCraft sc in allSpaceCrafts) {
				sc.IsSelected = false;
			}

			isSelected = true;
		}

		if (isSelected == true) {
			if (Input.GetKeyDown (KeyCode.Delete) == true)
				Destroy (gameObject);
		}
	}

	protected virtual void OnGUI ()
	{
		GUI.skin = customSkin;

		Vector2 sizeOfLabel = GUI.skin.textField.CalcSize (new GUIContent (gameObject.name));
		Vector2 screenPos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label (new Rect (screenPos.x - sizeOfLabel.x / 2, Screen.height - screenPos.y, 100, 100), gameObject.name);
		if (isSelected) {
			float vel = AdaptiveLeapfrog.GetVelMagnitude;
			GUI.Label (new Rect (screenPos.x - sizeOfLabel.x / 2 - 5, Screen.height - screenPos.y + 12, 100, 100), 
                (Mathf.Round (vel * 100f) / 100f).ToString () + "km/s");
			if (name == "Viking" || name == "Magellan")
				GUI.Label (new Rect (screenPos.x - sizeOfLabel.x / 2 - 5, Screen.height - screenPos.y + 24, 100, 100), Mathf.Round (spaceCraft.TimeActiveInYears * 365).ToString () + " days");
			else
				GUI.Label (new Rect (screenPos.x - sizeOfLabel.x / 2 - 5, Screen.height - screenPos.y + 24, 100, 100), (Mathf.Round (spaceCraft.TimeActiveInYears * 100) / 100).ToString () + " years");
		}
	}
}
