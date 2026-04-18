using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefab Asteroidi")]
    public GameObject[] prefabAsteroidi = new GameObject[3];

    [Header("Prefab Astronavi")]
    public GameObject[] prefabAstronavi = new GameObject[3];

    [Header("Spawn")]
    public float intervalloSpawn = 2f;
    public float raggioSpawn = 10f;

    [Header("Sicurezza")]
    public Transform nave;
    public float raggioDiSicurezza = 5f;
    public int maxTentativi = 10;

    [Header("Difficolta")]
    public float difficolta = 1f;
    public float intervalloMinimo = 0.3f;
    public float moltiplicatoreDifficolta = 0.1f;

    float timer;
    float intervalloCorrente;

    void Start()
    {
        AggiornaDifficolta();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= intervalloCorrente)
        {
            timer = 0f;
            SpawnAsteroide();
        }
    }

    public void ImpostaDifficolta(float nuovaDifficolta)
    {
        difficolta = nuovaDifficolta;
        AggiornaDifficolta();
    }

    void AggiornaDifficolta()
    {
        intervalloCorrente = Mathf.Max(
            intervalloMinimo,
            intervalloSpawn - (difficolta * moltiplicatoreDifficolta)
        );
    }

    void SpawnAsteroide()
    {
        // sceglie casualmente tra asteroidi e astronavi
        GameObject[] pool = Random.value > 0.5f ? prefabAsteroidi : prefabAstronavi;

        if (pool.Length == 0) return;

        int indice = Random.Range(0, pool.Length);
        GameObject prefab = pool[indice];
        if (prefab == null) return;

        for (int i = 0; i < maxTentativi; i++)
        {
            Vector2 posCasuale = Random.insideUnitCircle * raggioSpawn;
            Vector3 spawnPos = new Vector3(
                transform.position.x + posCasuale.x,
                nave.transform.position.y,
                transform.position.z + posCasuale.y
            );

            if (nave == null || Vector3.Distance(spawnPos, nave.position) >= raggioDiSicurezza)
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
                return;
            }
        }

        Debug.Log("Spawn saltato: nessuna posizione libera trovata.");
    }
}