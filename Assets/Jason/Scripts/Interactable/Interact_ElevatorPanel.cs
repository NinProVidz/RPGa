using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_ElevatorPanel : MonoBehaviour
{

    public bool isInteractable = false;

    [SerializeField] bool isInteracted = false;

    [SerializeField] KeyCode interactKey = KeyCode.F;

    [SerializeField] Canvas prompt;
    [SerializeField] Transform promptLocation;

    [SerializeField] Transform door1;
    [SerializeField] Transform door2;
    [SerializeField] Transform door3;
    [SerializeField] Transform door4;

    [SerializeField] Vector3 door1OpenPos;
    [SerializeField] Vector3 door1ClosePos;

    [SerializeField] Vector3 door2OpenPos;
    [SerializeField] Vector3 door2ClosePos;

    [SerializeField] Vector3 door3OpenPos;
    [SerializeField] Vector3 door3ClosePos;

    [SerializeField] Vector3 door4OpenPos;
    [SerializeField] Vector3 door4ClosePos;

    [SerializeField] float waitTime;
    [SerializeField] float moveTime;

    [SerializeField] AudioClip buttonPress;
    [SerializeField] AudioSource aS;

    [SerializeField] Elevator elevator;

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

        StartCoroutine(closeElevatorDoors1());

        StartCoroutine(elevator.downElevator());

        StartCoroutine(openElevatorDoors2());
    }



    public IEnumerator openElevatorDoors2()
    {
        yield return new WaitForSeconds(waitTime);

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            door3.localPosition = Vector3.Lerp(door3ClosePos, door3OpenPos, elapsed / moveTime);
            door4.localPosition = Vector3.Lerp(door4ClosePos, door4OpenPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator closeElevatorDoors1()
    {
        yield return new WaitForSeconds(waitTime);

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
