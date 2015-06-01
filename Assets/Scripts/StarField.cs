using UnityEngine;
using System.Collections;

public class StarField : MonoBehaviour
{
    //private Quaternion rotation;

	void Start ()
    {
        //rotation = transform.rotation;
	}
	
	void LateUpdate ()
    {
        //transform.rotation = rotation;
        transform.rotation = Quaternion.identity;
	}
}
