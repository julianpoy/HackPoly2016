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
	public int jumpForce;

	bool applyFallPhys;
	float fallPhysCounter;
	int physAlt;

	//Counter for holding space to punch
	private int holdAttack;
	//How long do they have to hold before attacking
	public int holdDuration;

	// Use this for initialization
	protected override void Start ()
	{

		//Call our superclass start
		base.Start();

		//Get our sounds
		shoot = GameObject.Find ("Dodge").GetComponent<AudioSource> ();
		jump = GameObject.Find ("LevelUp").GetComponent<AudioSource> ();
		hurt = GameObject.Find ("Hurt").GetComponent<AudioSource> ();
		death = GameObject.Find ("Death").GetComponent<AudioSource> ();

		//Set our actions
		shooting = false;
		jumps = 0;
		jumping = false;
		holdAttack = 0;

		applyFallPhys = false;
		fallPhysCounter = 0;
		physAlt = 0;
	}

	// Update is called once per frame
	protected override void Update ()
	{
		//Call our base update
		base.Update ();

		if(applyFallPhys && !jumping && physAlt % 8 == 0){

			charBody.AddForce (new Vector2 (0, -getJumpPhys(fallPhysCounter)));

			//Force some camera Lerp
			actionCamera.forceLerp(0, 0.0065f);

			fallPhysCounter += 0.3f;
			if(physAlt == 8) physAlt = 0;
			physAlt++;
		}

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
			if (Input.GetKey (KeyCode.RightShift) &&
				!jumping &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("Right Jump") &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("Left Jump")) {
				//Now since we are allowing holding space to punch we gotta count for it
				if(!shooting && holdAttack % holdDuration == 0)
				{
					//Set hold punch to zero
					holdAttack = 0;
					//Attacking working great
					StopCoroutine("Attack");
					StartCoroutine ("Attack");
				}

				//Increase hold punch
				holdAttack++;
			}



			//If they stop holding space, set holdpunch back to zero
			if (Input.GetKeyUp (KeyCode.RightShift)) {
				//Set hold punch to zero
				holdAttack = 0;
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
	//IEnumerator Shoot() {

	//}

	//Function for jumping
	IEnumerator Jump() {

			//Set our booleans
			jumping = true;
			jumps++;

			//Get our jump Rate

		float i = 0f;
		float rate = getJumpPhys(i);

		while (rate > 0) {

			//Add jump force to our character
			charBody.AddForce (new Vector2 (0, rate));

			//Force some camera Lerp
			actionCamera.forceLerp(0, -0.0065f);

			//Sub tract from the jump force
			rate = getJumpPhys(i);

			//Allow Jumping again a bit early
			if(rate < 2) jumping = false;

			i+=.3f;

			//Wait some frames
				//Wait a frame
				yield return 0;
		}

		i = 0;
		rate = getFallPhys(i);
		Debug.Log(rate);
		while(rate >= 0){
			//Add jump force to our character
			charBody.AddForce (new Vector2 (0, -rate));

			//Force some camera Lerp
			actionCamera.forceLerp(0, 0.0065f);

			//Sub tract from the jump force
			rate = getJumpPhys(i);
			Debug.Log(rate);

			i+=.3f;

			//Wait some frames
			//Wait a frame
			yield return 0;

		}
	}

	public float getJumpPhys(float x){
		float startPos = (x - 1.6f);
		float dropPos = (x - 2.3f);
		float dropAmount = (0.6f * (float)Math.Sin(x - 2.3f));
		float startY = 3.2f;
		return 70*(-(float)Math.Pow(startPos, 2f) + startY - (float)Math.Abs(dropAmount));
	}

	public float getFallPhys(float x){
		return 0.01f * (float)Math.Abs(Math.Pow(x, 2.3f));
	}

	//Function for if dodging
	public bool isJumping()
	{
		return jumping;
	}

	//Function to check if we can jump again for collisions
	void OnCollisionEnter2D(Collision2D collision)
	{
		applyFallPhys = false;
		//Check if it is the player
		if (collision.gameObject.tag == "JumpWall") {
			//Set Jumps to zero
			jumps = 0;

			actionCamera.impactPause ();
		}
	}

	//Function to check if we can jump again for collisions
	void OnCollisionExit2D(Collision2D collision)
	{
		applyFallPhys = true;
		fallPhysCounter = 0.3f;
		jumping = false;
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
