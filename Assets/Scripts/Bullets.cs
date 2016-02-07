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
	private ActionCamera actionCamera;

	//Our shooting sound
	private AudioSource shoot;

	//Our target to go away from
	private Player player;
	bool playerRight;

	// Use this for initialization
	void Start () {

		//Get a component reference to the Character's animator component
		animator = GetComponent<Animator>();
		render = GetComponent<SpriteRenderer>();

		//Get the rigid body on the prefab
		bullBody = GetComponent<Rigidbody2D>();

		//Set our bullet strength and speed
		strength = 2;
		speed = 40;

		//Go after our player!
		player = GameObject.Find("Player").GetComponent<Player>();

		//Get our Player current Direction
		if (player.getDirection () > 0 ||
			(player.getDirection() == 0 && player.getLastDirection() > 0 )) {
			animator.SetInteger ("Direction", 1);
			playerRight = true;
		} else {
			playerRight = false;
			animator.SetInteger ("Direction", -1);
		}

		//Play our shooting sound
		//shoot = GameObject.Find ("Dodge").GetComponent<AudioSource> ();

		//Get our camera script
		actionCamera = Camera.main.GetComponent<ActionCamera>();
	}
	
	// Update is called once per frame
	void Update () {

		//Bullet Spawns, add a huge amount of force
		float xForce = 0;
		if (playerRight)
			xForce = speed;
		else
			xForce = speed * -1.0f;

		//Little bit of randomness
		float yForce = speed * Random.insideUnitCircle.y;

		//Add some rotation to bullet
		gameObject.transform.Rotate(new Vector3(0, 0, yForce / 22));

		//Add the force to our object
		bullBody.AddForce (new Vector2 (xForce, yForce / 1.75f));
	}


	//Catch Collisions 
	void OnCollisionEnter2D(Collision2D collision)
	{

		//Set forces to zero
		bullBody.velocity = Vector3.zero;
		bullBody.angularVelocity = 0;

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
			int damage = strength * (speed / 20);
			int newHealth = e.getHealth() - damage;

			e.setHealth (newHealth);
		}

		//Lastly remove all force and delete the bullet
		StartCoroutine("BulletHit");
	}


	//Function to stop the bullet and delete it
	public IEnumerator BulletHit() {

		//wait a second
		yield return new WaitForSeconds(.5f);

		//delete the object
		Destroy(gameObject);
	}
}
