using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SunSimple : MonoBehaviour
{
	void Start ()
	{
		gameObject.GetComponent<SphereCollider>().radius = 1.03f * Scales.solarSystemEdge;
	}
	
	void Update ()
	{
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.transform.parent.tag == "SpaceCraft")
		{
            Destroy(other.transform.parent.gameObject);     //I destroy the parent of the mesh or BB because the SC doesn't have a collider (double click issue)
		}
	}
}
