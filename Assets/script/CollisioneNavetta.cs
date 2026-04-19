using UnityEngine;

public class CollisioneNavetta : MonoBehaviour
{
    [SerializeField] GameObject navetta;
    [SerializeField] AudioClip sfxEsplosione;
    [SerializeField] GameObject prefabEsplosione;
    [SerializeField] float ritardoDistruzione = 1f;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("dentro");
        if (collision.gameObject.CompareTag("asteroide") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EsplodiNavetta());
        }
    }

    private System.Collections.IEnumerator EsplodiNavetta()
    {
        // Istanzia il prefab esplosione nella posizione della navetta
        if (prefabEsplosione != null)
            Instantiate(prefabEsplosione, navetta.transform.position, navetta.transform.rotation);

        // Riproduci il suono
        if (sfxEsplosione != null)
            AudioSource.PlayClipAtPoint(sfxEsplosione, navetta.transform.position);

        // Nascondi subito la navetta mentre aspetti
        navetta.SetActive(false);

        yield return new WaitForSeconds(ritardoDistruzione);

        Destroy(navetta);
    }
}