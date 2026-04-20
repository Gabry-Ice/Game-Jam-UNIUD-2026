using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostUI : MonoBehaviour
{
    [Header("Riferimenti")]
    public CubeMovement cubeMovement;
    public Image immagineRadiale;
    public TMP_Text testoStato;
    public RectTransform contenitore;

    [Header("Colori")]
    public Color coloreProonto = new Color(0f, 0.898f, 1f);
    public Color coloreRicarica = new Color(1f, 0.267f, 0.267f);
    public Color coloreConsumato = new Color(0.1f, 0.1f, 0.1f);

    Image anelloConsumo;

    void Start()
    {
        GameObject anelloObj = new GameObject("AnelloConsumo");
        anelloObj.transform.SetParent(contenitore != null ? contenitore : transform as RectTransform, false);
        anelloObj.transform.SetSiblingIndex(immagineRadiale.transform.GetSiblingIndex() + 1);
        RectTransform anelloRT = anelloObj.AddComponent<RectTransform>();
        anelloRT.anchorMin = Vector2.zero;
        anelloRT.anchorMax = Vector2.one;
        anelloRT.offsetMin = Vector2.zero;
        anelloRT.offsetMax = Vector2.zero;
        anelloConsumo = anelloObj.AddComponent<Image>();
        anelloConsumo.sprite = immagineRadiale.sprite;
        anelloConsumo.color = coloreConsumato;
        anelloConsumo.type = Image.Type.Filled;
        anelloConsumo.fillMethod = Image.FillMethod.Radial360;
        anelloConsumo.fillOrigin = (int)Image.Origin360.Top;
        anelloConsumo.fillClockwise = false;
        anelloConsumo.fillAmount = 0f;
        anelloConsumo.gameObject.SetActive(false);
    }

    void Update()
    {
        if (cubeMovement == null || immagineRadiale == null) return;

        if (cubeMovement.BoostDisponibile)
        {
            immagineRadiale.fillAmount = 1f;
            immagineRadiale.color = coloreProonto;
            anelloConsumo.gameObject.SetActive(false);

            if (testoStato != null)
            {
                testoStato.text = "BOOST";
                testoStato.color = coloreProonto;
            }
        }
        else
        {
            float progresso = cubeMovement.ProgressoRicarica;

            immagineRadiale.fillAmount = 1f;
            immagineRadiale.color = coloreRicarica;

            anelloConsumo.gameObject.SetActive(true);
            anelloConsumo.fillAmount = 1f - progresso;

            float secondiRimanenti = cubeMovement.tempoRicarica * (1f - progresso);
            if (testoStato != null)
            {
                testoStato.text = secondiRimanenti.ToString("F1") + "s";
                testoStato.color = coloreRicarica;
            }
        }
    }
}