using UnityEngine;

public class InputCtrl : MonoBehaviour
{
    private Vector2 axisInputs;
    public Vector2 AxisInputs => axisInputs;
    [HideInInspector] public bool jumpInput = false;
    [HideInInspector] public bool rollInput = false;
    [HideInInspector] public bool attackInput = false;
    [HideInInspector] public bool defenceInput = false;

    private void Update()
    {
        SetAxisInputs(); // Set axisInputs with the user input (wasd and controller`s analog).
        SetJumpInput();
        SetRollInput();
        SetAttackInput();
        SetDefenceInput();
    }

    public void ResetAllInputs()
    {
        jumpInput = false;
        rollInput = false;
        attackInput = false;
        defenceInput = false;
    }

    private void SetAxisInputs()
    {
        axisInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // Set the axisInputs by getting from the user.
        axisInputs = Vector2.ClampMagnitude(axisInputs, 1f); // Normalize the axisInputs.
    }

    private void SetJumpInput()
    {
        jumpInput |= Input.GetKeyDown(KeyCode.Space);
    }

    private void SetRollInput()
    {
        rollInput |= Input.GetKeyDown(KeyCode.M);
    }

    private void SetAttackInput()
    {
        attackInput |= Input.GetKeyDown(KeyCode.B);
    }

    private void SetDefenceInput()
    {
        defenceInput |= Input.GetKey(KeyCode.N);
    }
}
