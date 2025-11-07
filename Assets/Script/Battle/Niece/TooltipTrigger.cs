using UnityEngine;
using UnityEngine.EventSystems; // Required for hover events

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Content")]
    [TextArea(3, 8)]
    public string tooltipContent;

    // This is called when the mouse cursor enters the button's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.instance.ShowTooltip(tooltipContent);
    }

    // This is called when the mouse cursor leaves the button's area
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}