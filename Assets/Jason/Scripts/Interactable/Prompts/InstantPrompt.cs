using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantPrompt : MonoBehaviour
{
    public Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized, Vector3.up);
    }
}
