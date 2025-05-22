using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public bool playerCheck;
    public ICollectable possibleCollectable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerCheck == true && possibleCollectable != null)
        {
            possibleCollectable.Collect();
        }
    } 
}
