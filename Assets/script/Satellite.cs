using UnityEngine;
using UnityEngine.SceneManagement; // <--- Necessario per cambiare scena

public class Satellite : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 3f;
    public float raggioMassimo = 15f;
    public float ritardoVisibilita = 1;

    [Header("Rotazione")]
    public Vector3 velocitaRotazione = new Vector3(0f, 90f, 45f);

    [Header("Scena")]
    public string nomeScenaWin = "WINscreen"; // Il nome della scena da caricare

    [HideInInspector]
    public SatelliteSpawner spawner;

    private Vector3 direzione;
    private Vector3 posizioneIniziale;
    private bool inMovimento = false;
    private Renderer objectRenderer;
    private Collider objectCollider;

    void Start()
    {
        posizioneIniziale = transform.position;

        float angolo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direzione = new Vector3(Mathf.Cos(angolo), 0f, Mathf.Sin(angolo)).normalized;

        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();

        if (objectRenderer != null) objectRenderer.enabled = false;
        if (objectCollider != null) objectCollider.enabled = false;

        Invoke(nameof(RendiVisibile), ritardoVisibilita);
        Invoke(nameof(AvviaMovimento), ritardoVisibilita);
    }

    void RendiVisibile()
    {
        if (objectRenderer != null) objectRenderer.enabled = true;
        if (objectCollider != null) objectCollider.enabled = true;
    }

    void AvviaMovimento() { inMovimento = true; }

    void Update()
    {
        if (!inMovimento) return;

        transform.position += direzione * velocita * Time.deltaTime;
        transform.Rotate(velocitaRotazione * Time.deltaTime);

        Vector3 delta = transform.position - posizioneIniziale;
        delta.y = 0f;
        if (delta.magnitude >= raggioMassimo)
            direzione = -direzione;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Segnale"))
        {
            if (spawner != null)
                spawner.OnCheckpointRaccolto();

            // Carica la scena della vittoria
            SceneManager.LoadScene(nomeScenaWin);

            Destroy(gameObject);
        }
    }
}