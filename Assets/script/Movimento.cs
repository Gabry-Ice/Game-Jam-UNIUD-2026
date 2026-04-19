using UnityEngine;
using UnityEngine.InputSystem;

public class CubeMovement : MonoBehaviour
{
    private Vector2 moveInput;
    public float speed = 10f;
    public float velocitaRotazione = 10f;
    public Camera cameraPrincipale;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        // movimento
        transform.position += movement * speed * Time.deltaTime;

        // limita entro i bordi della telecamera
        transform.position = ClampAllaBordoCamera(transform.position);

        // rotazione verso la direzione di movimento
        if (movement != Vector3.zero)
        {
            Quaternion rotazioneTarget = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotazioneTarget,
                velocitaRotazione * Time.deltaTime
            );
        }
    }

    Vector3 ClampAllaBordoCamera(Vector3 posizione)
    {
        if (cameraPrincipale == null) return posizione;

        // converti in viewport mantenendo la distanza corretta dalla camera
        Vector3 viewPos = cameraPrincipale.WorldToViewportPoint(posizione);

        // clamp X e Y del viewport (0=bordo sinistro/basso, 1=bordo destro/alto)
        viewPos.x = Mathf.Clamp(viewPos.x, 0f, 1f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0f, 1f);

        // riconverti in world space (viewPos.z contiene gia' la distanza dalla camera)
        Vector3 nuovaPosizione = cameraPrincipale.ViewportToWorldPoint(viewPos);
        nuovaPosizione.y = posizione.y;  // mantieni la Y originale dell'oggetto

        return nuovaPosizione;
    }
}