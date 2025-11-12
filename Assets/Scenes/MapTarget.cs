
using UnityEngine;

public class MapTargetTrigger2D : MonoBehaviour
{
    [Header("Indicators")]
    public GameObject targetIndicator;      // Map or minimap icon
    public GameObject interactIndicator;    // "Press F" UI prompt

    [Header("Next Target")]
    public GameObject nextTarget;           // The next object to activate

    [Header("Settings")]
    public bool deactivateSelfOnActivate = true;
    public bool requireInRangeToActivate = true;

    private bool playerInRange = false;

    private void Start()
    {
        // Only disable the indicators — never the parent.
        
        if (interactIndicator != null) interactIndicator.SetActive(false);
    }

    private void Update()
    {
        // Only check input if in range
        if (requireInRangeToActivate && !playerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ActivateNextTarget();
        }
    }

    private void ActivateNextTarget()
    {
        // Hide indicators first
        if (targetIndicator != null) targetIndicator.SetActive(false);
        if (interactIndicator != null) interactIndicator.SetActive(false);

        // Only deactivate *this* object when pressing F (not on exit)
        if (deactivateSelfOnActivate)
        {
            gameObject.SetActive(false);
        }

        // Activate next target
        if (nextTarget != null)
        {
            nextTarget.SetActive(true);
        }

        Debug.Log($"Activated next target: {(nextTarget != null ? nextTarget.name : "None")}");
        playerInRange = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (targetIndicator != null) targetIndicator.SetActive(true);
        if (interactIndicator != null) interactIndicator.SetActive(true);

        Debug.Log("Player entered range.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        // Only hide indicators (not the GameObject itself!)
        //if (targetIndicator != null) targetIndicator.SetActive(false);
        if (interactIndicator != null) interactIndicator.SetActive(false);

        Debug.Log("Player left range.");
    }

    private void OnDrawGizmosSelected()
    {
        var col = GetComponent<Collider2D>();
        Gizmos.color = Color.green;
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        else
            Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
