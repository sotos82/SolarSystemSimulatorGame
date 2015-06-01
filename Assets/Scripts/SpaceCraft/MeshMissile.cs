using UnityEngine;
using System.Collections;

public class MeshMissile : MonoBehaviour
{
	private float thresDist;
	private float objectCamDistance = 0;
	float thresCamDist = 0;

	// Use this for initialization
	void Start ()
	{
		thresDist = 80;
		
		gameObject.AddComponent<SphereCollider>();
		transform.GetComponent<SphereCollider>().center = Vector3.zero;
		transform.GetComponent<SphereCollider>().radius = 1;
		gameObject.GetComponent<Collider>().isTrigger = true;
		gameObject.AddComponent<MeshFilter>();

		GetComponent<Renderer>().material = ( (Material)Resources.Load( "Materials/" + name + "Material" ) );
		GetComponent<MeshFilter>().mesh = ( (GameObject)Resources.Load( "Mesh/geo" ) ).GetComponent<MeshFilter>().sharedMesh;
        //GetComponent<MeshFilter>().mesh = ((GameObject)Resources.Load("Mesh/mesh")).GetComponentInChildren<MeshFilter>().mesh;
		
		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		thresCamDist = objectCamDistance - thresDist;
		if(objectCamDistance < thresDist)
		{
			gameObject.GetComponent<Renderer>().enabled = true;
			gameObject.GetComponent<Collider>().enabled = true;
		}
		else if(objectCamDistance >= thresDist)
		{
			gameObject.GetComponent<Renderer>().enabled = false;
			gameObject.GetComponent<Collider>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		float thresCamDistNew = objectCamDistance - thresDist;
		
		if ( thresCamDist * thresCamDistNew < 0 )
		{
			if(objectCamDistance < thresDist)
			{
				gameObject.GetComponent<Renderer>().enabled = true;
				gameObject.GetComponent<Collider>().enabled = true;
			}
			else if(objectCamDistance >= thresDist)
			{
				gameObject.GetComponent<Renderer>().enabled = false;
				gameObject.GetComponent<Collider>().enabled = false;
			}
		}
		thresCamDist = thresCamDistNew;
	}
}
