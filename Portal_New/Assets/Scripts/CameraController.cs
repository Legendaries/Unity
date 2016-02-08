using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float mouseSensitivity = 2f;

	public Vector3 rotation = new Vector3(0,0,0);

	public float speed = 500;

	private Rigidbody rb;
	private MouseUtil mouse;

	private float distToGround;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		mouse = GetComponent<MouseUtil>();
		distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
	}

	void Update () {
		mouse.updateScreenLock ();
		move ();
		rotate ();
		transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z+1));
		transform.Rotate (rotation.x, rotation.y, 0);

	}

	private void move(){
		float moveX = Input.GetAxis ("Horizontal");
		float moveY = Input.GetAxis ("Vertical");
		if (Input.GetKeyDown (KeyCode.Space) && IsGrounded()) {
			rb.AddForce (new Vector3 (0, 400, 0));
		}
		Vector3 fwd = transform.forward;
		fwd.Scale(new Vector3(1,0,1));
		Vector3 rgt = transform.right;
		rgt.Scale(new Vector3(1,0,1));

		rb.AddForce(forceToGoalVelocity(fwd.normalized * moveY * speed + rgt.normalized * moveX * speed));
	}

	private Vector3 forceToGoalVelocity(Vector3 goalVelocity){
		Vector3 goalVel = (goalVelocity - rb.velocity);
		goalVel.Scale (new Vector3 (1, 0, 1));
		return goalVel;
	}

	private void rotate(){
		if (mouse.locked()) {
			float dx = Input.GetAxis ("Mouse X");
			rotation.y += dx * mouseSensitivity;
			float dy = Input.GetAxis ("Mouse Y");
			rotation.x += Mathf.Abs (Mathf.Cos (rotation.y)) * -dy * mouseSensitivity;
			rotation.z += Mathf.Sin (rotation.y) * dy * mouseSensitivity;
		}
	}

	void LateUpdate(){
			//transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z+1));
		//transform.rotation = new Quaternion (transform.rotation.x, player.transform.rotation.y, transform.rotation.z, transform.rotation.w);
	}

	void OnCollisionEnter(Collision collision) {
		//print (collision.contacts[0].normal.normalized.y);
		//if(collision.contacts[0].normal.normalized == new Vector3(0, 1, 0)){

		//}
	}

	bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void OnCollisionExit(Collision collision) {
		
	}
}
