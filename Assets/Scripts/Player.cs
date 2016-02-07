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
		jump = GameObject.Find ("LevelUp").GetComponent<AudioSource> ();
		hurt = GameObject.Find ("Hurt").GetComponent<AudioSource> ();
		death = GameObject.Find ("Death").GetComponent<AudioSource> ();

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

		//check if dead, allow movement if alive
		if (curHealth <= 0) {
			//make our player object invisible
			//possible display some animation first
			//Renderer r = (Renderer) gameObject.GetComponent("SpriteRenderer");
			//r.enabled = false;
			//No longer turning invisible, just looping death animation
			//play our death animation
			if (!animator.GetBool ("Death")) {
				animator.SetTrigger ("DeathTrigger");
				animator.SetBool ("Death", true);
				//play the death sound
				if (!death.isPlaying) {
					death.Play ();
				}
			}

			//Set our gameover text
			gameManager.setGameStatus (false);

			//set health to 0
			curHealth = 0;
		} else {

			//Call moving
			base.Move(Input.GetAxis("Horizontal"), shooting);

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.Backspace) &&
				!jumping &&
				animator.GetInteger("Direction") != 0) {
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



			//Shoot!
			if (Input.GetKeyUp (KeyCode.RightShift)) {

				if (!shooting && holdAttack % holdDuration == 0) {

					//Set hold punch to zero
					holdAttack = 0;

					//Shoot
					StopCoroutine("Shoot");
					StartCoroutine ("Shoot");

				}

				//increare hold attack
				holdAttack++;
			}

			//Jumping INput, cant jump if attacking
			if(Input.GetKeyDown(KeyCode.Space) && !shooting
				&& !jumping && jumps < 2) {

					//Jump Coroutine
					StopCoroutine ("Jump");
					StartCoroutine ("Jump");
			}

			//Now check if we are attacking for health regen
			if(!gameManager.getGameStatus())
			{
				//increase health by .5%
				int hpUp = (int)((maxHealth + healthRegenRate) * .05);

				//Check if it is less than one
				if(hpUp < 1)
				{
					hpUp = 1;
				}

				//We don't want to exceed our maximum health
				if(hpUp + curHealth > maxHealth)
				{
					//health is equal to full health
					curHealth = maxHealth;
				}
				else
				{
					//INcrease the health!
					curHealth = curHealth + hpUp;
				}

				//Set the character damage indicator
				editDamage();
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
		float spawnOffset = 0.0775f;
			if(dir == 1)
			{
				gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveAmount, gameObject.transform.position.y, 0);
			}
			else
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
		jumps++;

		//Force some camera Lerp
		actionCamera.forceLerp(0, -0.0065f);

		float i = 0f;
		float rate = 0f;

		if(jumps < 3){

			while (i <= 88f) {

				rate = getJumpPhys(i);

				//Add jump force to our character
				charBody.AddForce (new Vector2 (0, rate));

				//Allow Jumping again a bit early
				if(i > 65f) jumping = false;

				i+=3f;

				//Wait some frames
				//Wait a frame
				yield return 0;
			}
		}
	}

	public float getJumpPhys(float x){
		return 1.1f * ((-(float)Math.Pow(.22f * x - 9.3f, 2f)) + 100f);
	}

	//Function for if dodging
	public bool isJumping()
	{
		return jumping;
	}

	//Function to check if we can jump again for collisions
	void OnCollisionEnter2D(Collision2D collision)
	{
		//Set Jumps to zero
		jumps = 0;
		//Check if it is the player
		if (collision.gameObject.tag == "JumpWall") {
			charBody.gravityScale = 4;
			actionCamera.impactPause();
			actionCamera.startShake ();
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionExit2D(Collision2D collision)
	{
		charBody.gravityScale = 7;
	}

	//Function to check if we can jump again for collisions
	void OnCollisionStay2D(Collision2D collision)
	{

		//Check if it is the player
		if (collision.gameObject.tag == "JumpWall") {
			//Set Jumps to zero
			jumps = 0;
		}
	}
}
