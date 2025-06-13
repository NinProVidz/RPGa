using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public InventoryItem item;

    public InventoryManager inventoryManager;

    public GameObject slotObj;
    public Image icon;
    public Image highlight;
    public Image barlight;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI stackSizeText;
    private Vector3 normalScale;
    private Vector3 hoverHighlightNormalScale;
    public Color itemColor;
    

    private void Start()
    {
        //foreach (Behaviour b in GetComponentsInChildren<Behaviour>(true))
        //{
        //    b.enabled = true;
        //}
        normalScale = slotObj.transform.localScale;
        //hoverColor = highlight.color;
        hoverHighlightNormalScale = hoverHighlight.transform.localScale;
        highlight.color = itemColor;
        barlight.color = itemColor;
    }

    public void ClearSlot()
    {
        icon.enabled = false;
        if (labelText != null)
        {
            labelText.enabled = false;
        }
        stackSizeText.enabled = false;
    }

    public void DrawSlot(InventoryItem item)
    {
        if(item == null)
        {
            ClearSlot();
            return;
        }

        icon.enabled = true;
        icon.sprite = item.itemData.Icon;

        if (labelText != null)
        {
            labelText.enabled = true;
            labelText.text = item.itemData.displayName;
        }

        stackSizeText.enabled = true;
        stackSizeText.text = item.stackSize.ToString();
    }

    public bool isHovering = false;
    public bool wasHovering = false;
    public float hoverSizeFactor = 0.9f;
    public GameObject hoverHighlight;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        PlayerUIManager.instance.PlaySound("Highlight");
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        
    }

    public float hoverTime = 0.2f;
    public float hoverProg = 0;
    public float unHoverTime = 0.1f;
    public float hoverHighlightAlpha = 0.5f;
    

    private void Update()
    {
        if (isHovering)
        {
            hoverProg = Mathf.Clamp01(hoverProg + Time.deltaTime / hoverTime);
            
        }
        else
        {
            
            hoverProg = Mathf.Clamp01(hoverProg - Time.deltaTime / unHoverTime);
            hoverHighlight.transform.localScale = normalScale;
        }

        slotObj.transform.localScale = normalScale * (1-hoverProg + hoverSizeFactor * hoverProg);
        if (!isSelected)
        {
            highlight.enabled = false;
            Image[] hoverHighlightImages = hoverHighlight.GetComponentsInChildren<Image>();

            foreach (Image image in hoverHighlightImages)
            {
                image.color = new Color(itemColor.r, itemColor.g, itemColor.b, hoverHighlightAlpha * hoverProg);
            }
        }
        else
        {
            highlight.enabled = true;
            
        }
        //highlight.color = new Color(normalColor.r, normalColor.g, normalColor.b, hoverHighlightAlpha * hoverProg);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHovering)
        {
            PlayerUIManager.instance.PlaySound("Click");
            inventoryManager.SelectSlot(this);
            StopAllCoroutines();
            StartCoroutine(Select());
        }
    }

    public float hoverHighlightSize = 1.5f;
    public float hoverHighlightTime = 0.3f;

    public bool isSelected = false;

    private IEnumerator Select()
    {
        hoverHighlight.transform.localScale = normalScale;
        float elapsed = 0f;

        while (elapsed < hoverHighlightTime)
        {
            hoverHighlight.transform.localScale = Vector3.Lerp(normalScale, normalScale *hoverHighlightSize, elapsed / hoverHighlightTime);
            Image[] hoverHighlightImages = hoverHighlight.GetComponentsInChildren<Image>();
            foreach (Image image in hoverHighlightImages)
            {
                image.color = new Color(itemColor.r, itemColor.g, itemColor.b, hoverHighlightAlpha * ((hoverHighlightTime - elapsed)/hoverHighlightTime));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        hoverHighlight.transform.localScale = normalScale;

        if (isHovering)
        {

            elapsed = 0f;

            while (elapsed < hoverHighlightTime)
            {
                Image[] hoverHighlightImages = hoverHighlight.GetComponentsInChildren<Image>();
                foreach (Image image in hoverHighlightImages)
                {
                    image.color = new Color(itemColor.r, itemColor.g, itemColor.b, hoverHighlightAlpha * (elapsed / hoverHighlightTime));
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    public void DebugSus()
    {
        Debug.Log("sus");
    }
}
