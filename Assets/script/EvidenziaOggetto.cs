using UnityEngine;

public class EvidenziaNemico : MonoBehaviour
{
    private Renderer[] renderers;
    private Color coloreEvidenziatore = Color.red;
    [SerializeField] private float intensita = 2f;

    void Start()
    {
        // Prende tutti i renderer (utile se il nemico ha più pezzi)
        renderers = GetComponentsInChildren<Renderer>();

        ApplicaEvidenziatore();
    }

    void ApplicaEvidenziatore()
    {
        foreach (Renderer r in renderers)
        {
            // Creiamo un'istanza del materiale per non rovinare il file originale
            Material mat = r.material;

            // 1. Forza lo shader a mostrare l'emissione
            mat.EnableKeyword("_EMISSION");

            // 2. Imposta il colore rosso (HDR)
            // Moltiplichiamo il colore per l'intensità per farlo "bucare" lo schermo
            mat.SetColor("_EmissionColor", coloreEvidenziatore * intensita);

            // 3. Opzionale: Se vuoi che sia proprio TUTTO rosso, cambia anche l'Albedo
            mat.color = coloreEvidenziatore;
        }
    }
}