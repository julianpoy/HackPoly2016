using UnityEngine;
using System.Collections;

public class Player : CharacterParent
{

	//Our sounds 
	private AudioSource shoot;
	private AudioSource jump;
	private AudioSource hurt;
	private AudioSource death;

	//Boolean to check if attacking
	bool shooting;
	//Boolean to check if we are jumping
	bool jumping;

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

			//Jumping, cant jump if attacking
			if(Input.GetKeyDown("space") && !shooting &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("RightAttack") &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack")) {
				//Start the dodging coroutine
				StopCoroutine("Jump");
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

	//Function for if dodging
	public bool isJumping()
	{
		return jumping;
	}
}

