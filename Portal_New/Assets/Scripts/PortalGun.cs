using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortalGun : MonoBehaviour {

	//Variables loaded in start
	public GameObject portal1Prefab;
	public GameObject portal2Prefab;

	private GameObject portal1;
	private GameObject portal2;

	public GameObject laser;
	private LineRenderer laserLine;

	public GameObject portalGunObj;
	private Animator portalGunAnimator;

	private float camFov;
	private float initMouseSensitivity;

	//Other variables
	private const int MAX_DELAY = 5; //Max delay to re-enter a portal, currently only triggered on enter, not stay.
	private int delay = MAX_DELAY; //Current stored delay

	private GameObject carrying = null;

	private Rigidbody rb;

	private CameraController camController;

	private Vector3 carryingGoalPos = Vector3.zero;

	private MouseUtil mouse;
	public Canvas skillCanvas;
	public Canvas UiCanvas;

	private string skill = "Grapple";

	void Start () {
		portal1 = Instantiate (portal1Prefab); //Load the two portals
		portal2 = Instantiate (portal2Prefab);
		laserLine = laser.GetComponent<LineRenderer> (); //Load the line renderer for the portal gun's lasers
		portalGunAnimator = portalGunObj.GetComponent<Animator> (); //Load the animator for the portal gun

		camFov = Camera.main.fieldOfView; //Initial Fov to zoom/reset
		camController = GetComponent<CameraController> ();
		initMouseSensitivity = camController.mouseSensitivity;
		mouse = GetComponent<MouseUtil>();
		rb = GetComponent<Rigidbody> ();
	}


	void Update () {
		if (Input.GetButton ("Zoom")) {
			Camera.main.fieldOfView = camFov * 0.2f;
			camController.mouseSensitivity = initMouseSensitivity * 0.2f;
		} else {
			Camera.main.fieldOfView = camFov;
			camController.mouseSensitivity = initMouseSensitivity;
		}
		if (Input.GetButtonDown("Fire1")) {
			if (carrying == null) { //Shoot portal
				StopCoroutine ("fireLaser");
				StartCoroutine (fireLaser ("Fire1", portal1, Color.blue));
			} else {// Shoot object
				//carrying.transform.parent = null;
				carrying.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				float mouseX = Input.GetAxis ("Mouse X");
				carrying.GetComponent<Rigidbody> ().AddForce (transform.forward*5000);
				rb.AddForce (-transform.forward*1000);
				carrying.GetComponent<Rigidbody>().useGravity = true;
				carrying.GetComponent<Rigidbody>().freezeRotation = false;
				carrying = null;
			}
		}else if(Input.GetButtonDown("Fire2")){
			StopCoroutine("fireLaser");
			StartCoroutine(fireLaser("Fire2", portal2, Color.red));
		}else if (Input.GetButtonDown ("Pick Up")) {
			if (carrying == null) {
				RaycastHit hit;
				if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
					if (hit.transform.gameObject.tag == "Carriable") {
						carrying = hit.transform.gameObject;
						carrying.GetComponent<Rigidbody>().useGravity = false;
						carrying.GetComponent<Rigidbody> ().freezeRotation = true;
						StopCoroutine("pickUpItem");
						StartCoroutine(pickUpItem(Color.green, (hit.transform.position - transform.position).normalized));
					}
				}
			} else {
				float mouseX = Input.GetAxis ("Mouse X");
				carrying.GetComponent<Rigidbody> ().AddForce (mouseX*Mathf.Cos(transform.rotation.eulerAngles.y*Mathf.PI/180f)*1000f, Input.GetAxis("Mouse Y")*1000f, -mouseX*Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.PI/180f)*1000f);
				carrying.GetComponent<Rigidbody>().useGravity = true;
				carrying.GetComponent<Rigidbody>().freezeRotation = false;
				carrying = null;
			}
			
		}

		if (Input.GetButtonDown ("Skill")) {
			if (skill == "Grapple") {
				RaycastHit hit;
				if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
					StopCoroutine ("shootGrapple");
					StartCoroutine (shootGrapple (Color.magenta, hit.point));
				}
			} else if (skill == "GravityUp") {
				Physics.gravity = new Vector3 (0, 9.81f, 0);
			} else if (skill == "GravityDown") {
				Physics.gravity = new Vector3 (0, -1000f, 0);
			}
		} else if(Input.GetButtonUp ("Skill")) {
			if (skill == "Grapple") { } else if (skill == "GravityUp") {
				Physics.gravity = new Vector3 (0, -9.81f, 0);
			} else if (skill == "GravityDown") {
				Physics.gravity = new Vector3 (0, -9.81f, 0);
			}
		}

		if (Input.GetButtonDown ("SkillSelect")) {
			skillCanvas.GetComponent<Canvas>().enabled = true;
			UiCanvas.transform.Find("Cursor_Crosshairs").GetComponent<Image>().enabled = false;
			skillCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("isOpen", true);
			mouse.lockCusor (false);
		} else if (Input.GetButtonUp ("SkillSelect")) {
			skillCanvas.GetComponent<Canvas>().enabled = false;
			UiCanvas.transform.Find("Cursor_Crosshairs").GetComponent<Image>().enabled = true;
			skillCanvas.transform.GetChild(0).GetComponent<Animator>().SetBool("isOpen", false);
			mouse.lockCusor (true);
		}

		if (delay < MAX_DELAY)
			delay++;
	}

	public void FixedUpdate(){
		
	}

	IEnumerator shootGrapple(Color laserColor, Vector3 hitPos) {
		portalGunAnimator.SetBool ("Firing", true);
		laserLine.enabled = true;
		while(Input.GetButton ("Skill")) {
			rb.AddForce ((hitPos - transform.position).normalized*100);
			laserLine.SetPositions (new Vector3[]{ laser.transform.position,  hitPos });
			laserLine.material.SetColor ("_TintColor", new Color(laserColor.r, laserColor.g, laserColor.b, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));
			yield return null;
		}
		laserLine.enabled = false;
		portalGunAnimator.SetBool ("Firing", false);
	}

	IEnumerator pickUpItem(Color laserColor, Vector3 direction) {
		carryingGoalPos = carrying.transform.position;
		Rigidbody crb = carrying.GetComponent<Rigidbody> ();
		portalGunAnimator.SetBool ("Firing", true);
		while(carrying != null) {

			if ((carryingGoalPos - transform.position).magnitude > 8) {
				carryingGoalPos -= (carryingGoalPos - transform.position).normalized * 3;
			} else {
				carryingGoalPos = transform.position+transform.forward*7.5f;
			}
			crb.velocity = ((carryingGoalPos - carrying.transform.position))*10;

			laserLine.material.SetColor ("_TintColor", new Color(laserColor.r, laserColor.g, laserColor.b, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));

			laserLine.enabled = true;
			laserLine.SetPositions (new Vector3[]{ laser.transform.position,  carrying.transform.position });

			yield return null;
		}
		laserLine.enabled = false;
		portalGunAnimator.SetBool ("Firing", false);
	}

	IEnumerator fireLaser(string input, GameObject portal, Color color) {
		while(Input.GetButton(input)) {
			laserLine.material.SetColor ("_TintColor", new Color(color.r, color.g, color.b, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));

			RaycastHit hit;
			if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
				portalGunAnimator.SetBool ("Firing", true);
				portal.transform.LookAt (portal.transform.position - hit.normal);
				portal.transform.position = hit.point;
				laserLine.enabled = true;
				laserLine.SetPositions (new Vector3[]{ laser.transform.position,  hit.point });
			} else {
				portalGunAnimator.SetBool ("Firing", false);
				laserLine.enabled = false;
			}

			yield return null;
		}
		portalGunAnimator.SetBool ("Firing", false);
		laserLine.enabled = false;
	}

	public void setSKill(string skill){
		this.skill = skill;
	}

	void OnTriggerEnter(Collider other) {
		if (delay == MAX_DELAY) {
			if (other.gameObject == portal1) {
				transform.position = portal2.transform.position-portal2.transform.forward;
				rb.velocity = -portal1.transform.forward * rb.velocity.magnitude;
				//fpsControllerScript.goalVelocity = -portal2.transform.forward*fpsControllerScript.m_MoveDir.magnitude;
			} else if (other.gameObject == portal2) {
				transform.position = portal1.transform.position-portal2.transform.forward;
				rb.velocity = -portal1.transform.forward * rb.velocity.magnitude;
				//fpsControllerScript.goalVelocity = -portal1.transform.forward*fpsControllerScript.m_MoveDir.magnitude;
				//transform.LookAt (portal1.transform.position - portal1.transform.forward);
			}
			delay = 0;
		}
	}
	/*
	void OnTriggerStay(Collider other) {
		if (delay == MAX_DELAY) {
			if (other.gameObject == portal1) {
				transform.position = portal2.transform.position;
				otherScript.velocity = -otherScript.m_MoveDir;
				//transform.rotation = portal2.transform.rotation;
			} else if (other.gameObject == portal2) {
				transform.position = portal1.transform.position;
				otherScript.velocity = -otherScript.m_MoveDir;
				//transform.rotation = portal1.transform.rotation;
			}
			delay = 0;
		}
	}*/
}