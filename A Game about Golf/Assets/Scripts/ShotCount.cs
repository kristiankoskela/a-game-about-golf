using UnityEngine;
using TMPro;

public class ShotCount : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI shotCountText;
    private BallController ballControllerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject golfBall = GameObject.Find("golfBall");
        if (golfBall != null)
        {
            ballControllerScript = golfBall.GetComponent<BallController>();
        }
        else
        {
            Debug.LogError("shotCount - golfBall not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        shotCountText.text = "HITS: " + ballControllerScript.shotCount.ToString();
    }
}
