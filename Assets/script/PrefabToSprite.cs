using UnityEngine;

public class PrefabToSprite : MonoBehaviour
{
    [Header("Impostazioni")]
    [SerializeField] private GameObject prefabDaConvertire;
    [SerializeField] private string percorsoSalvataggio = "Assets/Sprites/";

    [ContextMenu("Converti in Sprite")]
    public void ConvertiPrefabInSprite()
    {
        if (prefabDaConvertire == null)
        {
            Debug.LogError("Nessun prefab assegnato!");
            return;
        }

        // Cerca un componente SpriteRenderer nel prefab
        SpriteRenderer spriteRenderer = prefabDaConvertire.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Il prefab non ha un componente SpriteRenderer!");
            return;
        }

        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Lo SpriteRenderer non ha uno sprite assegnato!");
            return;
        }

        // Ottieni lo sprite
        Sprite spriteOttenuto = spriteRenderer.sprite;

        Debug.Log($"Sprite estratto: {spriteOttenuto.name}");
        Debug.Log($"Dimensioni: {spriteOttenuto.rect.width} x {spriteOttenuto.rect.height}");

        // Opzionale: crea un nuovo GameObject con solo lo sprite
        CreaGameObjectConSprite(spriteOttenuto);
    }

    private void CreaGameObjectConSprite(Sprite sprite)
    {
        // Crea un nuovo GameObject
        GameObject nuovoOggetto = new GameObject($"{sprite.name}_Sprite");

        // Aggiungi SpriteRenderer
        SpriteRenderer nuovoRenderer = nuovoOggetto.AddComponent<SpriteRenderer>();
        nuovoRenderer.sprite = sprite;

        // Copia la posizione (opzionale)
        nuovoOggetto.transform.position = transform.position;

        // Seleziona il nuovo oggetto nell'hierarchy
        UnityEditor.Selection.activeGameObject = nuovoOggetto;

        Debug.Log($"Creato nuovo GameObject: {nuovoOggetto.name}");
    }
}