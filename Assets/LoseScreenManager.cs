using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour
{
    public string levelSceneName; // scena del livello da ricaricare

    public void Retry()
    {
        SceneManager.LoadScene(levelSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("LevelSelect"); // nome scena menu livelli
    }
}
