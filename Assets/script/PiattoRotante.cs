using UnityEngine;

public class RotateOnX : MonoBehaviour
{
    [Header("Impostazioni Rotazione")]
    [SerializeField] private float velocitaRotazione = 30f; // Gradi al secondo

    void Update()
    {
        // Ruota l'oggetto sull'asse X
        transform.Rotate(0f, velocitaRotazione * Time.deltaTime, 0f);
    }
}