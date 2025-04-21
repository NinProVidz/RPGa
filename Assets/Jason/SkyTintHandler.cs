using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTintHandler : MonoBehaviour
{

    [SerializeField] Color color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color currentTint = RenderSettings.skybox.GetColor("_Tint");
        RenderSettings.skybox.SetColor("_Tint", color); // Soft blue
    }
}
