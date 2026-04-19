using UnityEngine;

public class CollisioneNavetta : MonoBehaviour
{
    [SerializeField] GameObject navetta;

    private void OnTriggerEnter(Collider collision)
    {

        Debug.Log("dentro");
        if (collision.gameObject.CompareTag("asteroide") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(navetta);
        }
    }
}