using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollower : MonoBehaviour
{
    public float sensitivity = 2f;
    public float maxPitch = 80f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch = Mathf.Clamp(pitch - mouseY, -maxPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
