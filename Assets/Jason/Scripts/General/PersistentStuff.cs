using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentStuff : MonoBehaviour
{
    public static PersistentStuff instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
}
