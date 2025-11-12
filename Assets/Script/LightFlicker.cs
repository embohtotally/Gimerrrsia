using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;

    [Header("Intensity Settings")]
    public float minIntensity = 0.3f;
    public float maxIntensity = 1.0f;

    [Header("Falloff Settings")]
    public float minFalloff = 0.5f;
    public float maxFalloff = 1.5f;

    [Header("Flicker Timing")]
    public float flickerSpeed = 0.05f; // seberapa cepat perubahan
    public float randomDelayMin = 0.05f;
    public float randomDelayMax = 0.2f;

    private float timer;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Tidak ada Light2D di GameObject ini!");
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Randomize intensity dan falloff
            float newIntensity = Random.Range(minIntensity, maxIntensity);
            float newFalloff = Random.Range(minFalloff, maxFalloff);

            light2D.intensity = Mathf.Lerp(light2D.intensity, newIntensity, flickerSpeed);
            light2D.falloffIntensity = Mathf.Lerp(light2D.falloffIntensity, newFalloff, flickerSpeed);

            // Reset timer dengan delay acak
            timer = Random.Range(randomDelayMin, randomDelayMax);
        }
    }

}
