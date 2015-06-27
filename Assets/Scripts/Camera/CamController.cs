using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Space RTS Camera Style")]
public class CamController : MonoBehaviour
{
	public Transform lockedTransform { get; private set; }

	public float xSpeed = 200.0f;
	public float ySpeed = 200.0f;
	public float yMinLimit = -80;
	public float yMaxLimit = 80;
	public float zoomRate = 40;
	public bool panMode = false;
	public float panSpeed = 0.3f;
	public int panThres = 5;
	public float rotationDampening = 5.0f;
	private Transform targetRotation;
	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private Vector3 desiredPosition;
	private Vector3 CamPlanePoint;
	private Vector3 vectorPoint;
	private float lastClickTime = 0;
	private float catchTime = 0.25f;
	private bool isLocked = false;
	private Ray ray;
	private Vector3 off = Vector3.zero;
	private Vector3 offSet;
	private Mode mode = Mode.isIdle;
	private enum Mode
	{
		isIdle,
		isRotating,
		isZooming,
		isPanning
    }
	;

	void Awake ()
	{
		Init ();
	}
    
	public void Init ()
	{
		targetRotation = new GameObject ("Cam targetRotation").transform;

		xDeg = Vector3.Angle (Vector3.right, transform.right);
		yDeg = Vector3.Angle (Vector3.up, transform.up);

		LinePlaneIntersect (transform.forward.normalized, transform.position, Vector3.up, Vector2.zero, ref CamPlanePoint);

		targetRotation.position = CamPlanePoint;
		targetRotation.rotation = transform.rotation;

		lockedTransform = null;
	}

	void Start ()
	{
		LockObject (Planet.planetList [Random.Range (0, Planet.planetList.Count - 1)].transform);
	}

	void LateUpdate ()
	{
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		float wheel = Input.GetAxis ("Mouse ScrollWheel");
		RaycastHit hit;
        
		int layerMask = 1 << 9;
		layerMask = ~layerMask;

		if (isLocked) {
			offSet = lockedTransform.position - off;
			off = lockedTransform.position;

			if (Input.GetMouseButton (1) == false) {
				mode = Mode.isIdle;

				float magnitude = (targetRotation.position - transform.position).magnitude;
				transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offSet;
				targetRotation.position = targetRotation.position + offSet;
			}
		}

		if (Input.GetMouseButton (1)) {
			mode = Mode.isRotating;
		} else if (Input.GetMouseButtonUp (1)) {
			yDeg = transform.rotation.eulerAngles.x;
			if (yDeg > 180)
				yDeg -= 360;
            
			xDeg = transform.rotation.eulerAngles.y;
        	
			mode = Mode.isIdle;
		} else if (MouseXBoarder () != 0 || MouseYBoarder () != 0) {
			mode = Mode.isPanning;
		} else if (wheel != 0) {
			mode = Mode.isZooming;
		} else if (DoubleClick (Time.time) && Physics.Raycast (ray, out hit, float.MaxValue, layerMask) == true) {
			if (lockedTransform != hit.collider.gameObject.transform.parent.transform)
				LockObject (hit.collider.gameObject.transform.parent.transform);
		}

		switch (mode) {
		case Mode.isIdle:

			break;
            
		case Mode.isRotating:
            
			xDeg += Input.GetAxis ("Mouse X") * xSpeed;
                
			yDeg -= Input.GetAxis ("Mouse Y") * ySpeed;
			yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit, 5);

			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (yDeg, xDeg, 0), Time.deltaTime * rotationDampening / Time.timeScale);
			targetRotation.rotation = transform.rotation;
                
			float magnitude = (targetRotation.position - transform.position).magnitude;
			transform.position = targetRotation.position - (transform.rotation * Vector3.forward * magnitude) + offSet;
			targetRotation.position = targetRotation.position + offSet;            
			break;
            
		case Mode.isZooming:
            
			if (lockedTransform != null)
				UnlockObject ();

			float s0 = LinePlaneIntersect (transform.forward, transform.position, Vector3.up, Vector2.zero, ref CamPlanePoint);
			targetRotation.position = transform.forward * s0 + transform.position;
			float lineToPlaneLength = LinePlaneIntersect (ray.direction, transform.position, Vector3.up, Vector2.zero, ref vectorPoint);
                
			if (wheel > 0) {
				if (lineToPlaneLength > 1.1f)
					desiredPosition = ((vectorPoint - transform.position) / 2 + transform.position);
                    
			} else if (wheel < 0)
				desiredPosition = (-(targetRotation.position - transform.position) / 2 + transform.position);
                
			transform.position = Vector3.Lerp (transform.position, desiredPosition, zoomRate * Time.deltaTime / Time.timeScale);
                
			if (transform.position == desiredPosition)
				mode = Mode.isIdle;
			break;
            
		case Mode.isPanning:
            
			if (panMode == true) {
				float panNorm = transform.position.y;
				if ((Input.mousePosition.x - Screen.width + panThres) > 0) {
					targetRotation.Translate (Vector3.right * -panSpeed * Time.deltaTime * panNorm);   //here, right is wrt the loc ref because Space.Self by default
					transform.Translate (Vector3.right * -panSpeed * Time.deltaTime * panNorm);
				} else if ((Input.mousePosition.x - panThres) < 0) {
					targetRotation.Translate (Vector3.right * panSpeed * Time.deltaTime * panNorm);
					transform.Translate (Vector3.right * panSpeed * Time.deltaTime * panNorm);
				}
				if ((Input.mousePosition.y - Screen.height + panThres) > 0) {
					vectorPoint.Set (transform.forward.x, 0, transform.forward.z);
					targetRotation.Translate (vectorPoint.normalized * -panSpeed * Time.deltaTime * panNorm, Space.World);
					transform.Translate (vectorPoint.normalized * -panSpeed * Time.deltaTime * panNorm, Space.World);
				}
				if ((Input.mousePosition.y - panThres) < 0) {
					vectorPoint.Set (transform.forward.x, 0, transform.forward.z);
					targetRotation.Translate (vectorPoint.normalized * panSpeed * Time.deltaTime * panNorm, Space.World);
					transform.Translate (vectorPoint.normalized * panSpeed * Time.deltaTime * panNorm, Space.World);
				}
			}
			break;
            
		default:
			break;
		}
        
		transform.position = Vector3.ClampMagnitude (transform.position, Scales.solarSystemEdge);
	}
    
	public void LockObject (Transform transformToLock)
	{
		mode = Mode.isIdle;

		isLocked = true;
		lockedTransform = transformToLock;
		off = lockedTransform.position;

		targetRotation.position = lockedTransform.position;
		transform.position = targetRotation.position - new Vector3 (1.5f * lockedTransform.localScale.x, -1.5f * lockedTransform.localScale.x, 0);
	}

	private void UnlockObject ()
	{
		isLocked = false;
		lockedTransform = null;
		offSet = Vector3.zero;
	}

	private float LinePlaneIntersect (Vector3 u, Vector3 P0, Vector3 N, Vector3 D, ref Vector3 point)
	{
		float s = Vector3.Dot (N, (D - P0)) / Vector3.Dot (N, u);
		point = P0 + s * u;
		return s;
	}

	private int MouseXBoarder ()         //Mouse right left or in the screen
	{
		if ((Input.mousePosition.x - Screen.width + panThres) > 0)
			return 1;
		else if ((Input.mousePosition.x - panThres) < 0)
			return -1;
		else
			return 0;
	}

	private int MouseYBoarder ()         //Mouse above below or in the screen
	{
		if ((Input.mousePosition.y - Screen.height + panThres) > 0)
			return 1;
		else if ((Input.mousePosition.y - panThres) < 0)
			return -1;
		else
			return 0;
	}
    
	private static float ClampAngle (float angle, float minOuter, float maxOuter, float inner)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;

		angle = Mathf.Clamp (angle, minOuter, maxOuter);

		if (angle < inner && angle > 0)
			angle -= 2 * inner;
		else if (angle > -inner && angle < 0)
			angle += 2 * inner;

		return angle;
	}

	private bool DoubleClick (float t)
	{
		if (Input.GetMouseButtonDown (0)) {
			if ((Time.time - lastClickTime) < catchTime * Time.timeScale) {
				lastClickTime = Time.time;
				return true;
			} else {
				lastClickTime = Time.time;
				return false;
			}
		} else
			return false;
	}
}
