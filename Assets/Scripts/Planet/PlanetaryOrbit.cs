using UnityEngine;
using System.Collections;

public class PlanetaryOrbit : MonoBehaviour
{
	public float[] Par { get; set; } 		//eccentricity , r_pericenter,  orbital period,  radius, axial tilt, rot pediod longtitude of ascending node

	private float[] CosSinOmega = new float[2];

	public float OP { get; private set; }		//orbital period
	public float RP { get; private set; }		// roation period

	public float GetVelMagnitude ()
	{
		return ParametricVelocity ().magnitude * Scales.velmu2kms;
	}

	private const int N = 300;
	private float[] angleArray = new float[N];
	private float surface;
	private float k;
	private float orbitDt;

	public float Theta {
		get { return ThetaInt (time); }
	}

	public float time;

	void Start ()
	{
		OP = Par [2] * Scales.y2tmu;
		RP = Par [5] * Scales.y2tmu;

		surface = Mathf.Sqrt (-(1 + Par [0]) / Mathf.Pow (-1 + Par [0], 3)) * Mathf.PI * Par [1] * Par [1];
		k = 2 * surface / (Mathf.Pow (1 + Par [0], 2) * OP * Par [1] * Par [1]);
		orbitDt = OP / (2 * (N - 1));

		ThetaRunge ();
		time = Random.Range (0, OP);

		CosSinOmega [0] = Mathf.Cos (Par [6]);
		CosSinOmega [1] = Mathf.Sin (Par [6]);
	}

	void FixedUpdate ()
	{
		if (Scales.Pause == false) {
			time += Time.fixedDeltaTime;
			transform.localPosition = ParametricOrbit (ThetaInt (time));
		}
	}

	public Vector3 GetPositionAfterTime (float t)
	{
		return ParametricOrbit (ThetaInt (time + t));
	}

	public Vector3 ParametricOrbit (float th)
	{
		float Cost = Mathf.Cos (th);
		float Sint = Mathf.Sin (th);

		float x = (Par [1] * (1 + Par [0])) / (1 + Par [0] * Cost) * Cost;
		float z = (Par [1] * (1 + Par [0])) / (1 + Par [0] * Cost) * Sint;

		float xp = CosSinOmega [0] * x - CosSinOmega [1] * z;
		float yp = CosSinOmega [1] * x + CosSinOmega [0] * z;

		return new Vector3 (xp, 0f, yp);
	}

	private float dthdt (float th)
	{
		return k * Mathf.Pow ((1 + Par [0] * Mathf.Cos (th)), 2);
	}

	private void ThetaRecurrence ()
	{
		angleArray [0] = 0;  //Initial conditionL theta(0) = 0

		for (int i = 0; i < N - 2; i++)
			angleArray [i + 1] = orbitDt * dthdt (angleArray [i]) + angleArray [i];

		angleArray [N - 1] = Mathf.PI;
	}

	private void ThetaRunge ()
	{
		float w = 0, k1, k2, k3, k4;
		for (int i = 0; i < N - 2; i++) {
			k1 = orbitDt * dthdt (w);
			k2 = orbitDt * dthdt (w + k1 / 2);
			k3 = orbitDt * dthdt (w + k2 / 2);
			k4 = orbitDt * dthdt (w + k3);
			w = w + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
			angleArray [i + 1] = w;
		}
		angleArray [N - 1] = Mathf.PI;
	}

	public float ThetaInt (float t)
	{
		float theta0 = 0;
		t = t % OP;

		if (t <= OP / 2) {
			float i = t / orbitDt;
			float i0 = Mathf.Clamp (Mathf.Floor (i), 0, N - 1);
			float i1 = Mathf.Clamp (Mathf.Ceil (i), 0, N - 1);


			if (i0 == i1)
				theta0 = angleArray [(int)i0];
			else {
				theta0 = (angleArray [(int)i0] - angleArray [(int)i1]) / (i0 - i1) * i + (i0 * angleArray [(int)i1] - angleArray [(int)i0] * i1) / (i0 - i1);
			}
			return theta0;
		} else {
			t = -t + OP;
			float i = t / orbitDt;
			float i0 = Mathf.Clamp (Mathf.Floor (i), 0, N - 1);
			float i1 = Mathf.Clamp (Mathf.Ceil (i), 0, N - 1);

			if (i0 == i1)
				theta0 = -angleArray [(int)i0] + 2 * Mathf.PI;
			else {
				theta0 = -((angleArray [(int)i0] - angleArray [(int)i1]) / (i0 - i1) * i + (i0 * angleArray [(int)i1] - angleArray [(int)i0] * i1) / (i0 - i1)) + 2 * Mathf.PI;
			}
			return theta0;
		}
	}

	public Vector3 ParametricVelocity ()
	{
		float myfixedDt = 2 * orbitDt;
		Vector3 pos2 = ParametricOrbit (ThetaInt (time + myfixedDt));
		Vector3 pos1 = ParametricOrbit (ThetaInt (time - myfixedDt));

		return new Vector3 ((pos2.x - pos1.x) / (2 * myfixedDt), 0.0f, (pos2.z - pos1.z) / (2 * myfixedDt));
	}
}