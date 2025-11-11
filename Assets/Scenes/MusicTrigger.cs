
using UnityEngine;

public class MusicPlayerTrigger : MonoBehaviour
{

    [Header("Music Settings")]
    [Tooltip("The exact name of the music track to play (e.g., 'Theme', 'Level1')")]
    public string musicName;

    // This method is called when the script instance is being loaded.
    void Start()
    {
        
        if (string.IsNullOrEmpty(musicName))
        {
            Debug.LogWarning($"[MusicPlayerTrigger] No 'Music Name' was set in the Inspector for the object '{gameObject.name}'. No music will be played.", this);
            return;
        }

        
        if (AudioManager.instance != null)
        {
           
            Debug.Log($"[MusicPlayerTrigger] Telling AudioManager to play: {musicName}");
            AudioManager.instance.PlayMusic(musicName);
        }
        else
        {
           
            Debug.LogError($"[MusicPlayerTrigger] Could not find AudioManager.instance! Make sure an AudioManager is in the scene and active.", this);
        }
    }

}