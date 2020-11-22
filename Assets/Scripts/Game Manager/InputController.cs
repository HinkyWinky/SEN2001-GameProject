using UnityEngine;

public class InputController : MonoBehaviour
{
    private Vector2 axisInputs;
    public Vector2 AxisInputs => axisInputs;

    public bool jumpInput = false;
    public bool rollInput = false;

    private void Update()
    {
        SetAxisInputs(); // Set axisInputs with the user input (wasd and controller`s analog).
        SetJumpInput();
    }

    private void SetAxisInputs()
    {
        axisInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // Set the axisInputs by getting from the user.
        axisInputs = Vector2.ClampMagnitude(axisInputs, 1f); // Normalize the axisInputs.
    }

    private void SetJumpInput()
    {
        jumpInput |= Input.GetKeyDown(KeyCode.Space);
        rollInput |= Input.GetKeyDown(KeyCode.LeftShift);
    }
}
