using UnityEngine;
using System.Collections;

public class CharacterParent : MonoBehaviour {

	//Our player sprite
	public Rigidbody2D charBody;

	//Our player movepseed
	public float moveSpeed = 0f;


	//Our player stats
	public int maxHealth;
	private int curHealth;
	//Player health regen rate
	public int healthRegenRate;
	//Should we be regenning health
	private bool regenBool = false;

	//Game jucin', slow when attacking
	private int moveDec;

	private SpriteRenderer render; //Our sprite renderer to change our sprite color
	private bool showFlash;
	private Animator animator;   //Used to store a reference to the Player's animator component.

	//Boolean to check if attacking
	bool attacking;
	//Boolean to check if we are jumping
	bool jumping;

	//Counter for holding space to punch
	private int holdAttack;
	//How long do they have to hold before attacking
	public int holdDuration;

	//Our sounds 
	private AudioSource attack;
	private AudioSource shoot;
	private AudioSource jump;
	private AudioSource death;

	//Our game manager
	GameManager gameManager;

	// Use this for initialization
	void Start () {
		
		//Get a component reference to the Character's animator component
		animator = GetComponent<Animator>();
		render = GetComponent<SpriteRenderer>();

		//Set our default values
		maxHealth = 25;
		attacking = false;
		jumping = false;
		moveDec = 1;
		holdAttack = 0;
		regenBool = false;

		//Get our sounds
		attack = GameObject.Find ("Punch").GetComponent<AudioSource> ();
		shoot = GameObject.Find ("Dodge").GetComponent<AudioSource> ();
		jump = GameObject.Find ("LevelUp").GetComponent<AudioSource> ();
		death = GameObject.Find ("Death").GetComponent<AudioSource> ();

		//Default looking down
		//animator.SetInteger("Direction", 0);

		//Get our gammaneger
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {

		//check if dead, allow movement if alive
		if (curHealth <= 0) 
		{
			//make our player object invisible
			//possible display some animation first
			//Renderer r = (Renderer) gameObject.GetComponent("SpriteRenderer");
			//r.enabled = false;
			//No longer turning invisible, just looping death animation
			//play our death animation
			if(!animator.GetBool("Death"))
			{
				animator.SetTrigger("DeathTrigger");
				animator.SetBool("Death", true);
				//play the death sound
				if(!death.isPlaying)
				{
					death.Play();
				}
			}

			//Set our gameover text
			gameManager.setGameStatus(false);

			//set health to 0
			curHealth = 0;
		} 
		else 
		{
			//Move our player
			Move();

			//Attacks with our player (Check for a level up here as well), only attack if not jumping
			if (Input.GetKey (KeyCode.RightShift) && 
				!jumping &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("Right Jump") &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("Left Jump")) {
				//Now since we are allowing holding space to punch we gotta count for it
				if(!attacking && holdAttack % holdDuration == 0)
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
			if(Input.GetKeyDown("space") && !attacking &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("RightAttack") &&
				!animator.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack")) {
				//Start the dodging coroutine
				StopCoroutine("Jump");
				StartCoroutine ("Jump");
			}

			//Increase health Regen if we are not attacking (doing it halo style, so attacking wont stop you), or being hit/levling up (Check if we are flashing!)
			if(!showFlash)
			{
				regenBool = true;
			}
			//And if we do, reset it to zero
			else
			{
				regenBool = false;
			}

			//Now check if we are attacking for health regen
			if(regenBool && !gameManager.getGameStatus())
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

	//Function to move our Character
	void Move ()
	{
		//Get our input
		float h = Input.GetAxis("Horizontal");

		//Also check to make sure we stay that direction when not moving, so check that we are
		if (h != 0) {
			
			//tell the animator to stop idling
			animator.SetBool("Moving", true);

			//animate to the direction we are going to move
			//Find the greatest absolute value to get most promenint direction
			/*
			 * 		
			 * 0		1
			 * 		
			 * */

			if (h > 0) {
				animator.SetInteger ("Direction", 1);
			} else {
				animator.SetInteger ("Direction", 3);
			}

			//Create a vector to where we are moving
			Vector2 movement = new Vector2 (h, 0); 

			//When attacking start a slow movemnt coroutine
			if (attacking) {
				
				//Attacking working great
				StopCoroutine ("slowMoving");
				StartCoroutine ("slowMoving");
			}


			//Get our speed according to our current level
			float levelSpeed = moveSpeed;


			//Get our actual speed
			float superSpeed = moveSpeed / moveDec;

			//Can't go above .5 though
			if (superSpeed > .032f) {
				superSpeed = .032f;
			}

			//Move to that position
			charBody.MovePosition (charBody.position + movement * superSpeed);
		}

		//then we are not moving
		else {
			
			//Set our position to our current position, so we dont drift away
			charBody.MovePosition (charBody.position);

			//tell the animator we are no longer moving
			animator.SetBool("Moving", false);
		}
	}

	//Function to slow movement for a certain amount of time
	IEnumerator slowMoving()
	{
		//Increase Move Decrement
		moveDec = 5;
		yield return new WaitForSeconds(.5f);
		moveDec = 1;
	}

	//Function to catch attack commands
	IEnumerator Attack()
	{
		//Set attacking to true
		attacking = true;

		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("Attack");
		//Play the sounds
		attack.Play ();

		//Check what direction we are moving, and slight move away from that way when attacking
		int dir = animator.GetInteger("Direction");
		float moveAmount = .01f;
		if(dir == 0)
		{
			gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveAmount, gameObject.transform.position.y, 0);
		}
		else
		{
			gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveAmount, gameObject.transform.position.y, 0);
		}
		//Let the frame finish
		yield return null;
		//set attacking to false
		attacking = false;

	}

	//Catch when we collide with enemy
	void OnCollisionStay2D(Collision2D collision) 
	{
		//check if we are attacking
		if (attacking) 
		{
			//Check if it is an enemy
			if(collision.gameObject.tag == "Enemy")
			{
				//Check if the enemy is in the direction we are facing
				int dir = animator.GetInteger("Direction");

				//Get our x and y
				float playerX = gameObject.transform.position.x;
				float playerY = gameObject.transform.position.y;
				float enemyX = collision.gameObject.transform.position.x;
				float enemyY = collision.gameObject.transform.position.y;
				//Our window for our punch range
				float window = .15f;

				//Deal damage if we are facing the right direction, and they are not too above or around us
				if((dir == 1 && enemyX >= playerX && enemyY <= (playerY + window) && enemyY >= (playerY - window)) ||
					(dir == 3 && enemyX <= playerX && enemyY <= (playerY + window) && enemyY >= (playerY - window)) ||
					(dir == 2 && enemyY >= playerY && enemyX <= (playerX + window) && enemyX >= (playerX - window)) ||
					(dir == 0 && enemyY <= playerY & enemyX <= (playerX + window) && enemyX >= (playerX - window)))
				{
					//Get the enemy and decrease it's health
					Enemy e = (Enemy) collision.gameObject.GetComponent("Enemy");
					//Do damage
					e.setEHealth(e.ehealth - playerLevel);

					//Now knockback
					e.knockBack(animator.GetInteger("Direction"), playerLevel);

					//Shake the screen
					actionCamera.startShake();

					//Add slight impact pause
					actionCamera.startImpact();
				}
			}
		}

	}

	//Get/set funtion for health
	public int getHealth()
	{
		return curHealth;
	}

	//Get/set funtion for health
	public void setHealth(int newHealth)
	{
		curHealth = newHealth;
		if (curHealth > 0) 
		{
			//Set the character damage indicator
			editDamage();
		}
	}

	//Function for if dodging
	public bool isJumping()
	{
		return jumping;
	}

	//Function to indicate health
	public void editDamage()
	{
		//Create our red color indicator
		float healthPercent = maxHealth / curHealth;
		Color damage = new Color (1, healthPercent, healthPercent);
		render.material.color = damage;

	}

}
