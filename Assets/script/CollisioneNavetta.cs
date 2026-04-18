using UnityEngine;

public class CollisioneNavetta : MonoBehaviour
{
    [SerializeField] GameObject navetta;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("asteroide"))
        {

            //Debug.Log("dentroooo");
            Destroy(navetta);
        }
    }
}
