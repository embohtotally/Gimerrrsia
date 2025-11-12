using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class VignetteFlicker : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume volume; // assign Volume di Inspector

    private Vignette vignette;

    [Header("Flicker Settings")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 0.5f;
    public float flickerSpeed = 0.1f; // seberapa cepat transisi
    public float randomDelayMin = 0.05f;
    public float randomDelayMax = 0.2f;

    private float timer;

    void Start()
    {
        if (volume != null && volume.profile.TryGet(out vignette))
        {
            // pastikan vignette ada di profile
        }
        else
        {
            Debug.LogError("Volume tidak memiliki Vignette!");
        }
    }

    void Update()
    {
        if (vignette == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // target intensity acak
            float newIntensity = Random.Range(minIntensity, maxIntensity);

            // smooth transition
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, newIntensity, flickerSpeed);

            // reset timer dengan delay acak
            timer = Random.Range(randomDelayMin, randomDelayMax);
        }
    }

}
