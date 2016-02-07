using UnityEngine;
using System.Collections;

public class Enemy : CharacterParent
{

	//Our sounds
	private AudioSource attack;
	private AudioSource hurt;
	private AudioSource death;

	//Our target to fight
	private Player player;

	//Our boolean if we are dead
	bool dead;

	//Our boolean if we are colliding with the player
	bool playerCollide;

	//Our amount of frames before we deal damage
	int totalFrames;
	int attackFrames;

	//Our Enenemy Damage Multiplier
	private int eDamage;

	//Enemy speed
	private float eMoveSpeed;



	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

		//Get our sounds
		//attack = GameObject.Find ("Punch").GetComponent<AudioSource> ();
		//hurt = GameObject.Find ("Hurt").GetComponent<AudioSource> ();
		//death = GameObject.Find ("Death").GetComponent<AudioSource> ();

		//Set our Health
		maxHealth = maxHealth / 2;
		curHealth = maxHealth;

		//Set the enemy damage
		eDamage = 10;
		eMoveSpeed = .34f;

		//set dead to false;
		dead = false;
		playerCollide = false;

		//Set our attack frames
		totalFrames = 15;
		attackFrames = totalFrames;

		//Go after our player!
		player = GameObject.FindGameObjectWithTag("PlayerChar").GetComponent<Player>();

	}

	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();

		if(!gameManager.getGameStatus() && !dead && !playerCollide)
		{
				//Move our enemy
				enemyMove ();
		}

		//Check if enemy is dead
		if (curHealth <= 0 && !dead)
		{
			//Decrease the number of enemies we have
			gameManager.minusEnemy();

			//Destroy this enemy, possible display some animation first
			//Destroy(gameObject);

				//Move our enemy out of the way and play the death animation
				animator.SetTrigger("DeathTrigger");
				animator.SetBool("Death", true);

				//Set our sorting layer as a corpse so we step on top of it
				render.sortingLayerName = "Permenance";

				//and remove box collider
				GetComponent<BoxCollider2D>().isTrigger = true;

				//Set death boolean to true
				dead = true;

		}

	}

	//Function to move our player
	void enemyMove ()
	{
		//Get our angle stuff
		float h = gameObject.transform.position.x - player.transform.position.x;

		//Need to make a vector here to move towards


		//Get our speed according to our current level
		//Using enemy skill
		float superSpeed = eMoveSpeed / 8;

		//movement vector
		Vector2 move;
			//Get our movement vector
			move = Vector2.MoveTowards(transform.position, player.transform.position, superSpeed * Time.deltaTime);
			
		//Get the position we want to move to, and go to it using move towards
		charBody.MovePosition(move);
	}

		//Catch when we collide with enemy
		void OnCollisionStay2D(Collision2D collision)
		{
				//Check if it is the player
				if (collision.gameObject.tag == "Player") {
				//Set player collide to true
				playerCollide = true;


				//Decrease the number of frames until we attack
				if (attackFrames > 0) {
					--attackFrames;
				} else if (dead) {
					//Do nothing if dead
				}
				//attack the player
				else {

					//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
					animator.SetTrigger ("Attack");

					//Only do damage if they are not dodging
						//Get the player
						Player p = (Player)collision.gameObject.GetComponent ("Player");

						//Now using an int to calulate our damage before we apply to health
						//Using enemy skill
						int damage = (int)(eDamage * 1.5);
						if (damage < 1) {
							damage = 1;
						}
						int newHealth = p.getHealth () - damage;
						p.setHealth (newHealth);

						//Play the sound of hurt, only if the game is still on
						if (!gameManager.getGameStatus ()) {
							hurt.Play ();
						}

						//Shake the screen
						actionCamera.startShake ();

						//impact pause from the player
						actionCamera.startImpact ();

					//Reset attack frames
					attackFrames = totalFrames;

				}
			}

		}


		//Catch when we collide with something
		void OnCollisionExit2D(Collision2D collision)
		{

			//Check if it is the player
			if (collision.gameObject.tag == "Player") {
				//Set player collide to false
				playerCollide = false;
			}
		}


}
