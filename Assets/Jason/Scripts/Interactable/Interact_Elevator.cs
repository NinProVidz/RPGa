using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Elevator : MonoBehaviour, IInteractable
{
    public bool isInteractable = false;

    [SerializeField] bool isInteracted = false;

    [SerializeField] KeyCode interactKey = KeyCode.F;


    [SerializeField] Canvas prompt;
    [SerializeField] Transform promptLocation;

    [SerializeField] Transform door1;
    [SerializeField] Transform door2;

    [SerializeField] Vector3 door1OpenPos;
    [SerializeField] Vector3 door1ClosePos;

    [SerializeField] Vector3 door2OpenPos;
    [SerializeField] Vector3 door2ClosePos;

    [SerializeField] float waitTime;
    [SerializeField] float moveTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Interactable()
    {
        if (isInteracted == false)
        {
            isInteractable = true;

            if (Input.GetKey(interactKey))
            {
                Interact();
            }
        }
    }

    public void UnInteract()
    {
        isInteractable = false;
    }

    public void Interact()
    {
        isInteracted = true;

        StartCoroutine(openElevator());
    }

    private IEnumerator openElevator()
    {
        yield return new WaitForSeconds(waitTime);

        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            door1.localPosition = Vector3.Lerp(door1ClosePos, door1OpenPos, elapsed / moveTime);
            door2.localPosition = Vector3.Lerp(door2ClosePos, door2OpenPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void LateUpdate()
    {
        prompt.enabled = isInteractable && !isInteracted;



    }
}
