using UnityEngine;

public class Astronave : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 3f;
    public float velocitaRotazione = 5f;   // quanto veloce si gira verso il player

    [Header("Durata")]
    public float durata = 5f;

    Transform player;

    void Start()
    {
        // cerca automaticamente il player nella scena tramite tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        Destroy(gameObject, durata);
    }

    void Update()
    {
        if (player == null) return;

        // direzione verso il player sul piano XZ
        Vector3 direzione = (player.position - transform.position);
        direzione.y = 0f;
        direzione.Normalize();

        // rotazione verso il player
        if (direzione != Vector3.zero)
        {
            Quaternion rotazioneTarget = Quaternion.LookRotation(direzione);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotazioneTarget,
                velocitaRotazione * Time.deltaTime
            );
        }

        // movimento in avanti
        transform.position += direzione * velocita * Time.deltaTime;
    }
}