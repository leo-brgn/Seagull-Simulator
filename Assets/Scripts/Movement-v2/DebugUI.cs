using TMPro;
using UnityEngine;

namespace Movement_v2
{
    public class DebugUI : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public ThirdPersonCam thirdPersonCam;
        public TextMeshProUGUI textComponent;

        private void Start()
        {
            // Ensure the TextMeshPro component and ValueProvider are assigned
            if (textComponent == null) Debug.LogError("Text component not assigned to DebugUI script.");
            if (playerMovement == null) Debug.LogError("PlayerMovement not assigned to DebugUI script.");
            if (thirdPersonCam == null) Debug.LogError("ThirdPersonCam not assigned to DebugUI script.");
        }

        private void Update()
        {
            // Update the UI text with the value from the ValueProvider script
            if (textComponent != null && playerMovement != null)
                textComponent.text = $"Speed: {playerMovement.debugSpeed}\n" +
                                     $"Grounded: {playerMovement.grounded}\n" +
                                     $"FlyMode: {playerMovement.flyMode}\n" +
                                     $"Camera Type: {thirdPersonCam.currentStyle}\n" +
                                     $"FOV: {thirdPersonCam.freeLookCamera.m_Lens.FieldOfView}\n";
        }
    }
}