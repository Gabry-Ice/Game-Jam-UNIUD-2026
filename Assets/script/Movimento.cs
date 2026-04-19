using UnityEngine;
using UnityEngine.InputSystem;

public class CubeMovement : MonoBehaviour
{
    private Vector2 moveInput;
    public float speed = 10f;
    public float velocitaRotazione = 10f;
    public Camera cameraPrincipale;
    public float offsetBordo = 0f;  // Offset di 10 unità dai bordi

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

        // Calcola i limiti della telecamera in unità mondiali
        float cameraHeight = 2f * cameraPrincipale.orthographicSize;
        float cameraWidth = cameraHeight * cameraPrincipale.aspect;

        // Limita la posizione del player in base all'offset
        float limitX = Mathf.Clamp(posizione.x, cameraPrincipale.transform.position.x - cameraWidth + offsetBordo, cameraPrincipale.transform.position.x + cameraWidth - offsetBordo);
        float limitZ = Mathf.Clamp(posizione.z, cameraPrincipale.transform.position.z - cameraHeight + offsetBordo, cameraPrincipale.transform.position.z + cameraHeight - offsetBordo);

        return new Vector3(limitX, posizione.y, limitZ);
    }
}
