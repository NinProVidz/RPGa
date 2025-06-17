using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    [SerializeField] public float moveTime;

    [SerializeField] Vector3 upPos;
    [SerializeField] Vector3 downPos;

    [SerializeField] AudioClip movingSound;
    [SerializeField] AudioSource aS;

    [SerializeField] LayerMask playerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public IEnumerator upElevator()
    {
        aS.PlayOneShot(movingSound);
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.localPosition = Vector3.Lerp(downPos, upPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator downElevator()
    {
        aS.PlayOneShot(movingSound);
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.localPosition = Vector3.Lerp(upPos, downPos, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
