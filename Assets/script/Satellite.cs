using UnityEngine;
using UnityEngine.SceneManagement;

public class Satellite : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 3f;
    public float raggioMassimo = 15f;

    [Header("Rotazione")]
    public Vector3 velocitaRotazione = new Vector3(0f, 90f, 45f);

    [HideInInspector]
    public SatelliteSpawner spawner;

    private Vector3 direzione;
    private Vector3 posizioneIniziale;
    private bool puòEsserePreso = true;

    void Start()
    {
        posizioneIniziale = transform.position;

        float angolo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direzione = new Vector3(Mathf.Cos(angolo), 0f, Mathf.Sin(angolo)).normalized;

        Debug.Log("🛸 Satellite attivo e visibile immediatamente");
    }

    void Update()
    {
        transform.position += direzione * velocita * Time.deltaTime;
        transform.Rotate(velocitaRotazione * Time.deltaTime);

        Vector3 delta = transform.position - posizioneIniziale;
        delta.y = 0f;
        if (delta.magnitude >= raggioMassimo)
            direzione = -direzione;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!puòEsserePreso) return;

        if (other.CompareTag("Segnale"))
        {
            puòEsserePreso = false;
            Debug.Log($"🎯 Satellite ha toccato il segnale: {other.gameObject.name}");

            if (spawner != null)
                spawner.OnSatellitePreso(this);

            Destroy(gameObject, 0.1f);
        }
    }
}