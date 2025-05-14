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

    [SerializeField] float eyePercent;
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
}
