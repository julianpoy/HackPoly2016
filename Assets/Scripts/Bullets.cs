using UnityEngine;
using System.Collections;

public class Bullets : MonoBehaviour {

	//Bullet Properties
	private int strength;
	private int speed;

	//Our player sprite
	protected Rigidbody2D bullBody;

	protected SpriteRenderer render; //Our sprite renderer to change our sprite color
	protected Animator animator;   //Used to store a reference to the Player's animator component.

	//our camera Script
	public ActionCamera actionCamera;

	//Our shooting sound
	private AudioSource shoot;

	// Use this for initialization
	void Start () {

		//Set our bullet strength and speed
		strength = 2;
		speed = 100;

		//Play our shooting sound
		shoot = GameObject.Find ("Dodge").GetComponent<AudioSource> ();

		//Get our camera script
		actionCamera = Camera.main.GetComponent<ActionCamera>();

		//Bullet Spawns, add a huge amount of force
		float xForce = speed * 20000;
		//Little bit of randomness
		float yForce = speed * 200;

		//Add the force to our object
		bullBody.AddForce (new Vector2 (yForce, xForce));
	}
	
	// Update is called once per frame
	void Update () {
	}


	//Catch Collisions 
	void OnCollisionEnter2D(Collision2D collision)
	{

		//Set the trigger for the bullet to pop
		actionCamera.impactPause();
		animator.SetTrigger ("Collide");

		//Check if it is an enemy
		if (collision.gameObject.tag == "Enemy") {

			//Shake for damage
			actionCamera.startShake ();

			//Get the enemy object
			Enemy e = (Enemy)collision.gameObject.GetComponent ("Enemy");

			//Find our damage, and set it
			int damage = strength * (speed / 100);
			int newHealth = e.getHealth - damage;

			e.setHealth (newHealth);
		}

		//Lastly remove all force and delete the bullet
		StartCoroutine("BulletHit");
	}

	//Function to stop the bullet and delete it
	public IEnumerator BulletHit() {

		//Set forces to zero
		bullBody.velocity = Vector3.zero;

		//wait a second
		yield return new WaitForSeconds(.25f);

		//delete the object
		Destroy(gameObject);
	}
}
