using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public ICollectable possibleCollectable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && possibleCollectable != null)
        {
            possibleCollectable.Collect();
        }
    } 
}
