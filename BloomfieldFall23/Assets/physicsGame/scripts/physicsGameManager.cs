using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class physicsGameManager : MonoBehaviour
{

    public float timer = 60f;
    public float waveInterval = 20f;
    float waveTimer;
    float waveCount;

    //public TextMeshProUGUI myTimerText;
    //public TextMeshProUGUI playerScore;

    public GameObject myPlayer;
    playerController3D myPlayerController;

    public GameObject myEnemy;

    //we can use Vector2s to represent the X and Y boundaries of our game scene
    public Vector2 myXbounds;
    public Vector2 myYbounds;


    public GameObject[] wave1 = null;
    public GameObject[] wave2 = null;
    public GameObject[] wave3 = null;

    [Header("scenes")]
    public string introScene;
    public string gameScene;
    public string finaleScene;

    public enum GameState { GAMESTART,PLAYING,GAMEOVER };
    public GameState myGameState;


    void Awake()
    {
        //make sure our gameManager is persistent & doesn't die on scene change
        myGameState = GameState.GAMESTART;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        waveCount = 0;
        timer = 60f;
        waveTimer = waveInterval-2f;

    }

    // Update is called once per frame
    void Update()
    {
        switch (myGameState)
        {
                case GameState.GAMESTART: //code for the start menu goes here
                if (Input.GetKeyUp(KeyCode.Return))
                {
                    sceneChanger(gameScene); //sceneChanger takes a string as an argument and loads a new scene
                    StartCoroutine(setPlayer(1f)); //we delay the search for our player to avoid running find() before the player loads
                    EnterPlaying();
                }
                break;

                case GameState.PLAYING: //code for the game playing scene

                //every 30 seconds, spawn a wave of enemies
                if (waveTimer > waveInterval && myPlayer != null)
                {
                    waveCount++; //add to the wave tracker, so we don't double spawn 1 wave

                    if (waveCount == 1)
                    { SpawnWave(wave1); }

                    else if (waveCount == 2)
                    { SpawnWave(wave2); }

                    else if (waveCount == 3)
                    { SpawnWave(wave3); }

                    else { /*filler - add random spawns past wave 3 here*/ }

                    waveTimer = 0f; //reset spawn timer on spawn
                    waveInterval = waveInterval *= 1.2f; //make each wave spawn a little slower
                } //code for spawning pre-designed waves of enemies

                break;

                case GameState.GAMEOVER: //code for our game over screen/scene

                break;
        }





        //waveTimer counts up for enemy spawn while timer counts down for game time
        waveTimer += Time.deltaTime;
       // timer -= Time.deltaTime;

        //Mathf. includes a lot of useful math functions we can use
        //here we're using RoundToInt() to make a cleaner in-game clock display (no decimals)
       // float timeDisplay = Mathf.RoundToInt(timer);
       // Debug.Log("timer: " + timer + "timeDisplay: " + timeDisplay);
       // myTimerText.text = timeDisplay.ToString();


    }

    void ChangeMode(GameState state) //call this to change the GameState enum, takes a state as an argument
    {
        myGameState = state;
    }

    void EnterPlaying()
    {
        ChangeMode(GameState.PLAYING);
    }

    void EnterFinale()
    {

    }

    void EnterStartMenu()
    {

    }

    void SetCube(GameObject newEnemy, Vector3 targetPos)
    {
        //set the script and reference the player variable in our gameManager
        cubeEnemy newScript = newEnemy.GetComponent<cubeEnemy>();
        newScript.SetPlayer(myPlayer);
    }

    void SetRamp(GameObject newEnemy,  Vector3 targetPos)
    {
        //this time we just call the different enemy script
        rampEnemy newScript = newEnemy.GetComponent<rampEnemy>();
        newScript.SetPlayer(myPlayer);

    }

    void SpawnWave(GameObject[] myWave)
    {
        int waveLength = myWave.Length;
        for (int i = 0; i < waveLength; i++) //spawn an enemy of each type for each index in the array
        {
            //this line sets the enemy spawn to a random position inside the game bounds
            Vector3 targetPos = new Vector3(UnityEngine.Random.Range(myXbounds.x, myXbounds.y), 
                                            2f, 
                                            UnityEngine.Random.Range(myYbounds.x, myYbounds.y));
            
            //spawn an enemy of the type specified in array
            GameObject newEnemy = Instantiate(myWave[i], targetPos, Quaternion.identity);

            //check tags, set up enemy if valid, otherwise return debug log INVALID
            if (newEnemy.tag == "ramp") { SetRamp(newEnemy, targetPos); }
            else if (newEnemy.tag == "cube") { SetCube(newEnemy, targetPos); }
            else { Debug.Log("INVALID enemy type"); }
        }
    }

    //method to call whenever the scene needs to be changed
    void sceneChanger(string sceneName)
    {
        //built in Unity function to load a new scene
        SceneManager.LoadScene(sceneName);
    }

    void findPlayer()
    {
        //uses a string to find a specific object, be sure to correctly name your player
        myPlayer = GameObject.Find("physicsPlayer");
    }

    //this is a coroutine - a snippet of code that runs on its own time frame / loop when called
    //in this case we're using it to make sure our game scene loads before searching for the player
    //otherwise we'd risk searching before the player is loaded into the active game scene & failing to find
    IEnumerator setPlayer(float myTime)
    {
        //wait for a given amount of time
        yield return new WaitForSeconds(myTime);
        //call our findPlayer function
        findPlayer();
    }
}