using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public bool isTouchingGround = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isTouchingGround = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isTouchingGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isTouchingGround = false;
        }
    }
}