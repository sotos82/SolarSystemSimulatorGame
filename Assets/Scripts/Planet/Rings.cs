using UnityEngine;
using System.Collections;

public class Rings : MonoBehaviour
{	
	void LateUpdate ()
	{
		GetComponent<Renderer>().enabled = transform.parent.GetComponent<Renderer>().enabled;
	}
}
