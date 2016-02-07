using UnityEngine;
using System.Collections;

public class CharacterParent : MonoBehaviour {

	//Our player sprite
	protected Rigidbody2D charBody;

	//Our player movepseed
	public float moveSpeed;

	//Our player stats
	public int maxHealth;
	protected int curHealth;
	//Player health regen rate
	public int healthRegenRate;

	//Game jucin', slow when attacking
	private int moveDec;

	protected SpriteRenderer render; //Our sprite renderer to change our sprite color
	protected Animator animator;   //Used to store a reference to the Player's animator component.

	//Our game manager
	public GameManager gameManager;

	// Use this for initialization
	protected virtual void Start () {
		
		//Get a component reference to the Character's animator component
		animator = GetComponent<Animator>();
		render = GetComponent<SpriteRenderer>();

		//Get the rigid body on the prefab
		charBody = GetComponent<Rigidbody2D>();

		//Set our default values
		//maxHealth = 25; Max Health set by inspector
		moveDec = 1;

		//Set our health
		curHealth = maxHealth;

		//Default looking idle
		animator.SetInteger("Direction", 0);

		//Get our gammaneger
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	}



	//Function to move our Character
	public void Move (float direction, bool inAction)
	{
		//Get our input
		float h = direction;

		Debug.Log (h);

		//Also check to make sure we stay that direction when not moving, so check that we are
		if (h != 0) {

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
				animator.SetInteger ("Direction", -1);
			}

			//Create a vector to where we are moving
			Vector2 movement = new Vector2 (h, 0); 

			//When attacking start a slow movemnt coroutine
			if (!inAction) {

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
			animator.SetInteger ("Direction", 0);
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

	//Function to indicate health
	public void editDamage()
	{
		//Create our red color indicator
		float healthPercent = maxHealth / curHealth;
		Color damage = new Color (1, healthPercent, healthPercent);
		render.material.color = damage;

	}

}
