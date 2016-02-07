using UnityEngine;
using System.Collections;

public class Enemy : CharacterParent
{

	//Our sounds
	private AudioSource attack;
	private AudioSource shoot;
	private AudioSource jump;
	private AudioSource death;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start ();

	}

	// Update is called once per frame
	protected override void Update ()
	{
		base.Update ();

	}

	//Function to catch attack commands
	// IEnumerator Attack()
	// {
	// 	//Set attacking to true
	// 	attacking = true;
	//
	// 	//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
	// 	animator.SetTrigger ("Attack");
	// 	//Play the sounds
	// 	attack.Play ();
	//
	// 	//Check what direction we are moving, and slight move away from that way when attacking
	// 	int dir = animator.GetInteger("Direction");
	// 	float moveAmount = .01f;
	// 	if(dir == 0)
	// 	{
	// 		gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveAmount, gameObject.transform.position.y, 0);
	// 	}
	// 	else
	// 	{
	// 		gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveAmount, gameObject.transform.position.y, 0);
	// 	}
	//
	// 	//Let the frame finish
	// 	yield return null;
	// 	//set attacking to false
	// 	attacking = false;
	//
	// }
	//
	// //Catch when we collide with enemy
	// void OnCollisionStay2D(Collision2D collision)
	// {
	// 	//check if we are attacking
	// 	if (attacking)
	// 	{
	// 		//Check if it is an enemy
	// 		if(collision.gameObject.tag == "Enemy")
	// 		{
	// 			//Check if the enemy is in the direction we are facing
	// 			int dir = animator.GetInteger("Direction");
	//
	// 			//Get our x and y
	// 			float playerX = gameObject.transform.position.x;
	// 			float playerY = gameObject.transform.position.y;
	// 			float enemyX = collision.gameObject.transform.position.x;
	// 			float enemyY = collision.gameObject.transform.position.y;
	// 			//Our window for our punch range
	// 			float window = .15f;
	//
	// 			//Deal damage if we are facing the right direction, and they are not too above or around us
	// 			if((dir == 1 && enemyX >= playerX && enemyY <= (playerY + window) && enemyY >= (playerY - window)) ||
	// 				(dir == 3 && enemyX <= playerX && enemyY <= (playerY + window) && enemyY >= (playerY - window)) ||
	// 				(dir == 2 && enemyY >= playerY && enemyX <= (playerX + window) && enemyX >= (playerX - window)) ||
	// 				(dir == 0 && enemyY <= playerY & enemyX <= (playerX + window) && enemyX >= (playerX - window)))
	// 			{
	// 				//Get the enemy and decrease it's health
	// 				Enemy e = (Enemy) collision.gameObject.GetComponent("Enemy");
	// 				//Do damage
	// 				e.setEHealth(e.ehealth - playerLevel);
	//
	// 				//Now knockback
	// 				e.knockBack(animator.GetInteger("Direction"), playerLevel);
	//
	// 				//Shake the screen
	// 				actionCamera.startShake();
	//
	// 				//Add slight impact pause
	// 				actionCamera.startImpact();
	// 			}
	// 		}
	// 	}

	//}
}
