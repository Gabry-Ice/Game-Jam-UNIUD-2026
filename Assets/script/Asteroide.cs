using UnityEngine;

public class Asteroide : MonoBehaviour
{
    [Header("Movimento")]
    public float velocitaMin = 2f;
    public float velocitaMax = 6f;

    [Header("Durata")]
    public float durata = 5f;

    private GameObject target;
    Vector3 direzione;
    float velocita;

    void Start()
    {
        target = GameObject.FindWithTag("Segnale");

        if (target != null)
        {
            Vector3 direzioneVersoTarget = (target.transform.position - transform.position).normalized;
            float variazioneAngolo = Random.Range(-30f, 30f);
            direzione = Quaternion.Euler(0, variazioneAngolo, 0) * direzioneVersoTarget;
        }
        else
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            direzione = new Vector3(x, 0f, z).normalized;
        }

        velocita = Random.Range(velocitaMin, velocitaMax);

        // Manteniamo il timer di sicurezza (opzionale)
        Destroy(gameObject, durata);
    }

    void Update()
    {
        transform.position += direzione * velocita * Time.deltaTime;
    }

    // --- NUOVA LOGICA: AUTO-DISTRUZIONE FUORI CAMPO ---
    private void OnBecameInvisible()
    {
        // Si distrugge non appena esce dal POV della telecamera
        Destroy(gameObject);
    }
}