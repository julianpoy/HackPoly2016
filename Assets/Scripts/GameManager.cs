using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

	//Boolean to determine if the game is over
	private bool gameOver;

	//Rate (seconds) at which monsters should spawn
	public float spawnRate;
	//Our previous time to be stored for the spawn rate
	private float previousTime;

	//Our player object
	private Player user;

	//Our enemy prefab
	public GameObject[] enemies;
	public GameObject[] bosses;
	private int numEnemies;
	//Suggest max enemies 50
	public int maxEnemies;
	//number of enemies spawned
	private int defeatedEnemies;
	//Total number of enemies spawned
	private int totalSpawnedEnemies;

	//Our Maps
	//public Sprite[] maps;
	//private SpriteRenderer gameMap;

	//Our Hud
	private UnityEngine.UI.Text hud;

	//Our score
	private int score;


	//Our background music
	private AudioSource bgFight;

	//Array pf things to say once you die
	String[] epitaph = {"I ain't got time to bleed..."};

	// Use this for initialization
	void Start () {
			//Scale our camera accordingly
			gameOver = false;

			//Set our time to normal speed
			Time.timeScale = 1;

			//Get our player
			user = GameObject.Find ("Person").GetComponent<Player>();

			//Get our Hud
			hud = GameObject.FindGameObjectWithTag ("PlayerHUD").GetComponent<UnityEngine.UI.Text> ();

			//get our bg music
			bgFight = GameObject.Find ("GameSong").GetComponent<AudioSource> ();

			//Defeated enemies is one for score calculation at start
			defeatedEnemies = 1;
			//Total spawned enemies is one because we check for it to spawn enemies, and zero would get it stuck
			totalSpawnedEnemies = 1;

			//Set score to zero
			score = 0;

			//Spawn an enemies
			invokeEnemies ();
		}

		// Update is called once per frame
		void Update () {

			//Check if we need to restart the game
			if(Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.Return)) {
				Application.LoadLevel ("Game");
			}

			//Spawn enemies every frame
			if (!gameOver) {

				//Get the score for the player
				//Going to calculate by enemies defeated, level, and minutes passed
				score = (int) (defeatedEnemies * 100) + defeatedEnemies;


					//Update our hud to player
					hud.text = ("Health: " + user.getHealth () + "\tScore: " + score);

					//start the music! if it is not playing
					if(!bgFight.isPlaying)
					{
						bgFight.Play();
						bgFight.loop = true;
					}
		}
					else
					{
						//First get our epitaph index
						int epitaphIndex = 0;
						if(score / 1000 <= epitaph.Length - 1)
						{
							epitaphIndex = score / 1000;
						}
						else
						{
							epitaphIndex = epitaph.Length - 1;
						}


						//Show our game over
						hud.text = ("GAMEOVER!!!" + "\n" + epitaph[epitaphIndex] + "\nEnemies Defeated:" + defeatedEnemies
												+ "\nHighest Score:" + score);

						//stop the music! if it is playing
						if(bgFight.isPlaying)
						{
							bgFight.Stop();
						}

						//Slow down the game Time
						Time.timeScale = 0.25f;
					}
		}


		//Function to set gameover boolean
		public void setGameStatus(bool status)
		{
				gameOver = status;
		}

		//Function to get gameover boolean
		public bool getGameStatus()
		{
			return gameOver;
		}

		//Function to increase/decrease num enemies
		public void plusEnemy()
		{
				++numEnemies;
				++totalSpawnedEnemies;
		}

		public void minusEnemy()
		{
			--numEnemies;

			//Since enemy is gone add to defeated enemies
			++defeatedEnemies;
		}

		//fucntion to get our total number of spawned enemies
		public int getTotalSpawned()
		{
				// never return zero, only 1 or the actual amount
				if (totalSpawnedEnemies > 0) {
					return totalSpawnedEnemies;
				} else {
					return 1;
				}
		}
		//Functiont o do our invoke repeating functions
		public void invokeEnemies ()
		{
			//Cancel all of our invokes
			CancelInvoke();

			//Now invoke our enemies
			float superRate = spawnRate * defeatedEnemies / 100;
			InvokeRepeating("spawnEnemies", 0 , superRate);
		}


		//Function to spawn enemies repeatedly
		private void spawnEnemies()
		{
			//Only do this if there aren't a max number of enemies
			if (numEnemies < maxEnemies) {


				//We can spawn an enemy anywhere outside of the camera
				//Get ouyr player's position
				user = GameObject.Find ("Person").GetComponent<Player> ();
				Vector2 userPos = user.transform.position;

				//Now find an x and y coordinate that wouldnt be out of bounds the level, attaching this script to it's own object
				//It's position is X: 52, Y: -20 X is left lower, right higher, Y is top higher, bottom lower


				//Find an X to spawn
				float enemyX = 0;

				//get a random direction
				float eDir = -1;
				//Our enemy spawn offset
				float sOffX = 1.4f;
				float boundsX = .4f;
				//Get a random number to slightly influence our off set
				float slight = UnityEngine.Random.Range(0.0f, 0.7f);
				//loop until we get a direction that works
				while(eDir == -1)
				{
					//Get our direction 0,1,2,3
					eDir = Mathf.Floor(UnityEngine.Random.Range(0, 2.0f));

					//Check what direction we got
					if(eDir == 0)
					{
						if(userPos.x > boundsX)
						{
							eDir = -1;
						}
						else
						{
							enemyX = userPos.x + sOffX;
						}
					}
					else if(eDir == 1)
					{
						if(userPos.x < -boundsX)
						{
							eDir = -1;
						}
						else
						{
							enemyX = userPos.x - sOffX;
						}
					}
					else
					{
						//Keep looping
						eDir = -1;
					}
				}


				//Now create a vector with our x and y
				Vector2 spawnPos = new Vector2 (enemyX, 0);

				//Now re-create our spawn rates
				//Get our enemy index
				int enemyIndex = (int)Mathf.Floor (UnityEngine.Random.Range (0, enemies.Length));

				//Try catch for index out of range
				try {
					//create a copy of our gameobject
					Instantiate (enemies [enemyIndex], spawnPos, Quaternion.identity);
				} catch (IndexOutOfRangeException ex) {
					//Print our exception to the console
					print (ex);
				}

			}

		}

}
