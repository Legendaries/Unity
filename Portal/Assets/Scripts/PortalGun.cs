using UnityEngine;
using System.Collections;

public class PortalGun : MonoBehaviour {

	public GameObject bullet;
	public GameObject portal1Prefab;
	public GameObject portal2Prefab;

	private GameObject portal1;
	private GameObject portal2;

	private const int MAX_DELAY = 5;
	private int delay = MAX_DELAY;

	void Start () {
		portal1 = Instantiate (portal1Prefab);
		//portal1.GetComponent<SpriteRenderer> ().color = Color.blue;
		portal2 = Instantiate (portal2Prefab);
		//portal2.GetComponent<SpriteRenderer> ().color = Color.red;
	}


	void Update () {
		Vector3 test = transform.GetComponent<Rigidbody> ().velocity;
		print (test.y);
		if (Input.GetButtonDown("Fire1")) {
			shootBullet (portal1);
		}else if(Input.GetButtonDown("Fire2")){
			shootBullet (portal2);
		}
		if (delay < MAX_DELAY)
			delay++;
	}

	private void shootBullet(GameObject portal){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hit;
		Vector3 pos = Vector3.zero;

		Vector3 rot = Vector3.zero;

		if (Physics.Raycast (ray, out hit)) {
			pos = hit.point;
			rot = hit.transform.rotation.eulerAngles;
		}
		portal.transform.rotation = Quaternion.Euler(rot);
		rot.y *= Mathf.PI/180f;
		//TODO: replacee later
		if(rot.x == 0)
			pos.y -= 5.12f / 2f;
		else
			pos.y += 0.2f;
		pos.z -= 0.1f*Mathf.Cos(rot.y);
		pos.x -= 0.1f*Mathf.Sin(rot.y);

		portal.transform.position = pos;
	}

	private void portalUpdate(GameObject portal){
		
	}

	void OnTriggerEnter(Collider other) {
		if (delay == MAX_DELAY) {
			if (other.gameObject == portal1) {
				transform.position = portal2.transform.position;
				//transform.rotation = portal2.transform.rotation;
			} else if (other.gameObject == portal2) {
				transform.position = portal1.transform.position;
				//transform.rotation = portal1.transform.rotation;
			}
			delay = 0;
		}
	}
}