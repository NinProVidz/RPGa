using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_OpenVent : MonoBehaviour, IInteractable
{

    public bool isInteractable = false;

    [SerializeField] KeyCode interactKey = KeyCode.F;

    [SerializeField] float interactProgress = 0f;
    [SerializeField] float interactRate = 0.5f;
    [SerializeField] float interactDeRate = 0.3f;

    [SerializeField] ProgressPrompt prompt;
    [SerializeField] Transform promptLocation;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Interactable()
    {
        isInteractable = true;

        if (Input.GetKey(interactKey))
        {
            interactProgress = Mathf.Clamp01(interactProgress + interactRate * Time.deltaTime);
        }
        else
        {
            interactProgress = Mathf.Clamp01(interactProgress - interactRate * Time.deltaTime);
        }

        if (interactProgress >= 1)
        {
            Interact();
        }

    }

    public void UnInteract()
    {
        isInteractable = false;
    }

    public void Interact()
    {
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        prompt.canvas.enabled = isInteractable;
        if (isInteractable != true)
        {
            interactProgress = 0;
        }
        else
        {
            prompt.transform.position = promptLocation.position;
            prompt.progressBar.fillAmount = interactProgress;
        }

        
        
    }
}
