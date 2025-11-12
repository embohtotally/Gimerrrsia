using UnityEngine;

using UnityEngine.EventSystems;

using DG.Tweening;



/// <summary>

/// 🎭 Persona-like animated button controller

/// Adds floating, hover, and press/release effects to UI buttons.

/// Fully customizable via inspector.

/// </summary>

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

{

    [Header("🌊 Floating Settings")]

    public bool enableFloating = true;

    public float floatXDistance = 0f;   // horizontal float distance

    public float floatYDistance = 10f;  // vertical float distance

    public float floatDuration = 2f;

    public Ease floatEase = Ease.InOutSine;



    [Header("✨ Hover Settings")]

    public bool enableHover = true;

    public float hoverScaleMultiplier = 1.15f;

    public float hoverDuration = 0.25f;

    public Ease hoverEase = Ease.OutQuad;



    [Header("👆 Press Settings")]

    public bool enablePress = true;

    public float pressScaleMultiplier = 0.9f;

    public float pressDuration = 0.15f;

    public Ease pressEase = Ease.InOutQuad;



    [Header("🎯 Reset Settings")]

    public float resetDuration = 0.2f;

    public Ease resetEase = Ease.OutBack;



    private RectTransform rectTransform;

    private Vector3 originalScale;

    private Tween floatingTween;



    void Start()

    {

        rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;



        if (enableFloating)

            StartFloating(); // Floating loop starts once and never stops

    }



    /// <summary>

    /// Starts a looping float animation along X/Y axes

    /// </summary>

    private void StartFloating()

    {

        Vector2 offset = new Vector2(floatXDistance, floatYDistance);



        if (offset == Vector2.zero)

            return; // No movement if both are 0



        floatingTween = rectTransform.DOAnchorPos(rectTransform.anchoredPosition + offset, floatDuration)

            .SetEase(floatEase)

            .SetLoops(-1, LoopType.Yoyo)

            .SetUpdate(true); // continues even when hovering/pressing

    }



    // 🔹 Hover In

    public void OnPointerEnter(PointerEventData eventData)

    {

        if (enableHover)

        {

            rectTransform.DOScale(originalScale * hoverScaleMultiplier, hoverDuration)

                .SetEase(hoverEase)

                .SetUpdate(true);

        }

    }



    // 🔹 Hover Out

    public void OnPointerExit(PointerEventData eventData)

    {

        if (enableHover)

        {

            rectTransform.DOScale(originalScale, resetDuration)

                .SetEase(resetEase)

                .SetUpdate(true);

        }

    }



    // 🔹 Press Down

    public void OnPointerDown(PointerEventData eventData)

    {

        if (enablePress)

        {

            rectTransform.DOScale(originalScale * pressScaleMultiplier, pressDuration)

                .SetEase(pressEase)

                .SetUpdate(true);

        }

    }



    // 🔹 Release

    public void OnPointerUp(PointerEventData eventData)

    {

        if (enablePress || enableHover)

        {

            rectTransform.DOScale(originalScale * hoverScaleMultiplier, resetDuration)

                .SetEase(resetEase)

                .SetUpdate(true);

        }

        else

        {

            rectTransform.DOScale(originalScale, resetDuration)

                .SetEase(resetEase)

                .SetUpdate(true);

        }

    }

}