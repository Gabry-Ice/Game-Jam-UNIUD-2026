using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mixer;

    void Start()
    {
        Debug.Log("Script attivo");
    }

    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
    }
}