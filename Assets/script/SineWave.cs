using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SineWave : MonoBehaviour
{
    [Header("Onda")]
    public float ampiezza = 2f;
    public float frequenza = 2f;
    public float velocita = 10f;
    public float lunghezza = 8f;
    public int punti = 100;

    [Header("Colore")]
    public Color coloreInizio = Color.white;
    public Color coloreFine = Color.white;   // uguale a inizio per colore uniforme

    LineRenderer lr;
    float tempo;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = punti;
        lr.useWorldSpace = false;
        AggiornaCOlore();
    }

    void Update()
    {
        tempo += Time.deltaTime * velocita;

        for (int i = 0; i < punti; i++)
        {
            float t = (float)i / (punti - 1);
            float z = Mathf.Lerp(-lunghezza / 2f, lunghezza / 2f, t);
            float x = ampiezza * Mathf.Sin(2f * Mathf.PI * frequenza * t - tempo);
            lr.SetPosition(i, new Vector3(x, 3f, z));
        }
    }

    void AggiornaCOlore()
    {
        lr.startColor = coloreInizio;
        lr.endColor = coloreFine;
    }

    // aggiorna il colore anche quando modificato dall'Inspector in Play Mode
    void OnValidate()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (lr != null) AggiornaCOlore();
    }
}