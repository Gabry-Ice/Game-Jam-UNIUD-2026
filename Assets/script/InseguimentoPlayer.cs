using UnityEngine;

public class astronave : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 5f;
    public float velocitaRotazione = 5f;

    [Header("Inseguimento")]
    public float durataInseguimento = 4f;
    public float velocitaFuga = 8f;

    [Header("Uscita Rapida")]
    public float velocitaCaduta = 30f; // Molto veloce per sparire in 0.5s
    private float timerMorte = 0.5f;   // Il tuo mezzo secondo

    enum Stato { Inseguimento, Fuga, UscitaDiScena }
    Stato stato = Stato.Inseguimento;

    Transform player;
    float timer;
    Vector3 direzioneFuga;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Segnale");
        if (playerObj != null)
            player = playerObj.transform;

        timer = durataInseguimento;
    }

    void Update()
    {
        if (stato == Stato.UscitaDiScena)
        {
            EseguiCadutaRapida();
            return;
        }

        if (player == null) return;
        timer -= Time.deltaTime;

        if (stato == Stato.Inseguimento)
        {
            if (timer <= 0f)
            {
                stato = Stato.Fuga;
                direzioneFuga = (transform.position - player.position).normalized;
            }
            else Insegui();
        }
        else if (stato == Stato.Fuga)
        {
            Fuggi();
        }
    }

    // --- LOGICA DI USCITA VELOCE ---

    private void OnBecameInvisible()
    {
        if (stato != Stato.UscitaDiScena)
        {
            stato = Stato.UscitaDiScena;
        }
    }

    void EseguiCadutaRapida()
    {
        // 1. Muove l'astronave verso il basso molto velocemente
        transform.position += Vector3.down * velocitaCaduta * Time.deltaTime;

        // 2. Riduce il timer di vita residua
        timerMorte -= Time.deltaTime;

        // 3. Allo scadere del mezzo secondo, distrugge l'oggetto
        if (timerMorte <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // --- METODI ORIGINALI ---

    void Insegui()
    {
        Vector3 direzione = (player.position - transform.position).normalized;
        if (direzione != Vector3.zero)
        {
            Quaternion rotazioneTarget = Quaternion.LookRotation(direzione);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotazioneTarget, velocitaRotazione * Time.deltaTime);
        }
        transform.position += direzione * velocita * Time.deltaTime;
    }

    void Fuggi()
    {
        transform.position += direzioneFuga * velocitaFuga * Time.deltaTime;
    }
}