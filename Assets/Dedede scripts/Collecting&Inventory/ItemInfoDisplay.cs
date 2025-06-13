using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoDisplay : MonoBehaviour
{
    public GameObject displayColumn;
    public TextMeshProUGUI nameText;
    public Image icon;
    public TextMeshProUGUI mainDescText;
    public TextMeshProUGUI descText;

    // Start is called before the first frame update
    void Start()
    {
        displayColumn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void DisplayItemInfo(InventorySlot selectedSlot) 
    {
        if(selectedSlot != null)
        {
            displayColumn.SetActive(true);
            var data = selectedSlot.item.itemData;
            nameText.text = data.displayName;
            icon.sprite = data.Icon;
            mainDescText.text = data.GetMainDesc();
            descText.text = data.GetDesc();

        }
        else
        {
            displayColumn.SetActive(false);
        }
    }
}
