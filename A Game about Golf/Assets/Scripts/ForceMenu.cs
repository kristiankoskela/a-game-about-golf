using UnityEngine;

public class ForceMenu : MonoBehaviour
{
    private GameManager gameManagerScript;
    private SoundMixerManager soundMixerManagerScript;

    private void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (gameManagerScript == null) Debug.LogError("forceMenu - GameManager not found.");

        soundMixerManagerScript = GameObject.FindGameObjectWithTag("SoundMixerManager").GetComponent<SoundMixerManager>();
        if (soundMixerManagerScript == null) Debug.LogError("forceMenu - soundMixerManager not found.");
    }
    public void ForcePause()
    {
        gameManagerScript.PauseGame();
    }
    public void ForceResume()
    {
        gameManagerScript.ResumeGame();
    }
    public void ForceRestart()
    {
        gameManagerScript.RestartLevel();
    }
    public void ForceNextScene()
    {
        gameManagerScript.NextScene();
    }
    public void ForceMasterVolume(float level)
    {
        soundMixerManagerScript.SetMasterVolume(level);
    }
    public void ForceSFXVolume(float level)
    {
        soundMixerManagerScript.SetSFXVolume(level);
    }
    public void ForceMusicVolume(float level)
    {
        soundMixerManagerScript.SetMusicVolume(level);
    }
}
