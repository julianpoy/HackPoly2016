using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour {

	public UnityEngine.UI.Text startText;

	// Use this for initialization
	void Start () {
		StartCoroutine ("flashText");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Return)) {
			Application.LoadLevel ("GameScene");
		}

		if (!startText.enabled) {
			StartCoroutine ("flashText");
		}
	}

	public IEnumerator flashText() {
		float rate = 0.5f;
		yield return new WaitForSeconds(rate / 2);
		startText.enabled = true;
		yield return new WaitForSeconds(rate);
		startText.enabled = false;
	}
}
