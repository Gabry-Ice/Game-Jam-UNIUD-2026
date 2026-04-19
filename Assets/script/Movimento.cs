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

        // converti la posizione in viewport (0-1)
        Vector3 viewPos = cameraPrincipale.WorldToViewportPoint(posizione);

        // clamp dentro lo schermo
        viewPos.x = Mathf.Clamp(viewPos.x, 0f, 1f);
        viewPos.z = Mathf.Clamp(viewPos.z, 0f, 1f);

        // riconverti in world space mantenendo la Y originale
        Vector3 nuovaPosizione = cameraPrincipale.ViewportToWorldPoint(viewPos);
        nuovaPosizione.y = posizione.y;

        return nuovaPosizione;
    }
}