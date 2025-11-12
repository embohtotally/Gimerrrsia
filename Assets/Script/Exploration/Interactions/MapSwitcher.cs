using UnityEngine;
using System.Collections.Generic; // Needed for Lists

public class ObjectCycler : MonoBehaviour
{
    [Header("Object List")]
    [Tooltip("The list of GameObjects to cycle through. The first one (Element 0) should be the one active at the start.")]
    public List<GameObject> objectsToCycle;

    [Header("UI Element")]
    [Tooltip("The 'Press F' GameObject that appears when you are close.")]
    public GameObject interactIndicator;

    // --- Private Variables ---
    private bool playerIsInside = false; // Tracks if player is in the trigger
    private int currentActiveIndex = 0;  // Tracks which object in the list is active

    void Start()
    {
        // 1. Turn off interact UI at the start
        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false); // Hide the "Press F" GameObject
        }

        // 2. Initialize the object list
        // We assume Element 0 is the starting object
        for (int i = 0; i < objectsToCycle.Count; i++)
        {
            if (objectsToCycle[i] != null)
            {
                // Activate the first object, deactivate all others
                objectsToCycle[i].SetActive(i == 0);
            }
        }
        currentActiveIndex = 0;
    }

    void Update()
    {
        // Check if player is inside and 'F' is pressed
        if (playerIsInside && Input.GetKeyDown(KeyCode.F))
        {
            SwapObjects();
        }
    }

    // This function now instantly swaps the objects
    private void SwapObjects()
    {
        // Deactivate the current object
        if (objectsToCycle[currentActiveIndex] != null)
        {
            objectsToCycle[currentActiveIndex].SetActive(false);
        }

        // Find the index of the *next* object
        // The '%' (modulo) operator makes it wrap around the list automatically
        int nextIndex = (currentActiveIndex + 1) % objectsToCycle.Count;

        // Activate the next object
        if (objectsToCycle[nextIndex] != null)
        {
            objectsToCycle[nextIndex].SetActive(true);
        }

        // Update our tracker
        currentActiveIndex = nextIndex;
    }

    // --- Trigger Zone Detection (2D) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the Player
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(true); // Show 'F' icon GameObject
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object that exited is the Player
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
            if (interactIndicator != null)
            {
                interactIndicator.SetActive(false); // Hide 'F' icon GameObject
            }
        }
    }
}