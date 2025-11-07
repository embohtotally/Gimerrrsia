using UnityEngine;
using UnityEngine.UI;

public class GameQuit : MonoBehaviour
{
    [Tooltip("Key to press to quit the game")]
    public KeyCode quitKey = KeyCode.Escape;

    [Tooltip("Delay before quitting in seconds")]
    public float quitDelay = 0.2f;

    // Reference to the button if you want to handle it programmatically
    [Header("Optional Button Reference")]
    [Tooltip("Drag a UI Button here to automatically set up the click event")]
    public Button quitButton;

    private void Start()
    {
        // Automatically set up the button click event if a button is assigned
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void Update()
    {
        // Check if the quit key is pressed
        if (Input.GetKeyDown(quitKey))
        {
            QuitGame();
        }
    }

    /// <summary>
    /// Call this method from a UI Button's OnClick event or from code
    /// </summary>
    public void QuitGame()
    {
        // Play a sound if needed
        // AudioManager.Instance.Play("ButtonClick");
        
        // Cancel any previous quit invokes to prevent multiple calls
        CancelInvoke(nameof(DoQuit));
        
        // Invoke the quit with a small delay to allow for any animations or sounds to play
        Invoke(nameof(DoQuit), quitDelay);
    }

    private void DoQuit()
    {
#if UNITY_EDITOR
        // If running in the Unity editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build
        Application.Quit();
#endif
    }
}
