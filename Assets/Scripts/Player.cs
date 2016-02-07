﻿using UnityEngine;
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
				animator.SetTrigger ("Death");
				animator.SetBool ("Death", true);
				//play the death sound
				//if (!death.isPlaying) {
					//death.Play ();
				//}
			}

			//Set our gameover text
			gameManager.setGameStatus (false);

			//set health to 0
			curHealth = 0;
		} else {

			//Call moving
			if(!gameManager.getGameStatus()) base.Move(Input.GetAxis("Horizontal"), shooting);

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.Backspace) &&
				!jumping &&
				animator.GetInteger("Direction") != 0 &&
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
		while(rate >= 0){
			//Add jump force to our character
			charBody.AddForce (new Vector2 (0, -rate));

			//Force some camera Lerp
			actionCamera.forceLerp(0, 0.0065f);

			//Sub tract from the jump force
			rate = getJumpPhys(i);

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

			actionCamera.impactPause();
			actionCamera.startShake ();
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
