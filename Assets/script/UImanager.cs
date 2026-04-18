using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;

    // --- APERTURA / CHIUSURA SETTINGS ---
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // pausa il gioco (opzionale)
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // riprende il gioco
    }

    // --- AUDIO ---
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // semplice gestione volume
    }

    // --- FULLSCREEN ---
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // --- ESC PER CHIUDERE ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
            }
        }
    }
}