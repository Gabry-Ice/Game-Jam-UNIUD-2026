using UnityEngine;
using UnityEngine.SceneManagement; // <--- NECESSARIO per caricare le scene

public class CollisioneNavetta : MonoBehaviour
{
    [SerializeField] GameObject navetta;
    [SerializeField] Camera camera;
    [SerializeField] AudioClip sfxEsplosione;
    [SerializeField] GameObject prefabEsplosione;
    [SerializeField] float ritardoDistruzione = 1f;
    [SerializeField][Range(0f, 1f)] float volumeEsplosione = 1f;

    [Header("Asteroidi")]
    [SerializeField] AudioClip sfxEsplosioneAsteroide;
    [SerializeField] GameObject prefabEsplosioneAsteroide;

    [Header("Scena")]
    [SerializeField] string nomeScenaSconfitta = "LoseScreen"; // Il nome della tua scena di Game Over

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collisione rilevata con: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("asteroide") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            DistruggiAsteroide(collision.gameObject);
            StartCoroutine(EsplodiNavetta());
        }
        else if (collision.gameObject.CompareTag("Satellite"))
        {
            // Nota: Se il satellite deve caricare la WinScreen, 
            // assicurati che sia gestito qui o nello script del Satellite
            Destroy(collision.gameObject);
            Debug.Log("Hai raggiunto il checkpoint!");
        }
    }

    private void DistruggiAsteroide(GameObject asteroide)
    {
        Vector3 posizione = asteroide.transform.position;

        GameObject prefabDaUsare = prefabEsplosioneAsteroide != null
            ? prefabEsplosioneAsteroide
            : prefabEsplosione;

        if (prefabDaUsare != null)
            Instantiate(prefabDaUsare, posizione, Quaternion.identity);

        AudioClip sfxDaUsare = sfxEsplosioneAsteroide != null
            ? sfxEsplosioneAsteroide
            : sfxEsplosione;

        if (sfxDaUsare != null)
            AudioSource.PlayClipAtPoint(sfxDaUsare, posizione, volumeEsplosione);

        Destroy(asteroide);

        Debug.Log("richiamo loseScreen");
        // 5. CARICA LA SCENA DI SCONFITTA
        SceneManager.LoadScene(nomeScenaSconfitta);
    }

    private System.Collections.IEnumerator EsplodiNavetta()
    {
        // 1. Crea l'effetto visivo
        if (prefabEsplosione != null)
            Instantiate(prefabEsplosione, navetta.transform.position, navetta.transform.rotation);

        // 2. Riproduce l'audio
        if (sfxEsplosione != null)
            AudioSource.PlayClipAtPoint(sfxEsplosione, camera.transform.position, volumeEsplosione);

        // 3. Nasconde la navetta invece di distruggerla subito 
        // (altrimenti la coroutine si interrompe se lo script è sulla navetta)
        navetta.SetActive(false);

        // 4. Aspetta che l'esplosione sia visibile/udibile
        yield return new WaitForSeconds(ritardoDistruzione);


        

        // 6. Distruggi definitivamente l'oggetto
        Destroy(navetta);

        Debug.Log("richiamo loseScreen");
        // 5. CARICA LA SCENA DI SCONFITTA
        SceneManager.LoadScene(nomeScenaSconfitta);
    }
}