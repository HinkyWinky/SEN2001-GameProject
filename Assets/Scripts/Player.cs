using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 5f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 30f; // max acceleration while moving
    [SerializeField, Range(0f, 1f)] private float bounciness = 0.5f; // bounciness constant
    [SerializeField] private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f); // borders that the player can reach

    // inputs
    private Vector3 AxisInputs => new Vector3(GameManager.Cur.InputController.AxisInputs.x, 0, GameManager.Cur.InputController.AxisInputs.y);
    private Vector3 FixedAxisInputs => (AxisInputs.x * CamController.virtualCam.transform.right) + (AxisInputs.z * CamController.virtualCam.transform.forward);
    // movement
    private Vector3 velocity;
    private Vector3 displacement;
    private float curAngle;

    private float speed;

    private CameraController CamController => GameManager.Cur.CamController;
    private Enemy Enemy => GameManager.Cur.Enemy;

    private void Update()
    {
        // Rotate the player. The player always looks at the enemy. 
        transform.LookAt(Enemy.transform);

        // Move the player.
        Move(FixedAxisInputs);
    }

    private void Move(Vector3 axisInputs)
    {
        Vector3 desiredVelocity = axisInputs * maxSpeed; // Calculate the desiredVelocity with the axisInputs.
        float maxSpeedChange = maxAcceleration * Time.deltaTime; // Calculate the maxSpeedChange in one frame.

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange); // Change the speed to the desiredSpeed by increasing or decreasing with the maxSpeedChange.
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange); // Change the speed to the desiredSpeed by increasing or decreasing with the maxSpeedChange.
        displacement = velocity * Time.deltaTime; // Calculate the displacement in one frame.
        Vector3 newPos = transform.localPosition + displacement; // Calculate the newPos (new position) of the player by adding the displacement to its current position.

        // Check the newPos (new position) of the player is allowed or not.
        if (newPos.x < allowedArea.xMin)
        {
            newPos.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness; // Colliding with wall.
        }
        else if (newPos.x > allowedArea.xMax)
        {
            newPos.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPos.z < allowedArea.yMin)
        {
            newPos.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPos.z > allowedArea.yMax)
        {
            newPos.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPos; // Set the player`s current position to the newPos (new calculated position).
    }
}
