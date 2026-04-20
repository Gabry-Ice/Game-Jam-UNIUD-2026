using UnityEngine;

public class Satellite : MonoBehaviour
{
    [Header("Discesa")]
    public float velocitaDiscesa = 3f;
    public float altezzaMinima = -10f;

    [Header("Rotazione")]
    public Vector3 velocitaRotazione = new Vector3(0f, 90f, 45f);

    [Header("Sound")]
    private Camera camera; // <-- solo dichiarazione, niente FindWithTag qui
    [SerializeField] AudioClip success;
    [SerializeField][Range(0f, 1f)] float volumeEsplosione = 1f;

    [HideInInspector]
    public SatelliteSpawner spawner;

    void Awake()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>(); // <-- spostato qui
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
                AudioSource.PlayClipAtPoint(success, camera.transform.position, volumeEsplosione);

            Destroy(gameObject);
        }
    }
}