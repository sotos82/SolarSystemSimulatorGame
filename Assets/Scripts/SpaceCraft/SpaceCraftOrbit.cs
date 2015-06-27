using UnityEngine;
using System.Collections;

public class SpaceCraftOrbit : MonoBehaviour
{
	public Scales.GravityLevel gravityLevel = Scales.GravityLevel.normal;
	private Vector2 vel2;

	public Vector3 Velocity {
		get { return new Vector3 (vel2.x, 0, vel2.y); }

		set { vel2.Set (value.x, value.z); }
	}

	public float GetVelMagnitude {
		get { return vel2.magnitude * Scales.velmu2kms; }
	}

	private Vector2 pos2;

	public void Initialize (Vector3 initialPos, Vector3 initialVel, float direction)
	{
		pos2.x = initialPos.x;
		pos2.y = initialPos.z;

		transform.position = new Vector3 (pos2.x, 0, pos2.y);

		vel2.x = initialVel.x;
		vel2.y = initialVel.z;
	}

	void FixedUpdate ()
	{
		int layerMask = 1 << 9;
		Collider[] cols = Physics.OverlapSphere (transform.position, 1f, layerMask);

		if (Scales.Pause == false) {
			Integrator.AdaptiveLeapfrog (ref pos2, ref vel2, Time.fixedDeltaTime, 2 * Time.fixedDeltaTime, cols, ref gravityLevel);
			transform.position = new Vector3 (pos2.x, 0, pos2.y);
		}
	}
}