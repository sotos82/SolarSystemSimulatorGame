using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
	private Material arrowMat;
	private SpaceCraftOrbit sco;
	private SpaceCraft sc;
	private float thresDist = 80;

	private float width = 6f;
	private float length = 8f;

	void Start ()
	{
		sco = transform.parent.GetComponent<SpaceCraftOrbit> ();
		sc = transform.parent.GetComponent<SpaceCraft> ();

		gameObject.AddComponent<MeshFilter> ();

		arrowMat = new Material (Shader.Find ("Unlit/Transparent"));
		arrowMat.SetTexture ("_MainTex", Resources.Load ("Textures/Arrow") as Texture);

		GetComponent<MeshFilter> ().mesh = BillBoardMesh (width, length);
		GetComponent<MeshFilter> ().mesh.Optimize ();

		GetComponent<Renderer> ().material = arrowMat;

	}
	
	void Update ()
	{
		if (sc.isCourseCorrecting == true) {   
			Vector3 rotVector = Quaternion.Euler (0, sc.angle, 0) * sco.Velocity;

			GetComponent<Renderer> ().enabled = true;

			float objectCamDistance = sc.ObjectCamDistance;
			if (objectCamDistance > thresDist) {
				transform.localPosition = rotVector.normalized * transform.localScale.x * length;
				transform.localScale = new Vector3 (objectCamDistance, objectCamDistance, objectCamDistance) / thresDist;
			} else {
				transform.localPosition = rotVector.normalized * length;
			}
            
			transform.localEulerAngles = new Vector3 (0, AngleBetween (Vector3.forward, rotVector, Vector3.up), 0);
		} else
			GetComponent<Renderer> ().enabled = false;
	}

	private float AngleBetween (Vector3 a, Vector3 b, Vector3 n)
	{
		return Vector3.Angle (a, b) * Mathf.Sign (Vector3.Dot (n, Vector3.Cross (a, b)));
	}

	Mesh BillBoardMesh (float width, float length)
	{
		Mesh mesh = new Mesh ();

		Vector3 pos = new Vector3 (0, 0, 0);

		Vector3[] vertices = new Vector3[4];

		vertices [0] = new Vector3 (pos.x - width / 2, pos.y / 2, pos.z - length);
		vertices [1] = new Vector3 (pos.x + width / 2, pos.y / 2, pos.z - length);
		vertices [2] = new Vector3 (pos.x - width / 2, pos.y, pos.z + length / 2);
		vertices [3] = new Vector3 (pos.x + width / 2, pos.y, pos.z + length / 2);

		int[] tri = new int[6];

		//  Lower left triangle.
		tri [0] = 0;
		tri [1] = 2;
		tri [2] = 1;

		//  Upper right triangle.   
		tri [3] = 2;
		tri [4] = 3;
		tri [5] = 1;

		Vector3[] normals = new Vector3[4];

		normals [0] = -Vector3.up;
		normals [1] = -Vector3.up;
		normals [2] = -Vector3.up;
		normals [3] = -Vector3.up;

		Vector2[] uv = new Vector2[4];

		uv [0] = new Vector2 (0, 0);
		uv [1] = new Vector2 (1, 0);
		uv [2] = new Vector2 (0, 1);
		uv [3] = new Vector2 (1, 1);

		mesh.vertices = vertices;
		mesh.triangles = tri;
		mesh.normals = normals;
		mesh.uv = uv;

		return mesh;
	}
}
