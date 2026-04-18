using UnityEngine;

public class Asteroide : MonoBehaviour
{
    [Header("Movimento")]
    public float velocitaMin = 2f;
    public float velocitaMax = 6f;

    [Header("Durata")]
    public float durata = 5f;          // secondi prima di eliminarsi

    Vector3 direzione;
    float velocita;

    void Start()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        direzione = new Vector3(x, 0f, z).normalized;

        velocita = Random.Range(velocitaMin, velocitaMax);

        Destroy(gameObject, durata);   // si elimina dopo 'durata' secondi
    }

    void Update()
    {
        transform.position += direzione * velocita * Time.deltaTime;
    }
}
