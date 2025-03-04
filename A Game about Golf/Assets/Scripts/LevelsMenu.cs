using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public int level;
    public TextMeshProUGUI levelText;
    private GameManager gameManagerScript;


    private void Start()
    {
        levelText.text = "LEVEL " + level.ToString();
    }
    public void loadScene()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if ( gameManagerScript == null) Debug.LogError("levelsMenu - GameManager not found.");

        gameManagerScript.loadScene(level);
    }
}
