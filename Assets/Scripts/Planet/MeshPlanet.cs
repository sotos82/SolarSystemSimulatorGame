using UnityEngine;
using System.Collections;

public class MeshPlanet : MonoBehaviour
{
	private float thresDist;
	private float objectCamDistance = 0;
	float thresCamDist = 0;

	void Start ()
	{
		thresDist = 80;

		gameObject.AddComponent<SphereCollider> ();
		transform.GetComponent<SphereCollider> ().radius = 1;
		GetComponent<Collider> ().isTrigger = true;

		gameObject.AddComponent<MeshFilter> ();

		GetComponent<Renderer> ().material = ((Material)Resources.Load ("Materials/" + name + "Material"));
		GetComponent<MeshFilter> ().mesh = ((GameObject)Resources.Load ("Mesh/geo")).GetComponent<MeshFilter> ().sharedMesh;

		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		thresCamDist = objectCamDistance - thresDist;

		if (objectCamDistance < thresDist) {
			gameObject.GetComponent<Renderer> ().enabled = true;
			gameObject.GetComponent<Collider> ().enabled = true;
		} else if (objectCamDistance >= thresDist) {
			gameObject.GetComponent<Renderer> ().enabled = false;
			gameObject.GetComponent<Collider> ().enabled = false;
		}
	}

	void Update ()
	{
		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		float thresCamDistNew = objectCamDistance - thresDist;
		
		if (thresCamDist * thresCamDistNew < 0) {
			if (objectCamDistance < thresDist) {
				gameObject.GetComponent<Renderer> ().enabled = true;
				gameObject.GetComponent<Collider> ().enabled = true;
			} else if (objectCamDistance >= thresDist) {
				gameObject.GetComponent<Renderer> ().enabled = false;
				gameObject.GetComponent<Collider> ().enabled = false;
			}
		}
		thresCamDist = thresCamDistNew;
	}


}
