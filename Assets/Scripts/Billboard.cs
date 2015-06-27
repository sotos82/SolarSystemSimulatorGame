using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
	private Mesh bill;
	private Material billMat;
	private float thresDist;
	private float objectCamDistance = 0;
	private float scale;
	float thresCamDist = 0;

	void Start ()
	{
		thresDist = 80;

		gameObject.AddComponent<SphereCollider> ();
		transform.GetComponent<SphereCollider> ().radius = 1;
		gameObject.GetComponent<Collider> ().isTrigger = true;
		gameObject.AddComponent<MeshFilter> ();

		billMat = new Material (Shader.Find ("Unlit/Transparent"));
		billMat.SetTexture ("_MainTex", Resources.Load ("Textures/" + name.Substring (name.LastIndexOf ('B') + 1) + "Bill") as Texture);
        
		GetComponent<MeshFilter> ().mesh = BillBoardMesh (3, 3);
		GetComponent<MeshFilter> ().mesh.Optimize ();
		GetComponent<Renderer> ().material = billMat;

		scale = transform.localScale.x;

		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		thresCamDist = objectCamDistance - thresDist;
		if (objectCamDistance > thresDist) {
			//state = false;
			gameObject.GetComponent<Renderer> ().enabled = true;
			gameObject.GetComponent<Collider> ().enabled = true;
		} else if (objectCamDistance <= thresDist) {	
			gameObject.GetComponent<Renderer> ().enabled = false;
			gameObject.GetComponent<Collider> ().enabled = false;
		}
	}

	void Update ()
	{
		objectCamDistance = (Camera.main.transform.position - transform.position).magnitude;
		float thresCamDistNew = objectCamDistance - thresDist;

		if (thresCamDist * thresCamDistNew < 0) {
			if (objectCamDistance > thresDist) {
				gameObject.GetComponent<Renderer> ().enabled = true;
				gameObject.GetComponent<Collider> ().enabled = true;
			} else if (objectCamDistance <= thresDist) {	
				gameObject.GetComponent<Renderer> ().enabled = false;
				gameObject.GetComponent<Collider> ().enabled = false;
			}
		}
		thresCamDist = thresCamDistNew;

		transform.localScale = new Vector3 (objectCamDistance, objectCamDistance, objectCamDistance) / thresDist * scale;
		transform.LookAt (transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
	}

	Mesh BillBoardMesh (float width, float length)
	{
		Mesh mesh = new Mesh ();

		Vector3 pos = new Vector3 (0, 0, 0);

		Vector3[] vertices = new Vector3[4];
		
		vertices [0] = new Vector3 (pos.x - width / 2, pos.y - length / 2, pos.z + 0);
		vertices [1] = new Vector3 (pos.x + width / 2, pos.y - length / 2, pos.z + 0);
		vertices [2] = new Vector3 (pos.x - width / 2, pos.y + length / 2, pos.z + 0);
		vertices [3] = new Vector3 (pos.x + width / 2, pos.y + length / 2, pos.z + 0);
		
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
