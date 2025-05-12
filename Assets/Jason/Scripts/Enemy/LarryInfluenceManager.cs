using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;

public class LarryInfluenceManager : MonoBehaviour
{
    public static LarryInfluenceManager Instance { get; private set; }

    [SerializeField] private PostProcessVolume postProcessingVolume;
    [SerializeField] private float lerpSpeed = 2f;

    private float currentInfluence = 0f; // 0 = no Larry watching, 1 = max influence
    private float targetInfluence = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void RegisterInfluence(float influence)
    {
        // Called by Larrys each frame they are watching.
        targetInfluence = Mathf.Max(targetInfluence, influence);
    }

    private void LateUpdate()
    {
        // Smooth the effect
        currentInfluence = Mathf.Lerp(currentInfluence, targetInfluence, Time.deltaTime * lerpSpeed);
        ApplyPostProcessing(currentInfluence);
        targetInfluence = 0f; // Reset for next frame
    }

    private void ApplyPostProcessing(float intensity)
    {

        postProcessingVolume.weight = Mathf.Lerp(0f, 1f, intensity);

        // Add more effects as needed
    }
}