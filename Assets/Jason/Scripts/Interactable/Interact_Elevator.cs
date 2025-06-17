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

    [SerializeField] AudioClip buttonPress;
    [SerializeField] AudioClip doorOpen;
    [SerializeField] AudioClip doorClose;
    [SerializeField] AudioSource aS;

    [SerializeField] Elevator elevator;

    [SerializeField] Interact_Elevator upInteract;
    [SerializeField] Interact_Elevator ElevatorInteract;
    [SerializeField] Interact_Elevator downInteract;

    [SerializeField] int type;

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

        aS.PlayOneShot(buttonPress);

        if(type == 0)
        {
            StartCoroutine(actionZero());
        }
        else if(type == 1)
        {
            StartCoroutine(actionOne());
        }

        

        
    }

    public IEnumerator actionZero()
    {
        
        StartCoroutine(elevator.upElevator());
        yield return new WaitForSeconds(elevator.moveTime);
        StartCoroutine(openElevator());
        yield return new WaitForSeconds(moveTime);
        ElevatorInteract.enabled = true;
        yield return null;
    }

    public IEnumerator actionOne()
    {
        StartCoroutine(upInteract.closeElevator());
        yield return new WaitForSeconds(upInteract.moveTime);
        StartCoroutine(elevator.downElevator());
        yield return new WaitForSeconds(elevator.moveTime);
        StartCoroutine(openElevator());

        yield return null;
    }


    public IEnumerator openElevator()
    {

        aS.PlayOneShot(doorOpen);
        float elapsed = 0f;

        while(elapsed < moveTime)
        {
            door1.localPosition = Vector3.Lerp(door1ClosePos, door1OpenPos, elapsed / moveTime);
            door2.localPosition = Vector3.Lerp(door2ClosePos, door2OpenPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator closeElevator()
    {

        aS.PlayOneShot(doorClose);

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            door1.localPosition = Vector3.Lerp(door1OpenPos, door1ClosePos, elapsed / moveTime);
            door2.localPosition = Vector3.Lerp(door2OpenPos, door2ClosePos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void LateUpdate()
    {
        prompt.enabled = isInteractable && !isInteracted;

        prompt.transform.position = promptLocation.position;

    }
}
