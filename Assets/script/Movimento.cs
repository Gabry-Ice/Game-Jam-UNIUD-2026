using UnityEngine;
using UnityEngine.InputSystem;

public class CubeMovement : MonoBehaviour
{
    private Vector2 moveInput;
    public float speed = 10f;
    public float velocitaRotazione = 10f;
    public Camera cameraPrincipale;
    public float offsetBordo = 0.05f;  // 0 = bordo esatto, 0.1 = 10% di margine interno

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

        transform.position += movement * speed * Time.deltaTime;
        transform.position = ClampAllaBordoCamera(transform.position);

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

        Vector3 viewPos = cameraPrincipale.WorldToViewportPoint(posizione);

        viewPos.x = Mathf.Clamp(viewPos.x, offsetBordo, 1f - offsetBordo);
        viewPos.y = Mathf.Clamp(viewPos.y, offsetBordo, 1f - offsetBordo);

        Vector3 nuovaPosizione = cameraPrincipale.ViewportToWorldPoint(viewPos);
        nuovaPosizione.y = posizione.y;

        return nuovaPosizione;
    }
}