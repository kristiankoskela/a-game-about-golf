using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaded : MonoBehaviour
{
    public bool isLoaded;
    private GameManager gameManagerScript;

    private void Awake()
    {
        isLoaded = false;
        //Debug.Log("isLoaded = false");
    }
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoaded = true;
        //Debug.Log("isLoaded = true");

        gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManagerScript == null) Debug.LogError("levelsMenu - GameManager not found.");

        gameManagerScript.OnSceneLoaded();
        
    }
    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
