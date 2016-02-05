using UnityEngine;
using System.Collections;

public class CompanionCubeScript : MonoBehaviour {

	//Variables loaded in start
	public GameObject portal1;
	public GameObject portal2;

	//Other variables
	private const int MAX_DELAY = 5; //Max delay to re-enter a portal, currently only triggered on enter, not stay.
	private int delay = MAX_DELAY; //Current stored delay

	//private LineRenderer lineRenderer;

	void Start () {
		//lineRenderer = GetComponent<LineRenderer> ();
	}


	void Update () {
		
		if (delay < MAX_DELAY)
			delay++;
		transform.Rotate (new Vector3(0, 1, 0));

		/*
		lineRenderer.SetPositions (new Vector3[]{transform.position, transform.forward*100f});

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward*100, out hit)) {
			Destroy(hit.transform.gameObject);
		} else {
			
		}
		*/
	}

	public void FixedUpdate(){

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