using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class HealthBarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image frontBar; // Top, snaps immediately
    [SerializeField] private Image backBar;  // Bottom, eases into place

    [Header("Tweens")]
    [Tooltip("How quickly the back bar eases toward the front bar (in fill‐units/sec)")]
    [SerializeField] private float backLerpSpeed = 0.5f;

    private Coroutine _backLerpRoutine;

    /// <summary>
    /// Call this whenever health changes.
    /// </summary>
    /// <param name="healthPercent">0–1</param>
    public void SetHealthPercent(float healthPercent)
    {
        // Immediately snap the front bar:
        frontBar.fillAmount = Mathf.Clamp01(healthPercent);

        // Restart the back‐bar lerp:
        if (_backLerpRoutine != null)
            StopCoroutine(_backLerpRoutine);
        _backLerpRoutine = StartCoroutine(LerpBackBar());
    }

    private IEnumerator LerpBackBar()
    {
        // Wait one frame so frontBar.fillAmount has "settled"
        yield return null;

        // Lerp backBar.fillAmount down to match frontBar.fillAmount
        while (backBar.fillAmount > frontBar.fillAmount + 0.001f)
        {
            backBar.fillAmount = Mathf.MoveTowards(
                backBar.fillAmount,
                frontBar.fillAmount,
                backLerpSpeed * Time.deltaTime
            );
            yield return null;
        }

        backBar.fillAmount = frontBar.fillAmount;
        _backLerpRoutine = null;
    }
}