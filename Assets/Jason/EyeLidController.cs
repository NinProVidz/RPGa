using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeLidController : MonoBehaviour
{
    [SerializeField] public Image topLid;
    [SerializeField] public Image bottomLid;
    [SerializeField] float percentOpen = 1;
    [SerializeField] float topOff = 213;
    [SerializeField] float topRange = 542;
    [SerializeField] float bottomOff = -336;
    [SerializeField] float bottomrange = -536;
    [SerializeField] float animationDuration = 0.5f; // Duration of open/close

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        topLid.rectTransform.position = new Vector3(topLid.transform.position.x, topOff + (topRange * percentOpen) , topLid.transform.position.z);
        bottomLid.rectTransform.position = new Vector3(bottomLid.transform.position.x, bottomOff + (bottomrange * percentOpen), bottomLid.transform.position.z);
    }

    public IEnumerator CloseEyes()
    {
        yield return AnimateEyelids(1f, 0f);
    }

    public IEnumerator OpenEyes()
    {
        yield return AnimateEyelids(0f, 1f);
    }

    private IEnumerator AnimateEyelids(float from, float to)
    {
        float timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            percentOpen = Mathf.Lerp(from, to, timer / animationDuration);
            yield return null;
        }
        percentOpen = to;
    }
}
