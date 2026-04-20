using UnityEngine;

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
    private bool preso = false;

    void Start()
    {
        posizioneIniziale = transform.position;

        float angolo = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direzione = new Vector3(Mathf.Cos(angolo), 0f, Mathf.Sin(angolo)).normalized;
    }

    void Update()
    {
        if (preso) return;

        transform.position += direzione * velocita * Time.deltaTime;
        transform.Rotate(velocitaRotazione * Time.deltaTime);

        Vector3 delta = transform.position - posizioneIniziale;
        delta.y = 0f;
        if (delta.magnitude >= raggioMassimo)
            direzione = -direzione;
    }

    void OnTriggerEnter(Collider other)
    {
        if (preso) return;
        if (!other.CompareTag("Segnale")) return;

        preso = true;
        Debug.Log($"🎯 Satellite preso da: {other.gameObject.name}");

        if (spawner != null)
            spawner.OnSatellitePreso();

        Destroy(gameObject, 0.1f);
    }
}