using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Configurazione Livelli")]
    public int numberOfLevels = 6;
    private bool[] levelCompleted;

    [Header("Aspetto")]
    public Color darkHexColor = new Color(0.1f, 0.1f, 0.15f);
    public Color purpleColor = new Color(0.6f, 0.2f, 0.8f);
    public Color hoverColor = new Color(0.8f, 0.4f, 1f);    // Viola più chiaro per hover
    public Color pressedColor = new Color(0.4f, 0.1f, 0.6f); // Viola più scuro per pressione
    public int borderThickness = 6;
    public float hexagonSize = 150f;
    public float spacing = 200f;
    public float lineThickness = 6f;
    public float dashLength = 25f;
    public float gapLength = 15f;

    [Header("Animazione Pressione")]
    public float pressScaleDuration = 0.1f;  // Durata dell'animazione
    public float pressScaleFactor = 0.9f;    // Scala durante la pressione

    [Header("Sfondo")]
    public bool useStarfield = true;
    public Texture2D customBackgroundTexture;

    [Header("Numero - Riquadro di contrasto")]
    public Color numberBackgroundColor = new Color(0f, 0f, 0f, 0.6f); // Nero semitrasparente
    public Vector2 numberBackgroundPadding = new Vector2(20f, 20f);   // Spazio extra attorno al testo

    private Canvas canvas;
    private List<RectTransform> hexagonRects = new List<RectTransform>();
    private List<GameObject> connectionLines = new List<GameObject>();
    private List<Coroutine> activeAnimations = new List<Coroutine>();

    void Start()
    {
        // Esempio di completamento livelli
        levelCompleted = new bool[numberOfLevels];
        levelCompleted[0] = true;
        levelCompleted[1] = true;
        levelCompleted[2] = false;
        levelCompleted[3] = true;
        levelCompleted[4] = false;
        levelCompleted[5] = false;

        SetupCanvas();
        CreateBackground();
        CreateHexagons();
        CreateConnections();
    }

    void SetupCanvas()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();
    }

    void CreateBackground()
    {
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        RawImage bgImage = bgObj.AddComponent<RawImage>();

        if (customBackgroundTexture != null)
            bgImage.texture = customBackgroundTexture;
        else if (useStarfield)
            bgImage.texture = GenerateStarfieldTexture();
        else
            bgImage.color = Color.black;

        RectTransform rect = bgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    Texture2D GenerateStarfieldTexture()
    {
        int width = 512, height = 512;
        Texture2D tex = new Texture2D(width, height);
        Color32[] pixels = new Color32[width * height];
        Color32 black = new Color32(0, 0, 0, 255);
        for (int i = 0; i < pixels.Length; i++) pixels[i] = black;

        System.Random rand = new System.Random();
        for (int i = 0; i < 800; i++)
        {
            int x = rand.Next(0, width);
            int y = rand.Next(0, height);
            int b = rand.Next(180, 256);
            pixels[y * width + x] = new Color32((byte)b, (byte)b, (byte)b, 255);
        }
        tex.SetPixels32(pixels);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.Apply();
        return tex;
    }

    void CreateHexagons()
    {
        Texture2D hexTex = GenerateHexagonTexture();
        Sprite hexSprite = Sprite.Create(hexTex, new Rect(0, 0, hexTex.width, hexTex.height), new Vector2(0.5f, 0.5f));
        float startX = -(numberOfLevels - 1) * spacing / 2f;

        for (int i = 0; i < numberOfLevels; i++)
        {
            GameObject hexObj = new GameObject($"Level_{i + 1}");
            hexObj.transform.SetParent(canvas.transform, false);

            // Pulsante con effetti hover/pressione
            Button btn = hexObj.AddComponent<Button>();
            int levelIndex = i;
            btn.onClick.AddListener(() => OnLevelSelected(levelIndex + 1));

            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = hoverColor;
            colors.pressedColor = pressedColor;
            colors.selectedColor = purpleColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;

            // Animazione di scala alla pressione
            var pressAnim = hexObj.AddComponent<ButtonPressAnimation>();
            pressAnim.scaleDuration = pressScaleDuration;
            pressAnim.scaleFactor = pressScaleFactor;

            // Immagine esagono
            Image hexImage = hexObj.AddComponent<Image>();
            hexImage.sprite = hexSprite;
            hexImage.type = Image.Type.Simple;
            hexImage.preserveAspect = true;

            // ---- CONTENITORE PER NUMERO + RIQUADRO ----
            GameObject numberContainer = new GameObject("NumberContainer");
            numberContainer.transform.SetParent(hexObj.transform, false);
            RectTransform containerRect = numberContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            // RIQUADRO DI CONTRASTO (dietro il testo)
            GameObject bgNumberObj = new GameObject("NumberBackground");
            bgNumberObj.transform.SetParent(numberContainer.transform, false);
            Image bgImage = bgNumberObj.AddComponent<Image>();
            bgImage.color = numberBackgroundColor;
            bgImage.raycastTarget = false; // Non interferisce con i click

            // TESTO DEL NUMERO
            GameObject textObj = new GameObject("LevelNumber");
            textObj.transform.SetParent(numberContainer.transform, false);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = (i + 1).ToString();
            tmp.fontSize = 48;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = purpleColor;
            tmp.fontStyle = FontStyles.Bold;

            // Layout: testo al centro, sfondo dietro che si adatta al testo
            RectTransform bgRect = bgNumberObj.GetComponent<RectTransform>();
            RectTransform textRect = textObj.GetComponent<RectTransform>();

            // Imposta ancora e pivot per centrare
            bgRect.anchorMin = new Vector2(0.5f, 0.5f);
            bgRect.anchorMax = new Vector2(0.5f, 0.5f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);

            // Forza il calcolo delle dimensioni del testo
            tmp.ForceMeshUpdate();
            Vector2 textSize = tmp.GetRenderedValues(false);
            if (textSize == Vector2.zero) textSize = new Vector2(60, 60); // fallback

            // Dimensiona lo sfondo in base al testo + padding
            bgRect.sizeDelta = textSize + numberBackgroundPadding;
            textRect.sizeDelta = textSize; // opzionale, ma il testo si adatterà comunque

            // Posizione: entrambi centrati (0,0) rispetto al contenitore
            bgRect.anchoredPosition = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            // (Opzionale) Arrotonda gli angoli dello sfondo
            // Aggiungi un componente "UnityEngine.UI.Extensions.UICircle" o simile se vuoi angoli arrotondati,
            // altrimenti lascia quadrato.

            // Posizionamento esagono principale
            RectTransform hexRect = hexObj.GetComponent<RectTransform>();
            hexRect.sizeDelta = new Vector2(hexagonSize, hexagonSize);
            hexRect.anchorMin = new Vector2(0.5f, 0.5f);
            hexRect.anchorMax = new Vector2(0.5f, 0.5f);
            hexRect.anchoredPosition = new Vector2(startX + i * spacing, 0);

            hexagonRects.Add(hexRect);
        }
    }

    Texture2D GenerateHexagonTexture()
    {
        int size = 256;
        Texture2D tex = new Texture2D(size, size);
        Color32 clear = new Color32(0, 0, 0, 0);
        Color32 dark = darkHexColor;
        Color32 purple = purpleColor;

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                tex.SetPixel(x, y, clear);

        float radius = size / 2f - borderThickness;
        Vector2 center = new Vector2(size / 2f, size / 2f);
        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad - 30f * Mathf.Deg2Rad;
            Vector2 v = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vertices.Add(center + v);
        }

        FillPolygon(tex, vertices, dark);
        for (int i = 0; i < 6; i++)
        {
            Vector2 p1 = vertices[i];
            Vector2 p2 = vertices[(i + 1) % 6];
            DrawThickLine(tex, p1, p2, purple, borderThickness);
        }

        tex.Apply();
        return tex;
    }

    void FillPolygon(Texture2D tex, List<Vector2> vertices, Color32 color) { /* ... stessa implementazione di prima ... */ }
    void DrawThickLine(Texture2D tex, Vector2 p1, Vector2 p2, Color32 color, int thickness) { /* ... stessa ... */ }

    void CreateConnections() { /* ... identico a prima ... */ }
    void DrawSolidLine(RectTransform from, RectTransform to, Color color, int thickness) { /* ... identico ... */ }
    void DrawDashedLine(RectTransform from, RectTransform to, Color color, float dashLen, float gapLen, int thickness) { /* ... identico ... */ }

    void OnLevelSelected(int levelNumber)
    {
        Debug.Log($"Livello {levelNumber} selezionato");
        // Carica la scena (esempio)
        // SceneManager.LoadScene("Level" + levelNumber);
    }
}