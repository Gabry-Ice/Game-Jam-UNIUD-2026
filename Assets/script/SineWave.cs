using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SineWave : MonoBehaviour
{
    [Header("Onda")]
    public float ampiezza = 2f;
    public float frequenza = 2f;
    public float velocita = 10f;
    public float lunghezza = 8f;       // lunghezza impostabile
    public int punti = 100;

    LineRenderer lr;
    float tempo;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = punti;
        lr.useWorldSpace = false;
    }

    void Update()
    {
        tempo += Time.deltaTime * velocita;

        for (int i = 0; i < punti; i++)
        {
            float t = (float)i / (punti - 1);

            // girato 90 gradi: l'onda scorre su Z, oscilla su X
            float z = Mathf.Lerp(-lunghezza / 2f, lunghezza / 2f, t);
            float x = ampiezza * Mathf.Sin(2f * Mathf.PI * frequenza * t - tempo);

            lr.SetPosition(i, new Vector3(x, 0f, z));
        }
    }
}