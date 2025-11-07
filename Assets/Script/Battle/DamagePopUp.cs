using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    private TMP_Text textMesh;

    [Header("Animation Settings")]
    public float fadeInDuration = 0.2f;
    public float shrinkFontDuration = 1f;
    public float floatUpDistance = 60f;
    public float floatAndFadeOutDuration = 0.75f;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // This is for normal damage
    public void Setup(int damageAmount, EmotionalState state)
    {
        textMesh.text = $"{state.ToString()}, {damageAmount}";
        StartCoroutine(AnimatePopup());
    }

    // --- NEW METHOD for showing a dodge message ---
    public void SetupAsDodge()
    {
        textMesh.text = "DODGE!";
        // You could make the color different for dodges here if you want
        // textMesh.color = Color.cyan;
        StartCoroutine(AnimatePopup());
    }

    // --- NEW METHOD ---
    // This is a new setup for text messages like "DODGED!"
    public void SetupAsText(string message)
    {
        GetComponent<TMP_Text>().text = message;
        StartCoroutine(AnimatePopup());
    }


    private IEnumerator AnimatePopup()
    {
        float originalFontSize = textMesh.fontSize;
        transform.localScale = Vector3.one;

        textMesh.alpha = 0f;
        textMesh.fontSize = originalFontSize - 4f;

        // Phase 1: Pop In
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeInDuration;
            textMesh.alpha = Mathf.Lerp(0f, 1f, progress);
            textMesh.fontSize = Mathf.Lerp(originalFontSize - 4, originalFontSize + 6, progress);
            yield return null;
        }

        // Phase 2: Shrink Font Size
        timer = 0f;
        while (timer < shrinkFontDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / shrinkFontDuration;
            textMesh.fontSize = Mathf.Lerp(originalFontSize + 6, originalFontSize, progress);
            yield return null;
        }

        // Phase 3: Float Up and Fade Out
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, floatUpDistance, 0);

        timer = 0f;
        while (timer < floatAndFadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / floatAndFadeOutDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, progress);
            textMesh.alpha = Mathf.Lerp(1f, 0f, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}