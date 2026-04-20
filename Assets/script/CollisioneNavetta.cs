using UnityEngine;

public class CollisioneNavetta : MonoBehaviour
{
    [SerializeField] GameObject navetta;
    [SerializeField] AudioClip sfxEsplosione;
    [SerializeField] GameObject prefabEsplosione;
    [SerializeField] float ritardoDistruzione = 1f;
    [SerializeField][Range(0f, 1f)] float volumeEsplosione = 10f; // <-- nuovo campo

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("dentro");
        if (collision.gameObject.CompareTag("asteroide") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(EsplodiNavetta());
        }
        else if (collision.gameObject.CompareTag("Satellite"))
        {
            Destroy(collision.gameObject);
            Destroy(navetta);
            Debug.Log("Hai raggiunto il checkpoint!");
        }
    }

    private System.Collections.IEnumerator EsplodiNavetta()
    {
        if (prefabEsplosione != null)
            Instantiate(prefabEsplosione, navetta.transform.position, navetta.transform.rotation);

        if (sfxEsplosione != null)
            AudioSource.PlayClipAtPoint(sfxEsplosione, navetta.transform.position, volumeEsplosione); // <-- volume applicato

        navetta.SetActive(false);
        yield return new WaitForSeconds(ritardoDistruzione);
        Destroy(navetta);
    }
}