using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public GameObject portal;

	void Start () {
	
	}

	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "MainCamera") {
			Destroy (this.gameObject);

			Vector3 rot = collision.gameObject.transform.rotation.eulerAngles;
			Vector3 pos = collision.gameObject.transform.position;

			portal.transform.rotation = Quaternion.Euler (rot);
			rot.y *= Mathf.PI / 180f;
			//TODO: replacee later
			if (rot.x == 0)
				pos.y -= 5.12f / 2f;
			else
				pos.y += 0.2f;
			pos.z += 0.1f * Mathf.Cos (rot.y);
			pos.x -= 0.1f * Mathf.Sin (rot.y);

			portal.transform.position = pos;
		}
	}

}
