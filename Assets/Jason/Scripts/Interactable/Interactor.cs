using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interactable();

    public void UnInteract();
}

public class Interactor : MonoBehaviour
{

    public Transform InteractorSource;

    public float InteractRange;

    public LayerMask ignoreLayer;

    private IInteractable interactedObj;

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        // Invert the ignoreLayer mask to get only the layers you want to hit
        int layerMask = ~ignoreLayer.value;

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, layerMask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactedObj = interactObj;
                interactObj.Interactable();
            }
            else
            {
                if(interactedObj != null)
                interactedObj.UnInteract();
            }
        }
        else
        {
            if (interactedObj != null)
            interactedObj.UnInteract();
        }
    }
}
