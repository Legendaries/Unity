using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private GameObject portal1;
	private GameObject portal2;

	void Start () {
	
	}

	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "MainCamera") {
			Destroy (this.gameObject);

			portal1.transform.position = collision.transform.position;
			portal1.transform.LookAt (portal1.transform.position-collision.gameObject.transform.forward);

			/*
			Vector3 rot = collision.gameObject.transform.rotation.eulerAngles;
			Vector3 pos = collision.gameObject.transform.position;

			portal1.transform.rotation = Quaternion.Euler (rot);
			rot.y *= Mathf.PI / 180f;
			//TODO: replacee later
			if (rot.x == 0)
				pos.y -= 5.12f / 2f;
			else
				pos.y += 0.2f;
			pos.z += 0.1f * Mathf.Cos (rot.y);
			pos.x -= 0.1f * Mathf.Sin (rot.y);

			portal1.transform.position = pos;
			*/
		}
	}

}
