using UnityEngine;

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

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("dentro");
        if (collision.gameObject.CompareTag("asteroide") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            DistruggiAsteroide(collision.gameObject);
            StartCoroutine(EsplodiNavetta());
        }
        else if (collision.gameObject.CompareTag("Satellite"))
        {
            Destroy(collision.gameObject);
            Destroy(navetta);
            Debug.Log("Hai raggiunto il checkpoint!");
        }
    }

    private void DistruggiAsteroide(GameObject asteroide)
    {
        Vector3 posizione = asteroide.transform.position;

        // Effetto visivo sull'asteroide
        GameObject prefabDaUsare = prefabEsplosioneAsteroide != null
            ? prefabEsplosioneAsteroide
            : prefabEsplosione;

        if (prefabDaUsare != null)
            Instantiate(prefabDaUsare, posizione, Quaternion.identity);

        // Audio sull'asteroide
        AudioClip sfxDaUsare = sfxEsplosioneAsteroide != null
            ? sfxEsplosioneAsteroide
            : sfxEsplosione;

        if (sfxDaUsare != null)
            AudioSource.PlayClipAtPoint(sfxDaUsare, posizione, volumeEsplosione);

        Destroy(asteroide);
    }

    private System.Collections.IEnumerator EsplodiNavetta()
    {
        if (prefabEsplosione != null)
            Instantiate(prefabEsplosione, navetta.transform.position, navetta.transform.rotation);

        if (sfxEsplosione != null)
            AudioSource.PlayClipAtPoint(sfxEsplosione, camera.transform.position, volumeEsplosione);

        navetta.SetActive(false);
        yield return new WaitForSeconds(ritardoDistruzione);
        Destroy(navetta);
    }
}