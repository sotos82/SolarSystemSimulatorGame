using UnityEngine;
using System.Collections;

public static class Integrator
{
	public static bool displayMessage = false;
	private const float gravityThreshold = 2.5f;

	public static void AdaptiveLeapfrog (ref Vector2 pos2, ref Vector2 vel2, float t1, float nualf, Collider[] cols, ref Scales.GravityLevel gravityLevel)
	{
		int steps = 0, stepsLimit = 1;

		while (true) {
			pos2 += vel2 * t1 * 0.5f; 

			Vector2 force2 = Gravity (pos2, cols);
			float r = pos2.magnitude;
			float fr = force2.magnitude;

			if (fr > (gravityThreshold + 1) * (gravityThreshold + 1))
				gravityLevel = Scales.GravityLevel.high;
			else if (fr > gravityThreshold * gravityThreshold)
				gravityLevel = Scales.GravityLevel.medium;
			else 
				gravityLevel = Scales.GravityLevel.normal;

			float t0 = nualf / Mathf.Sqrt (fr / r);

			if (t1 < t0) {
				vel2 += force2 * t1;  			
				pos2 += vel2 * t1 * 0.5f;   	

				if (++steps == stepsLimit)
					break;

				while (( steps % 2 ) != 0) {
					steps /= 2;
					stepsLimit /= 2;        
					t1 *= 2.0f;
				}    
			} else {
				pos2 -= vel2 * t1 * 0.5f;  	
				
				t1 *= 0.5f; 
				steps *= 2;
				stepsLimit *= 2;
			}
		}
	}

	public static Vector2 Gravity (Vector2 pos2, Collider[] cols)
	{
		Vector2 forceCol = Vector3.zero;

		float e = 0.1f;
        
		foreach (Collider co in cols) {
			if (co.gameObject.name != "Earth") {
				Vector2 colPos = new Vector2 (co.transform.position.x, co.transform.position.z);
				float rCol = Mathf.Sqrt ((colPos - pos2).sqrMagnitude + e * e);
				forceCol += Scales.GM * co.GetComponent<Rigidbody> ().mass * Scales.sunMass2EarthMass * (colPos - pos2) / (rCol * rCol * rCol);
			}
		}
		return forceCol;
	}

	public static void AdaptiveLeapfrog (ref Vector2 pos2, ref Vector2 vel2, float nualf, float t1)
	{
		int steps = 0, stepsLimit = 1;

		while (true) {
			pos2 += vel2 * t1 * 0.5f;  		

			Vector2 force2 = Gravity (pos2);
			float r = pos2.magnitude;
			float fr = force2.magnitude;
			float t0 = nualf / Mathf.Sqrt (fr / r);

			if (t1 < t0) {
				vel2 += force2 * t1;  			
				pos2 += vel2 * t1 * 0.5f;   

				if (++steps == stepsLimit)
					break;

				while ((steps % 2) != 0) {
					steps /= 2;
					stepsLimit /= 2;        
					t1 *= 2.0f;
				}
			} else {
				pos2 -= vel2 * t1 * 0.5f;

				t1 *= 0.5f; 
				steps *= 2;
				stepsLimit *= 2;
			}
		}
	}

	public static Vector2 Gravity (Vector2 pos2)
	{
		float r = pos2.magnitude;
		return (-Scales.GM / (r * r * r) * pos2);
	}
}
