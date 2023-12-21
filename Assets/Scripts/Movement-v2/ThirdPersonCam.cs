using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Movement_v2;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    public float maxTiltAngle = 75f;
    public float minTiltAngle = -75f;
    public float tiltSpeed = 5f;

    public PlayerMovement playerMovement;

    public GameObject basicCam;
    public GameObject flyCam;

    public CameraStyle currentStyle;

    private float playerSpeed;

    public enum CameraStyle
    {
        Basic,
        Fly
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // TODO: TitlAngle relative to playerSpeed
    private void Update()
    {
        SwitchCameraStyle(playerMovement.flyMode ? CameraStyle.Fly : CameraStyle.Basic);
        playerSpeed = playerMovement.rb.velocity.magnitude;
    }

    private void LateUpdate()
    {
        // Sense of Speed
        freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(50, 65, playerSpeed / playerMovement.moveSpeedAir);
    }

    private void FixedUpdate()
    {
        if (currentStyle == CameraStyle.Basic)
        {
            // rotate orientation
            var transformPosition = transform.position;
            var playerPosition = playerMovement.transform.position;
            var viewDir = playerPosition - new Vector3(transformPosition.x, playerPosition.y, transformPosition.z);
            // Set player orientation
            playerMovement.transform.forward = viewDir.normalized;
        }

        else if (currentStyle == CameraStyle.Fly)
        {
            // Rotate the player's orientation based on the camera's forward direction
            var forward = transform.forward;
            var direction = new Vector3(forward.x, forward.y * 0.9f, forward.z);
            playerMovement.transform.forward = direction;


            var horizontalInput = Input.GetAxis("Horizontal");

            // Get the current tilt angle
            var currentTiltAngle = freeLookCamera.m_Lens.Dutch;

            // Calculate the new tilt angle based on input
            var tiltAngleRatio = currentTiltAngle - horizontalInput * tiltSpeed;

            // If no input is given, smoothly rotate back to zero
            if (Mathf.Approximately(horizontalInput, 0f))
                tiltAngleRatio = Mathf.Lerp(currentTiltAngle, 0f, Time.deltaTime * 0.5f);
            else
                // Apply the new tilt angle within a specified range
                tiltAngleRatio = Mathf.Clamp(tiltAngleRatio, minTiltAngle, maxTiltAngle);

            // Apply the new tilt angle to the camera's rig
            freeLookCamera.m_Lens.Dutch = tiltAngleRatio;
            // playerObj.Rotate(Vector3.forward, tiltAngleRatio);
            playerMovement.transform.Rotate(Vector3.forward, tiltAngleRatio);

            // Push the bird towards the tilt direction
            var tiltForceMultiplier = Math.Abs(tiltAngleRatio * 0.7f) *
                playerSpeed / playerMovement.moveSpeedAir;
            playerMovement.rb.AddForce(playerMovement.transform.up * tiltForceMultiplier, ForceMode.Force);
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        basicCam.SetActive(false);
        flyCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) basicCam.SetActive(true);
        if (newStyle == CameraStyle.Fly) basicCam.SetActive(true);

        currentStyle = newStyle;
    }
}