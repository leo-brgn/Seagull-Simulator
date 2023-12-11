using TMPro;
using UnityEngine;

namespace Movement_v2
{
    public class DebugUI : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public TextMeshProUGUI textComponent;

        private void Start()
        {
            // Ensure the TextMeshPro component and ValueProvider are assigned
            if (textComponent == null) Debug.LogError("Text component not assigned to ValueDisplay script.");

            if (playerMovement == null) Debug.LogError("PlayerMovement not assigned to PlayerMovement script.");
        }

        private void Update()
        {
            // Update the UI text with the value from the ValueProvider script
            if (textComponent != null && playerMovement != null)
                textComponent.text = $"Speed: {playerMovement.speed}\n" + $"Grounded: {playerMovement.grounded}\n" +
                                     $"FlyMode: {playerMovement.flymode}\n" +
                                     $"Times Space pressed: {playerMovement.spacePressCount}\n" +
                                     $"Space Time: {playerMovement.lastSpacePressTime}\n";
        }
    }
}