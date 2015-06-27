using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VikingInfo : SpaceCraftInfo
{

	protected override void Start ()
	{
		base.Start ();
	}
	
	protected override void Update ()
	{
		base.Update ();
	}

	protected override void OnGUI ()
	{
		base.OnGUI ();
	}

	private float AngleBetween (Vector3 a, Vector3 b, Vector3 n)
	{
		return Vector3.Angle (a, b) * Mathf.Sign (Vector3.Dot (n, Vector3.Cross (a, b)));
	}
}
