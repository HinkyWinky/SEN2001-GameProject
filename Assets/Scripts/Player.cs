using System.Collections;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components of other game objects
    private InputController InputController => GameManager.Cur.InputController;
    private Enemy Enemy => GameManager.Cur.Enemy;
    private Vector3 EnemyPos => Enemy.transform.position;
    
    // Components of the player
    private Rigidbody rig;
    private Transform tra;
    private AnimatorX animX;

    // Inspector Variables
    [SerializeField, Range(0.01f, 1f)] private float groundClearance = 0.1f; // height from the ground
    [SerializeField, Range(0f, 1f), DisableInPlayMode] private float lockedAbilityInputsDuration = 0.2f; // input lock duration after finishing abilities
    [Title("Move", Bold = true)]
    [SerializeField, Range(0f, 100f)] private float maxMoveSpeed = 2.75f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxMoveAcceleration = 40f; // max acceleration while moving
    [Title("Rotation", Bold = true)]
    [SerializeField, Range(500f, 3000f), LabelText("Max Rotation Acceleration For 180")]
    private float maxRotationAcceleration = 1000f; // max acceleration for 180 degree while rotating
    [Title("Jumping", Bold = true)]
    [SerializeField, Range(0f, 10f)] private float jumpForce = 2f; // jumpForce while jumping
    [SerializeField, Range(0f, 10f), DisableInPlayMode] private float jumpHeight = 0.6f; // max height while jumping
    [Title("Rolling", Bold = true)]
    [SerializeField, Range(0f, 100f)] private float rollMoveDistance = 2f; // roll move distance
    [SerializeField, Range(0f, 5f)] private float rollMoveDuration = 0.3f; // roll move duration
    [SerializeField, PropertyRange(0f, "rollMoveDuration"), LabelText("Roll Rotation Duration For 180")]
    private float rollRotationDuration = 0.1f; // roll rotation duration for 180 degree
    [SerializeField, Range(0f, 0.1f), DisableInPlayMode] private float stayStableAfterRollDuration = 0.025f;
    [Title("Attacking", Bold = true)]
    [SerializeField, Range(0f, 10f)] private float attackDuration = 2f; // attack duration

    // Inputs
    private Vector3 AxisInputs => new Vector3(InputController.AxisInputs.x, 0, InputController.AxisInputs.y); // direct axis inputs
    private Vector3 FixedAxisInputs => (AxisInputs.x * Vector3.Cross(Vector3.up,forward)) + (AxisInputs.z * forward); // axis inputs according to the player rotation
    
    // Movement
    private Vector3 velocity, desiredMoveVelocity, rollVelocity, desiredRollVelocity, forward, rollDirection;
    private Quaternion rotation = Quaternion.identity;
    private float JumpSpeed => Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
    private float JumpTime => -2f * JumpSpeed / Physics.gravity.y;

    // Cases
    private enum PlayerStates { IDLE, MOVE, JUMP, ROLL, ATTACK };
    [FoldoutGroup("Read Only Fields"), ShowInInspector, ReadOnly, LabelText("Current Player State")]
    private PlayerStates curPlayerState = PlayerStates.IDLE;
    private PlayerStates lastFramePlayerState = PlayerStates.MOVE;
    [FoldoutGroup("Read Only Fields"), ShowInInspector, ReadOnly]
    private bool onGround, hitGroundFirstTime, lockedPlayerState, lockedAbilityInputs;

    private IEnumerator abilityCor;
    private WaitForFixedUpdate waitForFixedUpdate;
    private WaitForSeconds waitForLockedAbilityInputsDuration, waitForStayStableAfterRollDuration;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        tra = GetComponent<Transform>();
        animX = GetComponent<AnimatorX>();

        waitForFixedUpdate = new WaitForFixedUpdate();
        waitForLockedAbilityInputsDuration = new WaitForSeconds(lockedAbilityInputsDuration);
        waitForStayStableAfterRollDuration = new WaitForSeconds(stayStableAfterRollDuration);
    }

    private void Update()
    {
        desiredMoveVelocity = FixedAxisInputs * maxMoveSpeed; // Calculate the desiredMoveVelocity with the axisInputs.
        desiredRollVelocity = FixedAxisInputs.normalized * rollMoveDistance / rollMoveDuration; // Calculate the desiredRollVelocity with the axisInputs.
    }

    private void FixedUpdate()
    {
        SetOnGround(); // Check the player is on the ground or not.
        SetForward(); // Set the player`s forward direction.

        MovementUpdate(); // All movement logic update.
        EnvironmentUpdate(); // All environment logic update.
        AnimationUpdate(); // All animation logic update.

        lastFramePlayerState = curPlayerState; // Set the last frame player state.
        rig.rotation = rotation; // Add the sum of all calculated rotations to the player`s rotation.
        rig.velocity = velocity; // Add the sum of all calculated velocities to the player`s velocity.
    }

    private void MovementUpdate()
    {
        if (lockedPlayerState) return;
        if (onGround) // Run the function if the player is on the ground.
        {
            // The player moves if the axis inputs are different than zero.
            Move();
            // The player jumps if the jump input button is clicked by the user.
            if (InputController.jumpInput)
            {
                InputController.jumpInput = false;
                Jump();
            }
            // The player rolls if the roll input button is clicked by the user.
            if (InputController.rollInput && !lockedAbilityInputs)
            {
                InputController.rollInput = false;
                if (desiredRollVelocity.magnitude >= 0.1f) // Run the function If there is axis inputs.
                    Roll();
            }
        }
        // The player attacks if the attack input button is clicked by the user.
        if (InputController.attackInput && !lockedAbilityInputs)
        {
            InputController.attackInput = false;
            Attack();
        }
        Rotate(); // The player rotates according to the player animation state.
    }
    private void EnvironmentUpdate()
    {
        if (onGround)
        {
            // vvv...call these lines while the player on the ground...vvv

            // vvv...call these lines one time when the player hits the ground first time...vvv
            if (hitGroundFirstTime) return;
            hitGroundFirstTime = true;
            rig.useGravity = false; // Cancel the gravity while the player on the ground.
            velocity.y = 0f;
        }
        else // inAir
        {
            // vvv...call these lines while the player in the air...vvv
            velocity = rig.velocity;

            // vvv...call these lines one time when the player jumps first time...vvv
            if (!hitGroundFirstTime) return;
            hitGroundFirstTime = false;
            rig.useGravity = true; // Activate the gravity while the player in the air.
        }
    }
    private void AnimationUpdate()
    {
        // Call this method one time when the state is changed.
        if (lastFramePlayerState == curPlayerState) return;

        switch (curPlayerState) // Check the current state.
        {
            case PlayerStates.IDLE:
                animX.StartAnimation("Idle", 1.5f, true, 0.15f);
                break;
            case PlayerStates.MOVE:
                animX.StartAnimation("Move", 1f, true, 0.15f);
                break;
            case PlayerStates.JUMP:
                animX.StartAnimation("Jump", JumpTime, false, 0.1f);
                break;
            case PlayerStates.ROLL:
                animX.StartAnimation("Roll", rollMoveDuration, false, 0f);
                break;
            case PlayerStates.ATTACK:
                animX.StartAnimation("Attack", attackDuration, false, 0.1f);
                break;
        }
    }

    #region Movement Methods
    private void Move()
    {
        ChangePlayerState(new Vector3(rig.velocity.x, 0, rig.velocity.z).magnitude <= 0f ? PlayerStates.IDLE : PlayerStates.MOVE, false);

        float maxSpeedChange = maxMoveAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredMoveVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredMoveVelocity.z, maxSpeedChange);
    }

    private void Jump()
    {
        ChangePlayerState(PlayerStates.JUMP, false);

        velocity += FixedAxisInputs.normalized * jumpForce - velocity; // Remove the player`s previous velocity.
        velocity.y += JumpSpeed; // Calculate the required velocity for the target height and add to the player`s velocity.
    }

    private void Roll()
    {
        ChangePlayerState(PlayerStates.ROLL, true);
        LockAbilityInputs();

        // Start Roll Coroutine
        if (abilityCor != null)
            StopCoroutine(abilityCor);
        abilityCor = RollCor();
        StartCoroutine(abilityCor);
    }
    private IEnumerator RollCor()
    {
        rollDirection = desiredRollVelocity.normalized; // Calculate the rolling direction for rotating.
        rollVelocity = desiredRollVelocity; // Calculate the initial rolling velocity.
        float stopTime = Time.time + rollMoveDuration; // Calculate the roll stop time
        while (Time.time <= stopTime) // rolling
        {
            velocity = rollVelocity; // Set the player`s velocity to the initial rolling velocity.
            yield return waitForFixedUpdate;
        }
        velocity = Vector3.zero;
        yield return waitForStayStableAfterRollDuration;
        UnlockPlayerState();
        yield return waitForLockedAbilityInputsDuration;
        UnlockAbilityInputs();
    }

    private void Attack()
    {
        ChangePlayerState(PlayerStates.ATTACK, true);
        LockAbilityInputs();

        // Start Attack Coroutine
        if (abilityCor != null)
            StopCoroutine(abilityCor);
        abilityCor = AttackCor();
        StartCoroutine(abilityCor);
    }
    private IEnumerator AttackCor()
    {
        float stopTime = Time.time + attackDuration; // Calculate the attack stop time.
        while (Time.time <= stopTime) // attacking
        {
            velocity = Vector3.zero; // Set the player`s velocity to zero.
            yield return waitForFixedUpdate;
        }
        UnlockPlayerState();
        yield return waitForLockedAbilityInputsDuration;
        UnlockAbilityInputs();
    }

    private void Rotate() // call in FixedUpdate()
    {
        if (curPlayerState == PlayerStates.ROLL)
        {
            StartCoroutine(RotateCor(rollRotationDuration, rollDirection));
        }
        else
        {
            var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Look at the enemy`s position.
            float fixedMaxRotationAcceleration = maxRotationAcceleration * Quaternion.Angle(rotation, targetRot) / 180f;
            float maxRotationChange = fixedMaxRotationAcceleration * Time.fixedDeltaTime;
            rotation = Quaternion.RotateTowards(rotation, targetRot, maxRotationChange);
        }
    }
    private IEnumerator RotateCor(float duration, Vector3 targetDir)
    {
        var startRot = rotation;
        var targetRot = Quaternion.LookRotation(targetDir, Vector3.up); // Look at the rolling direction.
        float fixedDuration = duration * Quaternion.Angle(startRot, targetRot) / 180f;
        float percent = 0f;
        while (percent <= 1f)
        {
            percent += Time.deltaTime / fixedDuration;
            rotation = Quaternion.Lerp(startRot, targetRot, percent);
            yield return waitForFixedUpdate;
        }
    }
    #endregion

    #region Environment Methods
    private void SetOnGround()
    {
        Vector3 startPos = transform.position + Vector3.up * 0.2f; // Starting position of the raycast.
        bool isHit = Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 0.2f + groundClearance); // Cast raycast.
        if (isHit && hit.collider.CompareTag("Ground")) // raycast hits the ground.
        {
            onGround = hit.normal.y >= 0.9f; // Angle between the player and the ground is enough to move or not.
            Debug.DrawLine(startPos, hit.point, Color.magenta);
        }
        else // raycast does not hit the ground.
            onGround = false;
    }
    private void SetForward()
    {
        Vector3 fixedEnemyPos = new Vector3(EnemyPos.x, tra.position.y, EnemyPos.z); // Enemy`s Position on x and z axis.
        forward = Vector3.Normalize(fixedEnemyPos - tra.position); // Direction from the player`s position to the enemy`s position.
    }
    #endregion

    #region State Control Methods
    private void ChangePlayerState(PlayerStates playerState, bool lockState)
    {
        if (!lockedPlayerState)
            curPlayerState = playerState;
        if (lockState)
            lockedPlayerState = true;
    }
    private void UnlockPlayerState()
    {
        lockedPlayerState = false;
    }

    private void LockAbilityInputs()
    {
        lockedAbilityInputs = true;
    }
    private void UnlockAbilityInputs()
    {
        lockedAbilityInputs = false;
    }
    #endregion
}
