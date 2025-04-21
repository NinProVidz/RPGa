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
}
