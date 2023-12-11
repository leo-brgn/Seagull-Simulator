using System.Collections;
using System.Collections.Generic;
using Movement_v2;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")] public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public PlayerMovement playerMovement;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public GameObject topDownCam;

    public CameraStyle currentStyle;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // switch styles
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCameraStyle(CameraStyle.Topdown);

        // rotate orientation
        var viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // roate player object

        if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            if (playerMovement.flyMode)
            {
                // Prevent rotation
                horizontalInput = 0;
                if (verticalInput < 0) verticalInput = 0;
            }

            var inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward =
                    Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if (currentStyle == CameraStyle.Combat)
        {
            var dirToCombatLookAt = combatLookAt.position -
                                    new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;
        }
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }
}