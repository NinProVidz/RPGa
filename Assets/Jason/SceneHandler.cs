using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTutorialCutScene()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToBunkerScene()
    {
        SceneManager.LoadScene("Bunker");
    }

    public void GoToTutorial2ShortScene()
    {
        SceneManager.LoadScene("Tutorial 2 Short");
    }

    public void GoToHubWorld()
    {
        SceneManager.LoadScene("HubWorld");
    }
}
