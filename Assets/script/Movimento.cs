using UnityEngine;
using UnityEngine.InputSystem;

public class CubeMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float velocita = 5f;
    public float velocitaRotazione = 10f;

    [Header("Boost")]
    public float velocitaBoost = 15f;
    public float moltiplicatoreBoost = 2f; // <-- nuovo campo
    public float durataBoost = 0.2f;
    public float tempoRicarica = 3f;
    [SerializeField] AudioClip dash;
    [SerializeField][Range(0f, 1f)] float volumeEsplosione = 10f;

    [Header("Bordi")]
    public float offsetBordo = 1f;
    public Camera cameraPrincipale;

    private Vector2 moveInput;
    private float timerBoost = 0f;
    private float timerRicarica = 0f;
    private bool boostDisponibile = true;
    private Vector3 direzioneBoost;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && boostDisponibile)
        {
            if (dash != null)
                AudioSource.PlayClipAtPoint(dash, cameraPrincipale.transform.position, volumeEsplosione); // <-- volume applicato
            Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);
            if (dir == Vector3.zero)
                dir = transform.forward;

            direzioneBoost = dir.normalized;
            timerBoost = durataBoost;
            boostDisponibile = false;
            timerRicarica = tempoRicarica;
        }
    }

    void Update()
    {
        if (!boostDisponibile)
        {
            timerRicarica -= Time.deltaTime;
            if (timerRicarica <= 0f)
                boostDisponibile = true;
        }

        Vector3 movimento = new Vector3(moveInput.x, 0f, moveInput.y);

        if (timerBoost > 0f)
        {
            // velocitaBoost moltiplicata per il moltiplicatore impostato nell'editor
            transform.position += direzioneBoost * velocitaBoost * moltiplicatoreBoost * Time.deltaTime;
            timerBoost -= Time.deltaTime;
        }
        else
        {
            transform.position += movimento * velocita * Time.deltaTime;
        }

        transform.position = ClampAiBordi(transform.position);

        if (movimento != Vector3.zero)
        {
            Quaternion rotazioneTarget = Quaternion.LookRotation(movimento);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotazioneTarget,
                velocitaRotazione * Time.deltaTime
            );
        }
    }

    Vector3 ClampAiBordi(Vector3 posizione)
    {
        if (cameraPrincipale == null) return posizione;

        float distanza = Mathf.Abs(cameraPrincipale.transform.position.y - posizione.y);
        Vector3 minBounds = cameraPrincipale.ViewportToWorldPoint(new Vector3(0, 0, distanza));
        Vector3 maxBounds = cameraPrincipale.ViewportToWorldPoint(new Vector3(1, 1, distanza));

        posizione.x = Mathf.Clamp(posizione.x, minBounds.x + offsetBordo, maxBounds.x - offsetBordo);
        posizione.z = Mathf.Clamp(posizione.z, minBounds.z + offsetBordo, maxBounds.z - offsetBordo);

        return posizione;
    }

    public bool BoostDisponibile => boostDisponibile;
    public float ProgressoRicarica => 1f - (timerRicarica / tempoRicarica);
}