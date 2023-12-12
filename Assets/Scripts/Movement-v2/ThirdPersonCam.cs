using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Movement_v2;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    public float minCameraDistance = 2f;
    public float maxCameraDistance = 15f;

    [Header("References")] public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public PlayerMovement playerMovement;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject basicCam;
    public GameObject flyCam;

    public CameraStyle currentStyle;

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

    private void Update()
    {
        SwitchCameraStyle(playerMovement.flyMode ? CameraStyle.Fly : CameraStyle.Basic);


        if (currentStyle == CameraStyle.Basic)
        {
            // rotate orientation
            var transformPosition = transform.position;
            var playerPosition = player.position;
            // The purpose is to get a horizontal direction vector that represents
            // where the camera is pointing relative to the player, ignoring any vertical offset.
            var viewDir = playerPosition - new Vector3(transformPosition.x, playerPosition.y, transformPosition.z);


            // Set player orientation
            orientation.forward = viewDir.normalized;
            playerObj.forward = viewDir.normalized;
        }

        else if (currentStyle == CameraStyle.Fly)
        {
            // Rotate the player's orientation based on the camera's forward direction
            var forward = transform.forward;
            var direction = new Vector3(forward.x, forward.y * 0.9f, forward.z);
            playerObj.forward = direction;
            orientation.forward = direction;
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