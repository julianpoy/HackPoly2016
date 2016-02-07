using UnityEngine;
using System.Collections;
using System;

public class Player : CharacterParent
{

	//Our sounds
	private AudioSource shoot;
	private AudioSource jump;
	private AudioSource hurt;
	private AudioSource death;

	//Boolean to check if attacking
	bool shooting;
	//Our Number of jumps we have done
	int jumps;
	bool jumping;

	//Counter for holding space to punch
	private int holdAttack;
	//How long do they have to hold before attacking
	public int holdDuration;

	//Our Buller Game Object
	public GameObject bullet;

	// Use this for initialization
	protected override void Start ()
	{

		//Call our superclass start
		base.Start();

		//Get our sounds
		//jump = GameObject.Find ("LevelUp").GetComponent<AudioSource> ();
		//hurt = GameObject.Find ("Hurt").GetComponent<AudioSource> ();
		//death = GameObject.Find ("Death").GetComponent<AudioSource> ();

		//Set our actions
		shooting = false;
		jumps = 0;
		jumping = false;
		holdAttack = 0;
	}

	// Update is called once per frame
	protected override void Update ()
	{
		//Call our base update
		base.Update ();

		//Add some accelerating gravity
		Vector2 gravityVector = new Vector2(0.0f, -9.81f);
		charBody.AddForce( new Vector2(gravityVector.x * Time.deltaTime, gravityVector.y * Time.deltaTime), ForceMode2D.Force);

		//check if dead, allow movement if alive
		if (curHealth <= 0) {
			//make our player object invisible
			//possible display some animation first
			//Renderer r = (Renderer) gameObject.GetComponent("SpriteRenderer");
			//r.enabled = false;
			//No longer turning invisible, just looping death animation
			//play our death animation
				animator.SetTrigger ("Death");
				//play the death sound
				//if (!death.isPlaying) {
					//death.Play ();
				//}

			//Set our gameover text
			gameManager.setGameStatus (true);

			//set health to 0
			curHealth = 0;
		} else {

			//Call moving
			if(!gameManager.getGameStatus()) base.Move(Input.GetAxis("Horizontal"), shooting);

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.Backspace) &&
				!jumping &&
				!gameManager.getGameStatus()) {
				//Now since we are allowing holding space to punch we gotta count for it
				if(!shooting && holdAttack % holdDuration == 0)
				{
					//Set hold punch to zero
					holdAttack = 0;
					//Attacking working great
					StopCoroutine("Shoot");
					StartCoroutine ("Shoot");
				}

				//Increase hold punch
				holdAttack++;
			}

			//Jumping INput, cant jump if attacking
			if(Input.GetKeyDown(KeyCode.Space) && !shooting
				&& !jumping && jumps < 2 &&
				!gameManager.getGameStatus()) {

					//Jump Coroutine
					StopCoroutine ("Jump");
					StartCoroutine ("Jump");
			}

		}
	}

	//Function for shooting
	IEnumerator Shoot() {

		//Set shooting to true
		shooting = true;

		//Play shooting sound


			//Check what direction we are moving, and slight move that way when attacking
			int dir = animator.GetInteger("Direction");

			float moveAmount = .005f;

		//Our spawn offset
		float spawnOffset = 0.0975f;
			if(dir == 1 ||
			(dir == 0 && lastDir > 0))
			{
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveAmount, gameObject.transform.position.y, 0);
			}
		else if(dir == -1 ||
			(dir == 0 && lastDir < 0))
			{
				gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveAmount, gameObject.transform.position.y, 0);
				spawnOffset = spawnOffset * -1.0f;
			}


			//Instantiate the bullet
			Instantiate(bullet, new Vector3(gameObject.transform.position.x + spawnOffset, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity);

				//Let the frame finish
			yield return null;
			//set attacking to false
			shooting = false;
	}

	//Function for jumping
	IEnumerator Jump() {

		//Set our booleans
		jumping = true;
		animator.SetBool ("Jump", true);
		jumps++;

		//Force some camera Lerp
		actionCamera.forceLerp(0, -0.0065f);

		float i = 0f;
		float rate = 0f;

		if(jumps < 3) {

			while (i <= 88f) {

				rate = getJumpPhys(i);

				//Add jump force to our character
				charBody.AddForce (new Vector2 (0, rate));

				//Allow Jumping again a bit early
				if(i > 65f) {
					animator.SetBool ("Jump", false);
					jumping = false;
				}

				i+=3f;

				//Wait some frames
				//Wait a frame
				yield return 0;
			}
		}
	}

	public float getJumpPhys(float x){
		return 1.2f * ((-(float)Math.Pow(.22f * x - 9.3f, 2f)) + 100f);
	}

	//Function for if dodging
	public bool isJumping()
	{
		return jumping;
	}

	//Function to check if we can jump again for collisions
	void OnCollisionEnter2D(Collision2D collision)
	{
		//Check if it is tthe jumping wall
		if (collision.gameObject.tag == "JumpWall") {
			//Turn Off Jumps
			StopCoroutine ("Jump");
			jumps = 0;
			jumping = false;
			animator.SetBool ("Jump", false);
			actionCamera.impactPause();
			actionCamera.startShake ();
		}

		//Check if it is spikes
		if(collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth(0);
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionStay2D(Collision2D collision)
	{

		//Check if it is the player
		if (collision.gameObject.tag == "JumpWall") {
			//Set Jumps to zero
			jumps = 0;
		}

		//Check if it is spikes
		if(collision.gameObject.tag == "SpikeWall") {
			//Kill the players
			setHealth(0);
		}
	}
}
