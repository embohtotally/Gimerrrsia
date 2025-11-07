using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    public GameObject tooltipObject;
    public TMP_Text tooltipText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ensure tooltip is hidden at the start
        if (tooltipObject != null)
        {
            tooltipObject.SetActive(false);
        }
    }

    void Update()
    {
        // Make the tooltip follow the mouse cursor
        if (tooltipObject != null && tooltipObject.activeSelf)
        {
            // We add a small offset so the tooltip doesn't sit directly under the cursor
            tooltipObject.transform.position = Input.mousePosition + new Vector3(15, -15, 0);
        }
    }

    public void ShowTooltip(string content)
    {
        if (tooltipObject != null)
        {
            tooltipText.text = content;
            tooltipObject.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (tooltipObject != null)
        {
            tooltipObject.SetActive(false);
            tooltipText.text = ""; // Clear the text
        }
    }
}