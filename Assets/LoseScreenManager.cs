using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour
{
    private string previousLevel; // scena del livello da ricaricare

    private void Start()
    {
        previousLevel = PlayerPrefs.GetString("CurrentLevel");

        Debug.Log("Win screen aperta da: " + previousLevel);
    }

    public void RetryLevel()
    {
        Debug.Log("Retry premuto");
        SceneManager.LoadScene(previousLevel);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("LevelSelect"); // nome scena menu livelli
    }
}
