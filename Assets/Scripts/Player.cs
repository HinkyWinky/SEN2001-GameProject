using System.Collections;
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
    private Animator anim;

    // Inspector Variables
    [SerializeField, Range(0.01f, 1f)] private float groundClearance = 0.1f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxMoveSpeed = 2.75f; // max speed while moving
    [SerializeField, Range(0f, 100f)] private float maxMoveAcceleration = 40f; // max acceleration while moving
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 0.6f; // jump height
    [SerializeField, Range(0f, 1f)] private float lockedAbilityInputsDuration = 0.2f; // input lock duration after finishing abilities
    [SerializeField, Range(0f, 10f)] private float rollDuration = 0.3f; // roll duration
    [SerializeField, Range(0f, 100f)] private float rollDistance = 2f; // max speed while moving
    [SerializeField, Range(0f, 10f)] private float attackDuration = 2f; // attack duration

    // Inputs
    private Vector3 AxisInputs => new Vector3(InputController.AxisInputs.x, 0, InputController.AxisInputs.y); // direct axis inputs
    private Vector3 FixedAxisInputs => (AxisInputs.x * transform.right) + (AxisInputs.z * transform.forward); // axis inputs according to the player rotation
    
    // Movement
    private Vector3 velocity, desiredMoveVelocity, rollVelocity, desiredRollVelocity, forward, rollDirection;
    private Quaternion rotation = Quaternion.identity;

    // Cases
    private enum AnimStates { IDLE, MOVE, JUMP, ROLL, ATTACK };
    private AnimStates curAnimState, lastFrameAnimState;
    private bool onGround, hitGroundFirstTime, lockedAnimState, lockedAbilityInputs;

    private IEnumerator animationCor, abilityCor;
    private WaitForFixedUpdate waitForFixedUpdate;
    private WaitForSeconds waitForLockedAbilityInputsDuration;

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        tra = GetComponent<Transform>();
        anim = GetComponent<Animator>();

        waitForFixedUpdate = new WaitForFixedUpdate();
        waitForLockedAbilityInputsDuration = new WaitForSeconds(lockedAbilityInputsDuration);
    }

    private void Update()
    {
        desiredMoveVelocity = FixedAxisInputs * maxMoveSpeed; // Calculate the desiredMoveVelocity with the axisInputs.
        desiredRollVelocity = FixedAxisInputs.normalized * rollDistance / rollDuration; // Calculate the desiredRollVelocity with the axisInputs.
    }

    private void FixedUpdate()
    {
        CheckGround(); // Check the player is on the ground or not.

        MovementUpdate(); // All movement logic update.
        EnvironmentUpdate(); // All environment logic update.
        AnimationUpdate(); // All animation logic update.

        lastFrameAnimState = curAnimState; // Set last frame animation state.
        rig.rotation = rotation; // Add the sum of all calculated rotations to the player`s rotation.
        rig.velocity = velocity; // Add the sum of all calculated velocities to the player`s velocity.
    }

    private void MovementUpdate()
    {
        if (lockedAnimState) return;
        if (!onGround) return; // Run the function if the player is on the ground.

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
            velocity = Vector3.zero; // Stop the player after hitting the ground.
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
        if (lastFrameAnimState == curAnimState) return;

        switch (curAnimState) // Check the current state.
        {
            case AnimStates.IDLE:
                if (animationCor != null)
                    StopCoroutine(animationCor); // Stop the last animation.
                animationCor = PlayAnimation("Idle", 1.5f, true);
                StartCoroutine(animationCor); // Start the new animation.
                break;
            case AnimStates.MOVE:
                if (animationCor != null)
                    StopCoroutine(animationCor); // Stop the last animation.
                animationCor = PlayAnimation("Move", 1f, true);
                StartCoroutine(animationCor); // Start the new animation. 
                break;
            case AnimStates.JUMP:
                if (animationCor != null)
                    StopCoroutine(animationCor); // Stop the last animation.
                animationCor = PlayAnimation("Jump", 0.71f, false);
                StartCoroutine(animationCor); // Start the new animation. 
                break;
            case AnimStates.ROLL:
                if (animationCor != null)
                    StopCoroutine(animationCor); // Stop the last animation.
                animationCor = PlayAnimation("Roll", rollDuration, false);
                StartCoroutine(animationCor); // Start the new animation. 
                break;
            case AnimStates.ATTACK:
                if (animationCor != null)
                    StopCoroutine(animationCor); // Stop the last animation.
                animationCor = PlayAnimation("Attack", attackDuration, false);
                StartCoroutine(animationCor); // Start the new animation. 
                break;
        }
    }

    #region Movement Methods
    private void Move()
    {
        ChangeAnimState(new Vector3(rig.velocity.x, 0, rig.velocity.z).magnitude <= 0f ? AnimStates.IDLE : AnimStates.MOVE, false);

        float maxSpeedChange = maxMoveAcceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredMoveVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredMoveVelocity.z, maxSpeedChange);
    }

    private void Jump()
    {
        ChangeAnimState(AnimStates.JUMP, false);

        velocity = Vector3.zero; // Remove the player`s previous velocity.
        velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight); // Calculate the required velocity for the target height and add to the player`s velocity.
    }

    private void Roll()
    {
        ChangeAnimState(AnimStates.ROLL, true);
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
        float stopTime = Time.time + rollDuration; // Calculate the roll stop time
        while (Time.time <= stopTime) // rolling
        {
            velocity = rollVelocity; // Set the player`s velocity to the initial rolling velocity.
            yield return waitForFixedUpdate;
        }
        UnlockAnimState();
        yield return waitForLockedAbilityInputsDuration;
        UnlockAbilityInputs();
    }

    private void Attack()
    {
        ChangeAnimState(AnimStates.ATTACK, true);
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
        UnlockAnimState();
        yield return waitForLockedAbilityInputsDuration;
        UnlockAbilityInputs();
    }

    private void Rotate() // call in FixedUpdate()
    {
        if (curAnimState == AnimStates.ROLL)
            rotation = Quaternion.LookRotation(rollDirection, Vector3.up); // Look at the rolling direction.
        else
        {
            Vector3 fixedEnemyPos = new Vector3(EnemyPos.x, tra.position.y, EnemyPos.z); // Enemy`s Position on x and z axis.
            forward = fixedEnemyPos - tra.position; // Direction from the player`s position to the enemy`s position.
            rotation = Quaternion.LookRotation(forward, Vector3.up); // Look at the enemy`s position.
        }
    }
    #endregion

    #region Animation Methods
    private IEnumerator PlayAnimation(string motionName, float second, bool loop)
    {
        anim.Play(motionName, 0, 0);
        float percent = 0f;
        while (percent <= 1f)
        {
            percent += Time.fixedDeltaTime / second;
            float func = percent;
            anim.SetFloat(motionName, percent);

            // if the loop is true, keep running the while loop
            if (loop && percent >= 1f)
                percent = 0f;

            yield return waitForFixedUpdate;
        }
        anim.SetFloat(motionName, 0);
    }
    #endregion

    #region Environment Methods
    private void CheckGround()
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
    #endregion

    #region State Control Methods
    private void ChangeAnimState(AnimStates animState, bool lockState)
    {
        if (!lockedAnimState)
            curAnimState = animState;
        if (lockState)
            lockedAnimState = true;
    }
    private void UnlockAnimState()
    {
        lockedAnimState = false;
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
