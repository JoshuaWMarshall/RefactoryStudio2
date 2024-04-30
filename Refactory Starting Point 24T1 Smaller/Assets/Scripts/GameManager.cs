
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform soccerField; // a reference to our soccer field
    public Vector3 moveArea; // the size of our area where we can move
    public Transform arCamera; // reference to our AR camera

    public GameObject soccerballPrefab; // a reference to the soccer ball in our scene.
    private GameObject currentSoccerBallInstance; // the current soccerball that has been spawned in.
    public Transform aRContentParent; // reference to the overall parent of the ar content.

    public int playerOneScore;
    public int playerTwoScore;


    public UIManager uiManager; // reference to our UI Manager

    public AudioManager audioManager; // reference to our audio manager

    private bool areCharactersRunningAway = false; // are there any characters currently running away from the player.

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("New RandomPosition of:" + ReturnRandomPositionOnField());
        playerOneScore = 0;
        playerTwoScore = 0; // reset our players scores.
        uiManager.DisplayScores(false); // hide our canvases to start with.
        uiManager.UpdateScores(playerOneScore, playerTwoScore); // update our players scores.
        
       
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) 
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadData();
        }

    }

    /// <summary>
    /// Increase the passed in players score by 1.
    /// </summary>
    /// <param name="playerNumber"></param>
    public void IncreasePlayerScore(int playerNumber)
    {
        if (playerNumber == 1)
        {
            playerOneScore++;
        }
        else if(playerNumber == 2)
        {
            playerTwoScore++;
        }
        ResetSoccerBall();
        uiManager.UpdateScores(playerOneScore, playerTwoScore);// updates the ui score to show our current values.
    }

    /// <summary>
    /// Resets the balls positions and velocities.
    /// </summary>
    private void ResetSoccerBall()
    {
        currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // reset the velocity of the ball
        currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // reset the angular velocity
        currentSoccerBallInstance.transform.position = ReturnRandomPositionOnField();// reset the position of the ball
    }

    /// <summary>
    /// Returns a random position within our move area/
    /// </summary>
    /// <returns></returns>
    public Vector3 ReturnRandomPositionOnField()
    {
        float xPosition = Random.Range(-moveArea.x / 2, moveArea.x / 2); // gives us a random number between negative moveArea X and positive moveAreaX.
        float yPosition = soccerField.position.y; // our soccer fields y transform position 
        float zPosition = Random.Range(-moveArea.z / 2, moveArea.z / 2); // gives us a random number between negative moveArea Z and positive moveAreaZ.

        return new Vector3(xPosition, yPosition, zPosition);
    }

    public void SaveData()
    {

        Vector3 vel = currentSoccerBallInstance.GetComponent<Rigidbody>().velocity;
        CharacterController[] players = GameObject.FindObjectsOfType<CharacterController>();
        PlayerPrefs.SetFloat("Player One location x", players[0].transform.position.x); // set player one position on x axis.
        PlayerPrefs.SetFloat("Player One location y", players[0].transform.position.y); // set player one position on y axis.
        PlayerPrefs.SetFloat("Player One location z", players[0].transform.position.z); // set player one position on z axis.
        PlayerPrefs.SetFloat("Player Two location x", players[1].transform.position.x); // set player two position on x axis.
        PlayerPrefs.SetFloat("Player Two location y", players[1].transform.position.y); // set player two position on y axis.
        PlayerPrefs.SetFloat("Player Two location z", players[1].transform.position.z); // set player two position on z axis.
        PlayerPrefs.SetInt("Player One Score", playerOneScore); // set player one score.
        PlayerPrefs.SetInt("Player Two Score", playerTwoScore); // set player two score.        
        int player1state = (int)players[0].currentCharacterState; // set up player one state.
        int player2state = (int)players[1].currentCharacterState; // set up player one state.
        PlayerPrefs.SetFloat("Ball Pos x", currentSoccerBallInstance.transform.position.x); // set up ball position on x axis.
        PlayerPrefs.SetFloat("Ball Pos y", currentSoccerBallInstance.transform.position.y); // set up ball position on y axis.
        PlayerPrefs.SetFloat("Ball Pos z", currentSoccerBallInstance.transform.position.z); // set up ball position on z axis.
        PlayerPrefs.SetFloat("Ball Velocity x", vel.x);
        PlayerPrefs.SetFloat("Ball Velocity y", vel.y);
        PlayerPrefs.SetFloat("Ball Velocity z", vel.z);
        PlayerPrefs.SetInt("playeronestate", player1state);
        PlayerPrefs.SetInt("playertwostate", player2state);
    }

    public void LoadData()
    {
        CharacterController[] players = GameObject.FindObjectsOfType<CharacterController>();
        players[0].transform.position = new Vector3(PlayerPrefs.GetFloat("Player One location x") , PlayerPrefs.GetFloat("Player One location y") , PlayerPrefs.GetFloat("Player One location z"));
        players[1].transform.position = new Vector3(PlayerPrefs.GetFloat("Player Two location x") , PlayerPrefs.GetFloat("Player Two location y") , PlayerPrefs.GetFloat("Player Two location z"));
        playerOneScore = PlayerPrefs.GetInt("Player One Score"); // get player one score.
        playerTwoScore = PlayerPrefs.GetInt("Player Two Score"); // get player two score.
        Vector3 vel = new Vector3();
        players[0].currentCharacterState = (CharacterController.CharacterStates)PlayerPrefs.GetInt("playeronestate");
        players[1].currentCharacterState = (CharacterController.CharacterStates)PlayerPrefs.GetInt("playertwostate");
        vel.x = PlayerPrefs.GetFloat("Ball Velocity x");
        vel.y = PlayerPrefs.GetFloat("Ball Velocity y");
        vel.z = PlayerPrefs.GetFloat("Ball Velocity z");
        currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = vel;
        currentSoccerBallInstance.transform.position = new Vector3(PlayerPrefs.GetFloat("Ball Pos x"), PlayerPrefs.GetFloat("Ball Pos y"), PlayerPrefs.GetFloat("Ball Pos z"));
    }
    /// <summary>
    /// this is a debug function, that lets us draw objects in our scene view, its not viewable in the game view.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // if the user hasn't put a soccer field in, just get out of this function
        if(soccerField == null)
        {
            return;
        }
        Gizmos.color = Color.red; // sets my gizmo to red.
        Gizmos.DrawCube(soccerField.position + new Vector3(0,0.5f,0), moveArea); // draws a cube the at the soccer fields position, and to the size of our move area.
    }

    /// <summary>
    /// Return true or false if we are too close, or not close enough to our AR camera.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    public bool IsPlayerTooCloseToCamera(Transform character, float distanceThreshold)
    {
        if (Vector3.Distance(arCamera.position, character.position) <= distanceThreshold)
        {
            // returns true if we are too close.
            return true;
        }
        else
        {
            // returns false if we are too far away
            return false;
        }
    }

    /// <summary>
    /// Spawns in a new soccer ball based on the position provided. If a soccer ball already exists in the world, we just want to move it to that new position
    /// </summary>
    /// <param name="positionToSpawn"></param>
    public void SpawnOrMoveSoccerBall(Vector3 positionToSpawn)
    {
        if(soccerballPrefab == null)
        {
            Debug.LogError("Something is wrong there is no soccerball assigned in the inspector");
            return;
        }

        // if the soccer ball isn't spawned into the world yet
        if(currentSoccerBallInstance == null)
        {
            // spawn in and store a reference to our soccer ball, and parent it to our ar content parent
            currentSoccerBallInstance = Instantiate(soccerballPrefab, positionToSpawn, soccerballPrefab.transform.rotation, aRContentParent);
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; //sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; //sets the angular velocity of the soccer ball to 0
            AlertCharactersToSoccerBallSpawningIn(); // tell everyone the ball is spawned
        }
        else
        {
            // the soccer ball already exists, so lets just move it.
            currentSoccerBallInstance.transform.position = positionToSpawn; // move our soccerball to the position to spawn
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; //sets the velocity of the soccer ball to 0
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; //sets the angular velocity of the soccer ball to 0
        }
    }


    /// <summary>
    /// Finds all the characters in the scene and loops through them and tells them, that there is a soccerball
    /// </summary>
    private void AlertCharactersToSoccerBallSpawningIn()
    {
        CharacterController[] mice = FindObjectsOfType<CharacterController>(); // find all instances of our character controller class in our scene.
        for(int i=0; i<mice.Length; i++)
        {
            // tell the characters the ball is spawned in.
            mice[i].SoccerBallSpawned(currentSoccerBallInstance.transform);
        }
        uiManager.DisplayScores(true); // display our scores on our goals.
        if(audioManager != null) // if we have a reference to the audio manager
        {
            audioManager.PlayPlayingMusic(); // start playing the second track/the soccer playing music.
        }
    }

    /// <summary>
    /// A function to handle the characters telling us that the player is to close, so play some music.
    /// </summary>
    /// <param name="isRunningAway"></param>
    public void RunningAwayFromCamera(bool isRunningAway)
    {
        if (isRunningAway == areCharactersRunningAway) // don't do anything if the value is the same that is coming in.
        {
            return;
        }
        else
        {
            areCharactersRunningAway = isRunningAway; // set our private bool to this value
        }

        // if characters are running away in fear
        if(areCharactersRunningAway == true)
        {
            audioManager.PlayFleeingMusic(); // start playing the fleeing music.
        }
        else
        {
            audioManager.PlayPreviousTrack(); // otherwise start playing the last track.
        }
    }
}
