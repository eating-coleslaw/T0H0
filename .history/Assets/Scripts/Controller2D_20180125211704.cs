using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	const float skinWidth = 0.15f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float horizontalRaySpacing;
	float verticalRaySpacing; 

	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;

	// Use this for initialization
	void Start () {
		collider = GetComponent<BoxCollider2D> ();
	}

	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth*-2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	
	}

	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRaySpacing = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRaySpacing = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, toRight;
		public Vector2 bottomLeft, bottomRight;
}

	// Update is called once per frame
	void Update () {
		UpdateRaycastOrigins ();
		CalculateRaySpacing ();

		for (int i = 0; i < verticalRayCount; i++) {
			Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * 02, Color.red);
		}
		
	}
}
