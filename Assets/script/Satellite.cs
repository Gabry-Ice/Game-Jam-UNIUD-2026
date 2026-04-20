using UnityEngine;

public class Satellite : MonoBehaviour
{
    [Header("Discesa")]
    public float velocitaDiscesa = 3f;
    public float altezzaMinima = -10f;

    [Header("Rotazione")]
    public Vector3 velocitaRotazione = new Vector3(0f, 90f, 45f);

    [Header("Sound")]
    [SerializeField] AudioClip success;
    [SerializeField][Range(0f, 1f)] float volumeSuccesso = 1f;

    [HideInInspector]
    public SatelliteSpawner spawner;

    private AudioSource audioSource;

    void Awake()
    {
        // Prende o aggiunge AudioSource sulla MainCamera
        audioSource = Camera.main.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        transform.position += Vector3.down * velocitaDiscesa * Time.deltaTime;
        transform.Rotate(velocitaRotazione * Time.deltaTime);

        if (transform.position.y < altezzaMinima)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawner != null)
                spawner.OnCheckpointRaccolto();

            if (success != null)
                audioSource.PlayOneShot(success, volumeSuccesso);

            Destroy(gameObject);
        }
    }
}