using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject[] inventory;

    public AudioClip hoverSFX;

    public AudioClip selectSFX;

    private void OnEnable()
    {
        foreach (GameObject inv in inventory)
        {
            inv.SetActive(false);
        }
    }

    public void OpenInv(int i)
    {
        foreach(GameObject inv in inventory)
        {
            inv.SetActive(false);
        }

        inventory[i].SetActive(true);
    }
    
}
