using UnityEngine;

public class Asteroide : MonoBehaviour
{
    [Header("Movimento")]
    public float velocitaMin = 2f;
    public float velocitaMax = 6f;

    [Header("Durata")]
    public float durata = 5f;          // secondi prima di eliminarsi

    // Riferimento al target (trovato in Start)
    private GameObject target;

    Vector3 direzione;
    float velocita;

    void Start()
    {
        // Cerchiamo il target tramite il Tag
        target = GameObject.FindWithTag("Segnale");

        if (target != null)
        {
            // 1. Calcoliamo la direzione base verso il segnale
            Vector3 direzioneVersoTarget = (target.transform.position - transform.position).normalized;

            // 2. Creiamo una rotazione casuale tra -30 e +30 gradi (totale 60°)
            float variazioneAngolo = Random.Range(-30f, 30f);

            // 3. Applichiamo la rotazione alla direzione calcolata (asse Y per movimento 2D/3D su piano)
            direzione = Quaternion.Euler(0, variazioneAngolo, 0) * direzioneVersoTarget;
        }
        else
        {
            // Meccanica originale: se non trova il segnale, usa una direzione casuale pura
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            direzione = new Vector3(x, 0f, z).normalized;
            Debug.LogWarning("Target 'Segnale' non trovato, direzione casuale attivata.");
        }

        // Meccanica originale: velocità random
        velocita = Random.Range(velocitaMin, velocitaMax);

        // Meccanica originale: autodistruzione
        Destroy(gameObject, durata);
    }

    void Update()
    {
        // Meccanica originale: movimento costante nel tempo
        transform.position += direzione * velocita * Time.deltaTime;
    }
}