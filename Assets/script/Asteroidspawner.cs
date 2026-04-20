using UnityEngine;
using System.Collections.Generic;

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

    [Header("Gestione Memoria")]
    public int maxOggettiSpaziali = 50;

    [Header("Riferimenti")]
    public Camera cameraPrincipale;

    float timer;
    float intervalloCorrente;
    Queue<GameObject> oggettiSpawnati = new Queue<GameObject>();

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

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

    Vector3 GetPosizioneeFuoriCamera()
    {
        float distanza = Mathf.Abs(cameraPrincipale.transform.position.y - nave.position.y);

        Vector3 bottomLeft = cameraPrincipale.ViewportToWorldPoint(new Vector3(0, 0, distanza));
        Vector3 topRight = cameraPrincipale.ViewportToWorldPoint(new Vector3(1, 1, distanza));

        float minX = bottomLeft.x;
        float maxX = topRight.x;
        float minZ = bottomLeft.z;
        float maxZ = topRight.z;

        int lato = Random.Range(0, 4);
        float x, z;

        switch (lato)
        {
            case 0:
                x = minX - Random.Range(1f, raggioSpawn);
                z = Random.Range(minZ, maxZ);
                break;
            case 1:
                x = maxX + Random.Range(1f, raggioSpawn);
                z = Random.Range(minZ, maxZ);
                break;
            case 2:
                x = Random.Range(minX, maxX);
                z = minZ - Random.Range(1f, raggioSpawn);
                break;
            default:
                x = Random.Range(minX, maxX);
                z = maxZ + Random.Range(1f, raggioSpawn);
                break;
        }

        return new Vector3(x, nave.position.y, z);
    }

    void SpawnAsteroide()
    {
        GameObject[] pool = Random.value > 0.5f ? prefabAsteroidi : prefabAstronavi;

        if (pool.Length == 0) return;

        int indice = Random.Range(0, pool.Length);
        GameObject prefab = pool[indice];

        if (prefab == null) return;

        for (int i = 0; i < maxTentativi; i++)
        {
            Vector3 spawnPos = GetPosizioneeFuoriCamera();

            if (nave == null || Vector3.Distance(spawnPos, nave.position) >= raggioDiSicurezza)
            {
                Vector3 posizioneTarget = nave.position;
                Vector3 direzioneViaggio = (posizioneTarget - spawnPos);
                direzioneViaggio.y = 0;
                direzioneViaggio.Normalize();

                Quaternion rotazioneVersoIlGiocatore = Quaternion.LookRotation(direzioneViaggio);

                GameObject nuovoOggetto = Instantiate(prefab, spawnPos, rotazioneVersoIlGiocatore);
                oggettiSpawnati.Enqueue(nuovoOggetto);

                if (oggettiSpawnati.Count > maxOggettiSpaziali)
                {
                    GameObject oggettoVecchio = oggettiSpawnati.Dequeue();
                    if (oggettoVecchio != null)
                    {
                        Destroy(oggettoVecchio);
                    }
                }

                return;
            }
        }
    }
}