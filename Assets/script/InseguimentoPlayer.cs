using UnityEngine;

public class astronave : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 5f;
    public float velocitaRotazione = 5f;

    [Header("Inseguimento")]
    public float durataInseguimento = 4f;   // secondi di inseguimento
    public float velocitaFuga = 8f;         // velocita dopo abbandono

    [Header("Eliminazione")]
    public float distanzaEliminazione = 30f; // distanza dal player per eliminarsi

    enum Stato { Inseguimento, Fuga }
    Stato stato = Stato.Inseguimento;

    Transform player;
    float timer;
    Vector3 direzioneFuga;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        timer = durataInseguimento;
    }

    void Update()
    {
        if (player == null) return;

        timer -= Time.deltaTime;

        if (stato == Stato.Inseguimento)
        {
            if (timer <= 0f)
            {
                // abbandona inseguimento, salva direzione di fuga
                stato = Stato.Fuga;
                direzioneFuga = (transform.position - player.position).normalized;
            }
            else
            {
                Insegui();
            }
        }
        else
        {
            Fuggi();

            // si elimina quando e' abbastanza lontano dal player
            if (Vector3.Distance(transform.position, player.position) >= distanzaEliminazione)
            {
                Destroy(gameObject);
            }
        }
    }

    void Insegui()
    {
        // direzione verso il player su tutti gli assi
        Vector3 direzione = (player.position - transform.position).normalized;

        // rotazione su X e Y verso il player
        if (direzione != Vector3.zero)
        {
            Quaternion rotazioneTarget = Quaternion.LookRotation(direzione);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotazioneTarget,
                velocitaRotazione * Time.deltaTime
            );
        }

        transform.position += direzione * velocita * Time.deltaTime;
    }

    void Fuggi()
    {
        // continua nella direzione di fuga senza ruotare
        transform.position += direzioneFuga * velocitaFuga * Time.deltaTime;
    }
}