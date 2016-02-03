using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

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

	private FirstPersonController fpsControllerScript;

	private float camFov;

	//Other variables
	private const int MAX_DELAY = 5; //Max delay to re-enter a portal, currently only triggered on enter, not stay.
	private int delay = MAX_DELAY; //Current stored delay

	private GameObject carrying = null;

	void Start () {
		portal1 = Instantiate (portal1Prefab); //Load the two portals
		portal2 = Instantiate (portal2Prefab);
		laserLine = laser.GetComponent<LineRenderer> (); //Load the line renderer for the portal gun's lasers
		portalGunAnimator = portalGunObj.GetComponent<Animator> (); //Load the animator for the portal gun

		fpsControllerScript = GetComponent<FirstPersonController>(); //To change speed of fps player

		camFov = Camera.main.fieldOfView; //Initial Fov to zoom/reset
	}


	void Update () {
		if (Input.GetButton ("Zoom")) {
			Camera.main.fieldOfView = camFov * 0.2f;
			fpsControllerScript.m_MouseLook.smooth = true;
		} else {
			Camera.main.fieldOfView = camFov;
			fpsControllerScript.m_MouseLook.smooth = false;
		}
		if (Input.GetButton("Fire1")) {
			StopCoroutine("fireLaser");
			StartCoroutine(fireLaser("Fire1", portal1, Color.blue));
		}else if(Input.GetButton("Fire2")){
			StopCoroutine("fireLaser");
			StartCoroutine(fireLaser("Fire2", portal2, Color.red));
		}
		if (delay < MAX_DELAY)
			delay++;
		fpsControllerScript.velocity = Vector3.zero;

		if (Input.GetButtonDown ("Pick Up")) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (carrying == null) {
					if (hit.transform.gameObject.tag == "Carriable") {
						carrying = hit.transform.gameObject;
						carrying.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
						hit.transform.parent = Camera.main.transform;
					}
				} else {
					carrying.transform.parent = null;
					carrying.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
					float mouseX = Input.GetAxis ("Mouse X");
					carrying.GetComponent<Rigidbody> ().AddForce (mouseX*Mathf.Cos(transform.rotation.eulerAngles.y*Mathf.PI/180f)*1000f, Input.GetAxis("Mouse Y")*1000f, -mouseX*Mathf.Sin(transform.rotation.eulerAngles.y*Mathf.PI/180f)*1000f);
					carrying = null;
				}
			}
		}
	}

	public void FixedUpdate(){
		
	}

	IEnumerator fireLaser(string input, GameObject portal, Color color) {
		while(Input.GetButton(input)) {
			laserLine.material.SetColor ("_TintColor", new Color(color.r, color.g, color.b, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
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

	void OnTriggerEnter(Collider other) {
		if (delay == MAX_DELAY) {
			if (other.gameObject == portal1) {
				transform.position = portal2.transform.position;
				fpsControllerScript.velocity = -portal2.transform.forward*fpsControllerScript.m_MoveDir.magnitude;
				//transform.rotation = portal2.transform.rotation;
			} else if (other.gameObject == portal2) {
				transform.position = portal1.transform.position;
				fpsControllerScript.velocity = -portal1.transform.forward*fpsControllerScript.m_MoveDir.magnitude;
				//transform.rotation = portal1.transform.rotation;
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