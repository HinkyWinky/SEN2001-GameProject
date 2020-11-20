using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 5f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 30f; // max acceleration while moving
    [SerializeField] private float rotationSpeed = 0.1f; // rotation speed while rotating
    [SerializeField, Range(0f, 1f)] private float bounciness = 0.5f; // bounciness constant
    [SerializeField] private Rect allowedArea = new Rect(-5f, -5f, 10f, 10f); // borders that the player can reach

    // inputs
    private Vector2 AxisInputs => GameManager.Cur.InputController.AxisInputs;
    // movement
    private Vector3 velocity;
    private Vector3 displacement;
   
    private void Update()
    {
        // Rotate the player if there is user axisInputs.
        if (AxisInputs != Vector2.zero)
        {
            Rotate(AxisInputs);
        }
        Move(AxisInputs);
    }
    
    private void Rotate(Vector2 axisInputs)
    {
        float targetAngle = Mathf.Atan2(axisInputs.x, axisInputs.y) * Mathf.Rad2Deg; // Calculate the targetAngle with the axisInputs.
        float currentAngle = transform.eulerAngles.y; // Set the currentAngle
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime); // Calculate the newAngle to rotate from currentAngle to targetAngle in every frame.
        transform.eulerAngles = new Vector3(0, newAngle, 0); // Set the player direction angle to the newAngle.
    }

    private void Move(Vector2 axisInputs)
    {
        Vector3 desiredVelocity = new Vector3(axisInputs.x, 0, axisInputs.y) * maxSpeed; // Calculate the desiredVelocity with the axisInputs.
        float maxSpeedChange = maxAcceleration * Time.deltaTime; // Calculate the maxSpeedChange in one frame.

        // Change the velocity to the desiredVelocity by increasing or decreasing with the maxSpeedChange.
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

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
