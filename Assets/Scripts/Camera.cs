using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("Settings")]
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public bool LockCameraPosition = false; // For locking the camera position on all axis

    [Header("Camera")]
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;

    private Transform targetObjectTransform;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;
    private GameObject mainCamera;

    void Start()
    {
        targetObjectTransform = transform.Find("TargetObject");
        if (targetObjectTransform == null)
        {
            Debug.LogError("Child not found.");
        }
        if (gameObject.GetComponent<Move>().isGrounded)
        {
            walkCamera.Priority = 1;
            flyCamera.Priority = 0;
        } else
        {
            walkCamera.Priority = 0;
            flyCamera.Priority = 1;
        }

        // get a reference to our main camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        // Initialize rotation with current values
        cinemachineTargetYaw = walkCamera.transform.rotation.eulerAngles.y;
        cinemachineTargetPitch = walkCamera.transform.rotation.eulerAngles.x;
    }

    void Update()
    {
        // If grounded
        if (gameObject.GetComponent<Move>().isGrounded)
        {
            walkCamera.Priority = 1;
            flyCamera.Priority = 0;
            CameraRotation();
        } else
        { 
            walkCamera.Priority = 0;
            flyCamera.Priority = 1;
        }
    }

    private void CameraRotation()
    {
        // if there is an input from the mouse and camera position is not fixed
        if (!LockCameraPosition)
        {
            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(lookX) > threshold || Mathf.Abs(lookY) > threshold)
            {
                cinemachineTargetYaw += lookX;
                cinemachineTargetPitch -= lookY; // Invert Y axis if necessary
            }

            // clamp our rotations so our values are limited to 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            targetObjectTransform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
