using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Quaternion originalRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Shake as a small rotational offset
            transform.localRotation = originalRot * Quaternion.Euler(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRot;
    }

    public void ShakeNow(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(duration, magnitude));
    }
}
