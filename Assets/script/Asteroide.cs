using UnityEngine;

public class Asteroide : MonoBehaviour
{
    [Header("Movimento")]
    public float velocitaMin = 2f;
    public float velocitaMax = 6f;

    [Header("Durata Massima")]
    public float durataFailsafe = 10f; // Distrugge comunque dopo 10s se non esce mai

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
            // Uso Quaternion.Euler(0, variazioneAngolo, 0) se il gioco è sul piano XZ (3D)
            // Se il gioco è 2D (piano XY), usa (0, 0, variazioneAngolo)
            direzione = Quaternion.Euler(0, variazioneAngolo, 0) * direzioneVersoTarget;
        }
        else
        {
            direzione = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        }

        velocita = Random.Range(velocitaMin, velocitaMax);

        // Distruzione di sicurezza se l'asteroide si incastra o non esce mai
        Destroy(gameObject, durataFailsafe);
    }

    void Update()
    {
        transform.position += direzione * velocita * Time.deltaTime;
    }

    // Chiamata da Unity quando l'oggetto esce da TUTTE le telecamere (inclusa la Scene View!)
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}