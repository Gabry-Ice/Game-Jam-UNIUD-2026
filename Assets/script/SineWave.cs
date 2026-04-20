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
    // Usare un Gradient ti permette di fare sfumature complesse dall'Inspector
    public Gradient coloreOnda;

    LineRenderer lr;
    float tempo;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = punti;
        lr.useWorldSpace = false;

        // Imposta il materiale Sprites-Default via codice se ti dimentichi
        if (lr.material == null || lr.material.name.Contains("Default-Material"))
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        AggiornaColore();
    }

    void Update()
    {
        tempo += Time.deltaTime * velocita;

        for (int i = 0; i < punti; i++)
        {
            float t = (float)i / (punti - 1);
            float z = Mathf.Lerp(-lunghezza / 2f, lunghezza / 2f, t);
            float x = ampiezza * Mathf.Sin(2f * Mathf.PI * frequenza * t - tempo);
            lr.SetPosition(i, new Vector3(x, 0f, z));
        }
    }

    void AggiornaColore()
    {
        // Applica l'intero gradiente alla linea
        lr.colorGradient = coloreOnda;
    }

    void OnValidate()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (lr != null) AggiornaColore();
    }
}