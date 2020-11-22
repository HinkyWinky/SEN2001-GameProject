using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private InputController InputController => GameManager.Cur.InputController;
    private CameraController CamController => GameManager.Cur.CamController;
    private Enemy Enemy => GameManager.Cur.Enemy;
    private Vector3 EnemyPos => Enemy.transform.position;
    
    private Rigidbody rig;
    private Transform tra;

    [SerializeField, Range(0f, 100f)] private float maxMoveSpeed = 3.5f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxMoveAcceleration = 15f; // max acceleration while moving
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 1f; // jump height
    [SerializeField, Range(0f, 10f)] private float rollHeight = 0.2f; // roll height
    [SerializeField, Range(0f, 100f)] private float rollSpeed = 6f; // max speed while moving

    // inputs
    private Vector3 AxisInputs => new Vector3(InputController.AxisInputs.x, 0, InputController.AxisInputs.y); // direct axis inputs
    private Vector3 FixedAxisInputs => (AxisInputs.x * transform.right) + (AxisInputs.z * transform.forward); // axis inputs according to the player rotation
    // movement
    private Vector3 velocity, desiredMoveVelocity, rollVelocity, targetDir;
    private bool onGround = false;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        tra = GetComponent<Transform>();
    }

    private void Update()
    {
        desiredMoveVelocity = FixedAxisInputs * maxMoveSpeed; // Calculate the desiredMoveVelocity with the axisInputs.
        rollVelocity = FixedAxisInputs.normalized * rollSpeed; // Calculate the rollVelocity with the axisInputs.
    }

    private void FixedUpdate()
    {
        velocity = rig.velocity; // Set the velocity variable by adding the player`s last frame velocity. 

        // Rotate the player
        Rotate();

        // Move the player if it is on the ground.
        if (onGround)
        {
            float maxSpeedChange = maxMoveAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredMoveVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredMoveVelocity.z, maxSpeedChange);
        }

        // Jump the player if the jump input button is clicked by the user.
        if (InputController.jumpInput)
        {
            InputController.jumpInput = false;
            Jump();
        }

        // Roll the player if the roll input button is clicked by the user.
        if (InputController.rollInput)
        {
            InputController.rollInput = false;
            Roll();
        }

        rig.velocity = velocity; // Add the sum of all calculated velocities to the player`s velocity.
        onGround = false; // Always call this line at the end of the FixedUpdate().
    }

    private void Rotate() // call in FixedUpdate()
    {
        Vector3 fixedEnemyPos = new Vector3(EnemyPos.x, tra.position.y, EnemyPos.z); // Enemy`s Position on x and z axis.
        targetDir = fixedEnemyPos - tra.position; // Direction from the player`s position to the enemy`s position.
        rig.rotation = Quaternion.LookRotation(targetDir, Vector3.up); // Look to the enemy`s position.
    }
    private void Jump()
    {
        if (!onGround) return; // Run the function if the player is on the ground.
        velocity = Vector3.zero; // Remove the player`s previous velocity.
        velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight); // Calculate the required velocity for the target height and add to the player`s velocity.
    }
    private void Roll()
    {
        if (!onGround) return; // Run the function if the player is on the ground.
        if (rollVelocity.magnitude < 0.1f) return; // Run the function If there is axis inputs. 
        velocity = Vector3.zero; // Remove the player`s previous velocity.
        velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * rollHeight); // Calculate the required velocity for the target height and add to the player`s velocity.
        velocity += rollVelocity; // Add roll velocity to the player`s velocity.
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            CheckGroundCollision(other);
            rig.velocity = Vector3.zero; // Stop the player after hitting the ground.
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            CheckGroundCollision(other);
        }
    }

    private void CheckGroundCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++) // Check all hit points with the ground.
        {
            Vector3 normal = collision.GetContact(i).normal; // Get the normal of the hit point.
            onGround |= normal.y >= 0.9f; // Calculate the angle between the player and the ground. If angle is enough to move on the ground, return onGround true.
        }
    }
}
