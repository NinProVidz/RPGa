using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEyelidController : MonoBehaviour
{
    [SerializeField] GameObject topLid;
    [SerializeField] GameObject bottomLid;

    [SerializeField] float topMin;
    [SerializeField] float topMax;
    [SerializeField] float bottomMin;
    [SerializeField] float bottomMax;

    [Range(0f, 1f)] [SerializeField] float eyePercent;
    [SerializeField] float animationDuration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float topAngle = topMin + (topMax - topMin) * eyePercent;
        topLid.transform.localRotation = Quaternion.Euler(topAngle, 0, 0);
        float bottomAngle = bottomMin + (bottomMax - bottomMin) * eyePercent;
        bottomLid.transform.localRotation = Quaternion.Euler(bottomAngle, 0, 0);
    }

    public IEnumerator CloseEyes()
    {
        yield return AnimateEyes(eyePercent, 0f);
    }

    public IEnumerator OpenEyes()
    {
        yield return AnimateEyes(eyePercent, 1f);
    }

    private IEnumerator AnimateEyes(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            eyePercent = Mathf.Lerp(from, to, elapsed / animationDuration);
            yield return null;
        }
        eyePercent = to;
    }
}
