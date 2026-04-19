using UnityEngine;

public class PlayerBounds : MonoBehaviour
{
    void LateUpdate()
    {
        // 1. Trova la distanza tra l'oggetto e la telecamera sull'asse Z
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        // 2. Calcola i punti estremi (0,0 e 1,1) in coordinate del mondo
        Vector3 minBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 maxBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        // 3. Applica il limite alla posizione attuale dell'oggetto
        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, minBounds.x, maxBounds.x);
        currentPos.y = Mathf.Clamp(currentPos.y, minBounds.y, maxBounds.y);

        transform.position = currentPos;
    }
}