using UnityEngine;
using System.Collections;

public class PlanetRotation : MonoBehaviour
{
	private PlanetaryOrbit PO;

	void Start ()
	{
		PO = transform.parent.gameObject.GetComponent<PlanetaryOrbit>();
		transform.rotation = Quaternion.Euler(PO.Par[4], 0, 0);
	}
	

	void FixedUpdate ()
	{
		if( Scales.Pause == false )
		{
			transform.Rotate( 0, 360f / PO.RP * Time.fixedDeltaTime, 0 , Space.Self );
		}
	}
}
