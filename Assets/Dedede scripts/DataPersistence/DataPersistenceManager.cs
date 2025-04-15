using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    public GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    
    public static DataPersistenceManager instance { get; private set; } //get the instance publicly but can only modify here

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("THERES MORE THAN ONE INSTANCE OF THIS THING IN THE SCENE AAAAAAAAA");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //load any data from a file using data handler
        //if there is no data, initialize a new game
        if(this.gameData == null)
        {
            Debug.Log("No data has been found, creating a new file...");
            NewGame();
        }
        //push all loaded data to all scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //push data to all scripts so they can be updated
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved the date " + gameData.month + "/" + gameData.day);

        //save data to file using data handler
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);

    }
}
