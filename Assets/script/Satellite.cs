using UnityEngine;

public class Satellite : MonoBehaviour
{
    [Header("Discesa")]
    public float velocitaDiscesa = 3f;
    public float altezzaMinima = -10f;

    [Header("Rotazione")]
    public Vector3 velocitaRotazione = new Vector3(0f, 90f, 45f);

    [HideInInspector]
    public SatelliteSpawner spawner; // assegnato dallo spawner al momento dell'instantiate

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

            Destroy(gameObject);
        }
    }
}