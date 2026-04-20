using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    private string previousLevel;

    void Start()
    {
        // 🔥 SICURO: riattiva il gioco (IMPORTANTISSIMO se prima hai usato timeScale = 0)
        Time.timeScale = 1f;

        // recupera livello precedente
        previousLevel = PlayerPrefs.GetString("CurrentLevel");

        Debug.Log("Win screen aperta da: " + previousLevel);
    }

    public void RetryLevel()
    {
        Debug.Log("Retry premuto");
        SceneManager.LoadScene(previousLevel);
    }

    public void NextLevel()
    {
        Debug.Log("Next premuto");
        SceneManager.LoadScene("Livello_2");
    }
}
