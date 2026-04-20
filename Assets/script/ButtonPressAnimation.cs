using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Aggiunge un'animazione di scala a un pulsante UI quando viene premuto.
/// </summary>
public class ButtonPressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Durata dell'animazione di scala")]
    public float scaleDuration = 0.1f;

    [Tooltip("Fattore di riduzione della scala (es. 0.9 = 90%)")]
    public float scaleFactor = 0.9f;

    [Header("Numero - Riquadro di contrasto")]
    public Color numberBackgroundColor = new Color(0f, 0f, 0f, 0.6f); // Nero semitrasparente
    public Vector2 numberBackgroundPadding = new Vector2(20f, 20f);   // Spazio extra attorno al testo

    private Vector3 originalScale;
    private Coroutine currentAnimation;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateScale(originalScale * scaleFactor, scaleDuration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateScale(originalScale, scaleDuration));
    }

    private IEnumerator AnimateScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;
        currentAnimation = null;
    }
}