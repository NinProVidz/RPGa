using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamlessSceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public string sceneToUnload;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<SceneHandler>().LoadSceneWithBlink(sceneToLoad, sceneToUnload);
        }
    }
}
