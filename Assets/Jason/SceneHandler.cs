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

    public void LoadSceneSeamless(string sceneToLoad, string sceneToUnload)
    {
        StartCoroutine(LoadThenUnload(sceneToLoad, sceneToUnload));
    }

    private IEnumerator LoadThenUnload(string sceneToLoad, string sceneToUnload)
    {
        // Step 1: Load the new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Step 2: Set the new scene as active
        Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
        if (newScene.IsValid() && newScene.isLoaded)
        {
            SceneManager.SetActiveScene(newScene);
        }

        // Step 3: Move all DontDestroyOnLoad-eligible objects to the persistent scene
        MoveSingletonsToDontDestroy();

        // Optional: Remove duplicate singletons from the new scene
        CleanupDuplicateSingletons();

        // Step 4: Unload the old scene if it's still loaded
        Scene oldScene = SceneManager.GetSceneByName(sceneToUnload);
        if (oldScene.IsValid() && oldScene.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!unloadOp.isDone)
                yield return null;
        }
        else
        {
            Debug.LogWarning($"Scene '{sceneToUnload}' is not valid or already unloaded.");
        }
    }

    private void MoveSingletonsToDontDestroy()
    {
        if (PersistentStuff.instance != null)
        {
            GameObject psGO = PersistentStuff.instance.gameObject;

            if (psGO.scene.name != "DontDestroyOnLoad")
            {
                DontDestroyOnLoad(psGO);
            }
        }

        // Repeat this for other singletons as needed
    }

    private void CleanupDuplicateSingletons()
    {
        // Find all instances in the scene by tag, name, or type
        var allObjects = GameObject.FindObjectsOfType<PersistentStuff>();

        foreach (var obj in allObjects)
        {
            if (obj != PersistentStuff.instance)
            {
                Destroy(obj.gameObject);
            }
        }
    }

    [SerializeField] PhysicalEyelidController eyelidController;

    public void LoadSceneWithBlink(string sceneToLoad, string sceneToUnload)
    {
        StartCoroutine(LoadThenUnloadWithEyelid(sceneToLoad, sceneToUnload));
    }

    private IEnumerator LoadThenUnloadWithEyelid(string sceneToLoad, string sceneToUnload)
    {
        // Step 0: Close eyes
        yield return StartCoroutine(FindObjectOfType<PhysicalEyelidController>().CloseEyes());

        // Step 1: Load the new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Step 2: Set new scene as active
        Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
        if (newScene.IsValid() && newScene.isLoaded)
            SceneManager.SetActiveScene(newScene);

        MoveSingletonsToDontDestroy();
        CleanupDuplicateSingletons();

        // Step 3: Unload the old scene
        Scene oldScene = SceneManager.GetSceneByName(sceneToUnload);
        if (oldScene.IsValid() && oldScene.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!unloadOp.isDone)
                yield return null;
        }

        // Step 4: Open eyes
        yield return StartCoroutine(FindObjectOfType<PhysicalEyelidController>().OpenEyes());
    }

}
