using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public HoleTrigger holeTriggerScript;
    public BallController ballControllerScript;
    public GameObject pauseMenuUI;
    public GameObject stageCompletedUI;

    private int sceneIndex;
    public bool gamePaused;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {


    }
    private void Update()
    {
        if (holeTriggerScript != null)
        {
            if (holeTriggerScript.stageComplete)
            {
                StageCompleted();
            }
        }
        if (sceneIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }
        if (pauseMenuUI != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                /*
                TogglePause();
                */

                if (!gamePaused)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }

            }
        }
    }
    public void OnSceneLoaded()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("GameManager - OnSceneLoaded called." + "\n" + "sceneIndex: " + sceneIndex + "\n" + "Scene name: " + SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;

        gamePaused = false;

        if (sceneIndex != 0) //dont find ball, hole and pausemenu if in main menu
        {
            holeTriggerScript = GameObject.FindGameObjectWithTag("HoleTrigger").GetComponent<HoleTrigger>();
            //if (holeTriggerScript == null) Debug.LogError("GameManager - holeTrigger not found!");

            ballControllerScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
            //if (ballControllerScript == null) Debug.LogError("GameManager - golfBall not found!");

            pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenuUI");
            pauseMenuUI?.SetActive(false);
            stageCompletedUI = GameObject.FindGameObjectWithTag("StageCompletedUI");
            stageCompletedUI?.SetActive(false);
        }
    }
    private void StageCompleted()
    {
        ballControllerScript.enabled = false;
        //Time.timeScale = 0f;
        stageCompletedUI.SetActive(true);


        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {

        //stageCompletedUI = GameObject.FindGameObjectWithTag("StageCompletedUI");
        if (stageCompletedUI != null)
        {
            stageCompletedUI.SetActive(false);
        }
        if (gamePaused)
        {
            ResumeGame();
        }

        GameObject startPosition = GameObject.FindGameObjectWithTag("StartPosition"); //find startposition
        GameObject golfBall = GameObject.FindGameObjectWithTag("Ball"); //find golfBall
        if (startPosition != null && golfBall != null)
        {
            ballControllerScript = golfBall.GetComponent<BallController>();
            ballControllerScript.enabled = true;

            Rigidbody rb = golfBall.GetComponent<Rigidbody>(); //get golfBall rb
            if (rb != null)
            {
                rb.position = startPosition.transform.position + new Vector3(0, 0.26f, 0);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                holeTriggerScript = GameObject.FindGameObjectWithTag("HoleTrigger").GetComponent<HoleTrigger>();
                //if (holeTriggerScript == null) Debug.LogError("GameManager - holeTrigger not found!");

                holeTriggerScript.stageComplete = false;

                ballControllerScript.shotCount = 0;
                //Debug.Log("Restarted stage.");
            }
        }
    }

    public void PauseGame()
    {
        gamePaused = true;
        //Debug.Log("Game paused");
        ballControllerScript.enabled = false;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        gamePaused = false;
        //Debug.Log("Game resumed", gameObject);
        //pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenuUI");
        pauseMenuUI?.SetActive(false);
        Time.timeScale = 1f;

        ballControllerScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
        //if (ballControllerScript == null) Debug.LogError("GameManager - golfBall not found!");

        ballControllerScript.enabled = true;
    }

    /*
public void TogglePause()
{
    gamePaused = !gamePaused;

    Time.timeScale = gamePaused ? 0f : 1f;

    //pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenuUI");
    if (pauseMenuUI != null)
    {
        pauseMenuUI.SetActive(gamePaused);
    }

    ballControllerScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<ballController>();
    if (ballControllerScript != null)
    {
        ballControllerScript.enabled = !gamePaused;
    }
    else
    {
        Debug.LogError("GameManager - golfBall not found!");
    }
}
    */
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //Debug.Log("GameManager - Loading next scene.");
    }
    public void loadScene(int level)
    {
        SceneManager.LoadScene("Level_" + level.ToString());
        //Debug.Log("GameManager - Loading level_" + level.ToString() + ".");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("GameManager - Quit game.");
    }
}
