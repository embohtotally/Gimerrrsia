using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToStage : MonoBehaviour
{
    [Header("Scene Loading")]
    [Tooltip("Name of the scene to load. Must be added to Build Settings!")]
    public string targetSceneName;

    [Tooltip("Optional reference to a UI Button to automatically set up the click event")]
    public Button targetButton;

    [Tooltip("Delay before loading the scene in seconds")]
    public float loadDelay = 0.2f;

    private void Start()
    {
        // Automatically set up the button click event if a button is assigned
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(LoadTargetScene);
        }
    }

    /// <summary>
    /// Loads the target scene specified in the inspector
    /// </summary>
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target scene name is not set!");
            return;
        }

        // Cancel any previous load invokes to prevent multiple calls
        CancelInvoke(nameof(LoadScene));
        
        // Play a sound if needed
        // AudioManager.Instance.Play("ButtonClick");
        
        // Invoke the load with a small delay to allow for any animations or sounds to play
        Invoke(nameof(LoadScene), loadDelay);
    }

    /// <summary>
    /// Loads a specific scene by name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadSpecificScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty!");
            return;
        }

        targetSceneName = sceneName;
        LoadTargetScene();
    }

    private void LoadScene()
    {
        try
        {
            SceneManager.LoadScene(targetSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load scene {targetSceneName}: {e.Message}");
            #if UNITY_EDITOR
            Debug.LogWarning("Make sure the scene is added to the Build Settings!\n" +
                           "Go to File > Build Settings... and add your scene to the Scenes In Build list.");
            #endif
        }
    }
}
