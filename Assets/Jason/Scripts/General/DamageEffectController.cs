using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DamageEffectController : MonoBehaviour
{
    //public CanvasGroup damageFlash;
    public float flashDuration = 0.3f;
    public AudioSource audioSource;
    public AudioClip[] painSounds;
    public CameraShake cameraShake;
    public PostProcessVolume postProcessVolume;
    public float lowHealthThreshold = 0.25f;

    [SerializeField] float minMag = 0.5f;
    [SerializeField] float maxMag = 1f;

    [SerializeField] float minDur = 0.05f;
    [SerializeField] float maxDur = 0.3f;

    private Coroutine flashCoroutine;

    private Coroutine damageVignetteCoroutine;

    public void PlayDamageEffects(float healthPercent, float damageAmount)
    {
        // Normalize the damage (example: assuming max damage is 100)
        float normalizedDamage = Mathf.Clamp01(damageAmount / (PlayerManager.instance.GetComponent<Health>().maxHealth * 0.1f));

        // Flash screen red
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        //flashCoroutine = StartCoroutine(DamageFlashRoutine());

        // Play random pain sound
        if (painSounds.Length > 0)
        {
            AudioClip clip = painSounds[Random.Range(0, painSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        // Camera shake: scale duration & magnitude by damage
        float shakeMagnitude = Mathf.Lerp(minMag, maxMag, normalizedDamage);
        float shakeDuration = Mathf.Lerp(minDur, maxDur, normalizedDamage);
        cameraShake.ShakeNow(shakeDuration, shakeMagnitude);

        // Post-process for low health
        if (healthPercent <= lowHealthThreshold)
        {
            if (postProcessVolume.profile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.Override(0.1f + (0.5f - healthPercent));
            }
        }

        // Trigger damage vignette flash
        if (damageVignetteCoroutine != null)
            StopCoroutine(damageVignetteCoroutine);
        damageVignetteCoroutine = StartCoroutine(DamageVignetteFlash());
    }

    private IEnumerator DamageVignetteFlash()
    {
        if (postProcessVolume.profile.TryGetSettings(out Vignette vignette))
        {
            Color originalColor = vignette.color.value;
            float originalIntensity = vignette.intensity.value;

            vignette.color.Override(Color.red);
            vignette.intensity.Override(0.5f); // Adjust intensity as needed

            float elapsed = 0f;
            float duration = 0.3f; // Duration of the flash

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                vignette.intensity.Override(Mathf.Lerp(0.5f, originalIntensity, t));
                yield return null;
            }

            vignette.color.Override(originalColor);
            vignette.intensity.Override(originalIntensity);
        }
    }

    //private IEnumerator DamageFlashRoutine()
    //{
    //    damageFlash.alpha = 1f;
    //    float elapsed = 0f;
    //
    //    while (elapsed < flashDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        damageFlash.alpha = Mathf.Lerp(1f, 0f, elapsed / flashDuration);
    //        yield return null;
    //    }
    //
    //    damageFlash.alpha = 0f;
    //}
}
