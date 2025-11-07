
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

[RequireComponent(typeof(Collider2D))]
public class SceneTrigger : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("The exact name of the scene you want to load.")]
    public string sceneNameToLoad;

    // This function automatically runs when another collider enters this object's trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the Player
        if (other.CompareTag("Player"))
        {
            // Check if a scene name has been provided
            if (!string.IsNullOrEmpty(sceneNameToLoad))
            {
                SceneManager.LoadScene(sceneNameToLoad);
            }
            else
            {
                Debug.LogError("SceneNameToLoad is not set on the trigger object!", this);
            }
        }
    }

    // This ensures the collider is always set as a trigger
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
}