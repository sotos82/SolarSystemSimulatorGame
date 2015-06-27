using UnityEngine;
using System.Collections;

public class DebugScript : MonoBehaviour
{
	void Update ()
	{
		Debug.DrawLine (Vector3.zero, new Vector3 (5000, 0, 0), Color.red);
		Debug.DrawLine (Vector3.zero, new Vector3 (0, 5000, 0), Color.green);
		Debug.DrawLine (Vector3.zero, new Vector3 (0, 0, 5000), Color.blue);
	}
}
