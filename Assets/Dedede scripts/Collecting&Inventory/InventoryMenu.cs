using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct InventoryTogglePair
{
    public GameObject inventory;
    public Toggle toggle;
}

public class InventoryMenu : MonoBehaviour
{
    public List<InventoryTogglePair> inventoryToggles;

    private Dictionary<GameObject, Toggle> inventoryTogglesDict;

    public AudioClip hoverSFX;

    public AudioClip selectSFX;

    private void Awake()
    {
        inventoryTogglesDict = new Dictionary<GameObject, Toggle>();
        foreach (var pair in inventoryToggles)
        {
            inventoryTogglesDict[pair.inventory] = pair.toggle;
        }
    }

    private void OnEnable()
    {
        foreach (GameObject inv in inventoryTogglesDict.Keys)
        {
            inv.SetActive(inventoryTogglesDict.GetValueOrDefault(inv).isOn);
        }


    }

    public void ChangeInv()
    {
        foreach(GameObject inv in inventoryTogglesDict.Keys)
        {
            inv.SetActive(inventoryTogglesDict.GetValueOrDefault(inv).isOn);
        }
    }
    
}
