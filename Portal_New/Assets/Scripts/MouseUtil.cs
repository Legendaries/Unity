using UnityEngine;
using System.Collections;

public class MouseUtil : MonoBehaviour {

	void Start () {
	
	}

	public void updateScreenLock(){
		if(Input.GetButton("Fire1")){
			lockCusor (true);
		}else if(Input.GetKeyDown(KeyCode.Escape)){
			lockCusor (false);
		}
	}

	public void lockCusor(bool lockMouse){
		if (lockMouse) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	public bool locked(){
		return Cursor.lockState == CursorLockMode.Locked;
	}

}
