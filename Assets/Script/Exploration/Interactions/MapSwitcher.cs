using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Drag your 'MapManager' GameObject here.")]
    public MapManager mapManager;

    [Tooltip("Which map index from the manager's list should this trigger activate?")]
    public int mapIndexToShow;

    [Header("UI")]
    [Tooltip("The 'Press F' GameObject.")]
    public GameObject interactIndicator;

    // --- Private Variables ---
    private bool playerIsInside = false;

    void Start()
    {
        // Hide the indicator at the start
        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false);
        }
    }

    void Update()
    {
        // Check if player is inside and 'F' is pressed
        if (playerIsInside && Input.GetKeyDown(KeyCode.F))
        {
            // This is it! We just tell the manager what to do.
            // The manager handles all the logic.
            if (mapManager != null)
            {
                mapManager.ActivateMap(mapIndexToShow);
            }
        }
    }

    // --- Trigger Zone Detection (2D) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(false);
            }
        }
    }
}