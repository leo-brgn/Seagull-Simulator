using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SeagullInputs : MonoBehaviour
{
    [Header("Character Input Values")] public Vector2 move;

    public Vector2 look;

    // public bool jump;
    // public bool sprint;
    // public bool fly;
    public bool inventory;
    public float mouseWheel;
    public bool use;
    public bool throwObject;


    [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
    public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnInventory(InputValue value)
    {
        InventoryInput(value.isPressed);
    }

    public void OnMouseWheel(InputValue value)
    {
        MouseWheelInput(value.Get<float>());
    }

    public void OnUse(InputValue value)
    {
        UseInput(value.isPressed);
    }

    public void OnThrowObject(InputValue value)
    {
        ThrowObjectInput(value.isPressed);
    }

#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void InventoryInput(bool newInventoryState)
    {
        inventory = newInventoryState;
    }

    public void MouseWheelInput(float newMouseWheel)
    {
        mouseWheel = newMouseWheel;
    }

    public void UseInput(bool newUseState)
    {
        use = newUseState;
    }

    public void ThrowObjectInput(bool newThrowState)
    {
        throwObject = newThrowState;
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}