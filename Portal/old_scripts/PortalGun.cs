using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class PortalGun : MonoBehaviour {

	public GameObject laser;
	private LineRenderer[] laserLine = new LineRenderer[4];
	public GameObject portal1Prefab;
	public GameObject portal2Prefab;

	private GameObject portal1;
	private GameObject portal2;

	private const int MAX_DELAY = 5;
	private int delay = MAX_DELAY;

	private float camFov;

	private FirstPersonController otherScript;

	void Start () {
		
		portal1 = Instantiate (portal1Prefab);
		//portal1.GetComponent<SpriteRenderer> ().color = Color.blue;
		portal2 = Instantiate (portal2Prefab);
		//portal2.GetComponent<SpriteRenderer> ().color = Color.red;
		camFov = Camera.main.fieldOfView;
		otherScript = GetComponent<FirstPersonController>();
		//laserLine[0] = laser.GetComponent<LineRenderer> ();
		for(int i = 0; i < 4; i++)
			laserLine[i] = Instantiate (laser).GetComponent<LineRenderer> ();
	}


	void Update () {
		if (Input.GetKey ("z")) {
			Camera.main.fieldOfView = camFov * 0.2f;
			otherScript.m_MouseLook.smooth = true;
		} else {
			Camera.main.fieldOfView = camFov;
			otherScript.m_MouseLook.smooth = false;
		}
		if (Input.GetButtonDown("Fire1")) {
			shootBullet (portal1);
			for(int i = 0; i < 4; i++)
				laserLine[i].material.SetColor ("_TintColor", new Color(0, 0, 1, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));
		}else if(Input.GetButton("Fire2")){
			shootBullet (portal2);
			for(int i = 0; i < 4; i++)
				laserLine[i].material.SetColor ("_TintColor", new Color(1, 0, 0, 1/2f+1/2f*Mathf.Abs(Mathf.Sin(5f*Time.time))));
		}else if (Input.GetButtonUp("Fire1")) {
			for(int i = 0; i < 4; i++)
				laserLine[i].enabled = false;
		}else if(Input.GetButtonUp("Fire2")){
			for(int i = 0; i < 4; i++)
				laserLine[i].enabled = false;
		}
		if (delay < MAX_DELAY)
			delay++;
		otherScript.velocity = Vector3.zero;
	}

	private void shootBullet(GameObject portal){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			portal.transform.LookAt (portal.transform.position-hit.normal);
			portal.transform.position = hit.point;

			laserLine[0].enabled = true;
			laserLine[0].SetPositions (new Vector3[] {laser.transform.position, hit.point + portal.transform.up * 3f + portal.transform.right * 1.5f});
			laserLine[1].enabled = true;
			laserLine[1].SetPositions (new Vector3[] {laser.transform.position, hit.point + portal.transform.up * 3f - portal.transform.right * 1.5f});
			laserLine[2].enabled = true;
			laserLine[2].SetPositions (new Vector3[] {laser.transform.position, hit.point - portal.transform.up * 3f + portal.transform.right * 1.5f});
			laserLine[3].enabled = true;
			laserLine[3].SetPositions (new Vector3[] {laser.transform.position, hit.point - portal.transform.up * 3f - portal.transform.right * 1.5f});
		}
	}

	private void FixedUpdate() {

	}

	private void portalUpdate(GameObject portal){
		
	}

	void OnTriggerEnter(Collider other) {
		if (delay == MAX_DELAY) {
			if (other.gameObject == portal1) {
				transform.position = portal2.transform.position;
				otherScript.velocity = -portal2.transform.forward*otherScript.m_MoveDir.magnitude;
				//transform.rotation = portal2.transform.rotation;
			} else if (other.gameObject == portal2) {
				transform.position = portal1.transform.position;
				otherScript.velocity = -portal1.transform.forward*otherScript.m_MoveDir.magnitude;
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