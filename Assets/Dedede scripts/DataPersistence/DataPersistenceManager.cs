using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    public GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;
    
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
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            SaveGame();
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //load any data from a file using data handler
        this.gameData = dataHandler.Load();
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
        Debug.Log("Data successfully loaded!");
    }

    public void SaveGame()
    {
        //push data to all scripts so they can be updated
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {            
            dataPersistenceObj.SaveData(ref gameData);            
        }

        //save data to file using data handler
        dataHandler.Save(gameData);
        Debug.Log("Data successfully saved!");
    }

    /*private void OnApplicationQuit()
    {
        SaveGame();
    } */

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);

    }
}
