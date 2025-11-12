using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Tooltip("The list of ALL your maps. Order matters! Map 0, Map 1, etc.")]
    public List<GameObject> allMaps;

    // This tracks which map is currently on.
    // We use -1 to mean "all are off".
    private int currentMapIndex = -1;

    void Start()
    {
        // At the start of the game, turn ALL maps off.
        for (int i = 0; i < allMaps.Count; i++)
        {
            if (allMaps[i] != null)
            {
                allMaps[i].SetActive(false);
            }
        }
        currentMapIndex = -1; // Set our tracker to "all off"
    }

    // This is the public function our triggers will call.
    // It's the one "source of truth".
    public void ActivateMap(int mapIndex)
    {
        // If we're already on this map, do nothing.
        if (mapIndex == currentMapIndex)
        {
            return;
        }

        // --- Turn off the OLD map ---
        // Check if a map is actually on (index isn't -1)
        if (currentMapIndex != -1)
        {
            // Safety check
            if (currentMapIndex < allMaps.Count && allMaps[currentMapIndex] != null)
            {
                allMaps[currentMapIndex].SetActive(false);
            }
        }

        // --- Turn on the NEW map ---
        // Safety check
        if (mapIndex < allMaps.Count && allMaps[mapIndex] != null)
        {
            allMaps[mapIndex].SetActive(true);
            currentMapIndex = mapIndex; // Update our tracker
        }
        else
        {
            Debug.LogWarning("You tried to activate a map index that doesn't exist!");
        }
    }
}