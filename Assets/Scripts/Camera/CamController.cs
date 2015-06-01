using UnityEngine;
using System.Collections;


[AddComponentMenu("Camera-Control/Space RTS Camera Style")]
public class CamController : MonoBehaviour
{
	public Transform lockedTransform { get; private set; }

	private Transform targetRotation;
	private GameObject sphereRot;

	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public int yMinLimit = -80;
	public int yMaxLimit = 80;
	public int zoomRate = 40;
	public bool panMode = false;
	public float panSpeed = 0.3f;
	public int panThres = 5;
	public float zoomDampening = 5.0f;
	
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;

	private bool isZooming = false;
	private bool isRotating = false;
	private bool isPaning = false;

	private Vector3 desiredPosition;
	private Vector3 CamPlanePoint;

	private Vector3 vectorPoint;

	private float lastClickTime = 0;
	private float catchTime= 0.25f;
	private bool isLocked = false;

	private Ray ray;

    Vector3 off = Vector3.zero;
	private Vector3 offSet;

	void Awake() { Init(); }
	
	public void Init()
    {
		targetRotation = new GameObject("Cam targetRotation").transform;

		/*sphereRot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphereRot.transform.parent = targetRotation;
		sphereRot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		sphereRot.name = "Point of Rotation";
		sphereRot.collider.enabled = false;*/
		
		xDeg = Vector3.Angle(Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );

		LinePlaneIntersect(transform.forward.normalized, transform.position, Vector3.up, Vector2.zero, ref CamPlanePoint);
		targetRotation.position = CamPlanePoint;
		targetRotation.rotation = transform.rotation;

		lockedTransform = null;
	}

    void Start()
    {
        LockObject(Planet.planetList[Random.Range(0, Planet.planetList.Count - 1)].transform);
        //LockObject(Planet.planetList[4].transform);
    }

	void LateUpdate()
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float wheel = Input.GetAxis ("Mouse ScrollWheel");
		RaycastHit hit;
		
        int layerMask = 1 << 9;
		layerMask = ~layerMask;

        if (isLocked && lockedTransform !=null)
		{
			/*Renderer lineRenderer = lockedTransform.gameObject.GetComponent<LineRenderer>();
			if ( lineRenderer != null )
				lineRenderer.enabled = false;*/

			offSet = lockedTransform.position - off;
			off = lockedTransform.position;

			if (Input.GetMouseButton(1) == false)
			{
				isRotating = false;
				isZooming = false;
				isPaning = false;

				float magnitude = (targetRotation.position - transform.position).magnitude;
				transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offSet;
				targetRotation.position = targetRotation.position + offSet;
			}
		} 
		else //if(!isLocked && lockedTransform != null )
		{
			UnlockObject();
		}

		if (Input.GetMouseButton(1))
		{
			isRotating = true;
			isZooming = false;
			isPaning = false;

			xDeg += Input.GetAxis("Mouse X") * xSpeed;
			yDeg -= Input.GetAxis("Mouse Y") * ySpeed;

			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit, 5);

			//RotateAroundPoint(targetRotation, xDeg, yDeg)

			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yDeg, xDeg, 0), Time.deltaTime * zoomDampening / Time.timeScale);
			float magnitude = (targetRotation.position - transform.position).magnitude;
			transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offSet;
			targetRotation.rotation = transform.rotation;
			targetRotation.position = targetRotation.position + offSet;
		}
		else if( MouseXBoarder() != 0 || MouseYBoarder() != 0 )
		{
			isPaning = true;
			isZooming = false;
			isRotating = false;
		}
		else if ( wheel != 0 )
		{
			isZooming = true;
			isRotating = false;
			isPaning = false;
			isLocked = false;

			offSet = Vector3.zero;
		}
		else if(DoubleClick(Time.time) && Physics.Raycast (ray, out hit, float.MaxValue, layerMask) == true)
		{
			LockObject( hit.collider.gameObject.transform.parent.transform );
		}

		if (isZooming)
		{
			float s0 = LinePlaneIntersect(transform.forward, transform.position, Vector3.up, Vector2.zero, ref CamPlanePoint);
			targetRotation.position = transform.forward*s0 + transform.position;
			float lineToPlaneLength = LinePlaneIntersect(ray.direction, transform.position, Vector3.up, Vector2.zero, ref vectorPoint);
            
			if (wheel > 0)
			{
				if( lineToPlaneLength > 1.1f)
					desiredPosition = ((vectorPoint - transform.position)/2 + transform.position);
                /*print(transform.position);
                print((vectorPoint - transform.position) / 2);
                print(((vectorPoint - transform.position) / 2 + transform.position));
                print(desiredPosition);*/
			}
			else if (wheel < 0)
				desiredPosition = (-(targetRotation.position - transform.position)/2 + transform.position);
			
			transform.position = Vector3.Lerp (transform.position, desiredPosition,  zoomRate * Time.deltaTime / Time.timeScale);

			if(transform.position == desiredPosition)
				isZooming = false;
		}

		if(panMode == true && isPaning == true)
		{
			float panNorm = transform.position.y;
			if ((Input.mousePosition.x - Screen.width + panThres) > 0)
			{
				targetRotation.Translate (Vector3.right * -panSpeed * Time.deltaTime * panNorm);   //here, right is wrt the loc ref because Space.Self by default
				transform.Translate (Vector3.right * -panSpeed * Time.deltaTime* panNorm);
			}
			else if ((Input.mousePosition.x - panThres) < 0)
			{
				targetRotation.Translate (Vector3.right * panSpeed * Time.deltaTime* panNorm);
				transform.Translate (Vector3.right * panSpeed * Time.deltaTime* panNorm);
			}
			if ((Input.mousePosition.y - Screen.height + panThres) > 0)
			{
				vectorPoint.Set(transform.forward.x, 0, transform.forward.z);
				targetRotation.Translate (vectorPoint.normalized * -panSpeed * Time.deltaTime* panNorm, Space.World);
				transform.Translate (vectorPoint.normalized * -panSpeed * Time.deltaTime* panNorm, Space.World);
			}
			if ((Input.mousePosition.y - panThres) < 0) 
			{
				vectorPoint.Set (transform.forward.x, 0, transform.forward.z);
				targetRotation.Translate (vectorPoint.normalized * panSpeed * Time.deltaTime* panNorm, Space.World);
				transform.Translate (vectorPoint.normalized * panSpeed * Time.deltaTime* panNorm, Space.World);
			}
		}

        //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 1, Mathf.Infinity), transform.position.z);
        transform.position = Vector3.ClampMagnitude(transform.position, Scales.solarSystemEdge);
	}
	
	public void LockObject(Transform transformToLock)
	{
		isLocked = true;
		isZooming = false;
		isPaning = false;
		isRotating = false;

		lockedTransform = transformToLock;
		
		targetRotation.position = lockedTransform.position;
        transform.position = targetRotation.position - new Vector3(1.5f * lockedTransform.localScale.x, -1.5f * lockedTransform.localScale.x, 0);
		transform.LookAt(targetRotation.position, Vector3.up);
		
		xDeg = Vector3.Angle( Vector3.right, transform.right );
		yDeg = Vector3.Angle(Vector3.up, transform.up );
		
		off = lockedTransform.position;
	}

	private void UnlockObject()
	{
		lockedTransform = null;
	}

	private void RotateAroundPoint(ref Transform target,float xDeg, float yDeg)
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yDeg, xDeg, 0), Time.deltaTime * zoomDampening);
		float magnitude = (target.position - transform.position).magnitude;
		transform.position = target.position - (transform.rotation * Vector3.forward * magnitude);
		target.rotation = transform.rotation;
	}

    //LinePlaneIntersect(ray.direction, transform.position, Vector3.up, Vector2.zero, ref vectorPoint);
	private float LinePlaneIntersect(Vector3 u, Vector3 P0, Vector3 N, Vector3 D, ref Vector3 point)
	{
		float s = Vector3.Dot (N, (D - P0)) / Vector3.Dot (N, u);
		point = P0 + s * u;
		return s;
	}

	private int MouseXBoarder()         //Mouse right left or in the screen
	{
		if ((Input.mousePosition.x - Screen.width + panThres) > 0)
			return 1;
		else if ((Input.mousePosition.x - panThres) < 0)
			return -1;
		else
			return 0;
	}

	private int MouseYBoarder()         //Mouse above below or in the screen
	{
		if ((Input.mousePosition.y - Screen.height + panThres) > 0)
			return 1;
		else if ((Input.mousePosition.y - panThres) < 0)
			return -1;
		else
			return 0;
	}
	
	private static float ClampAngle(float angle, float minOuter, float maxOuter, float inner)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;

		angle = Mathf.Clamp(angle, minOuter, maxOuter);

		if(angle < inner && angle > 0)
			angle -= 2*inner;
		else if(angle > -inner && angle < 0)
			angle += 2*inner;

		return angle;
	}

	private bool DoubleClick(float t)
	{
		if(Input.GetMouseButtonDown(0))
		{
			if( (Time.time - lastClickTime) < catchTime * Time.timeScale)
			{
				lastClickTime = Time.time;
				return true;
			}
			else
			{
				lastClickTime = Time.time;
				return false;
			}
		}
		else return false;
	}
}