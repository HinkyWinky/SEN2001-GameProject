using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour, IHitable
    {
        // Components of other game objects
        private InputCtrl InputCtrl => GameManager.Cur.InputCtrl;
        private Enemy1 Enemy => GameManager.Cur.Enemy;
        private Vector3 EnemyPos => Enemy.transform.position;
        
        // Components of the player
        private Rigidbody rig;
        private Transform tra;
        private AnimatorX animX;

        // Inspector Variables
        [SerializeField] private Sword sword = default;
        [SerializeField, Min(0)] private int maxHealth = 100;
        [ShowInInspector, ReadOnly] private int health;
        [SerializeField, Range(0.01f, 1f)] private float groundClearance = 0.1f; // height from the ground
        [SerializeField, Range(0.1f, 1f), DisableInPlayMode] private float lockedAbilityInputsDuration = 0.2f; // input lock duration after finishing abilities

        [Title("Move", Bold = true)]
        [SerializeField, Range(0f, 100f)] private float maxMoveSpeed = 2.75f; // max speed while moving
        [SerializeField, Range(0f, 100f)] private float maxMoveAcceleration = 40f; // max acceleration while moving

        [Title("Rotate", Bold = true)]
        [SerializeField, Range(500f, 3000f), LabelText("Max Rotation Acceleration For 180")]
        private float maxRotationAcceleration = 1000f; // max acceleration for 180 degree while rotating

        [Title("Jump", Bold = true)]
        [SerializeField, Range(0f, 10f)] private float jumpForceXZ = 2f; // jumpForceXZ while jumping
        [SerializeField, Range(0.2f, 10f)] private float jumpHeight = 0.6f; // max height while jumping

        [Title("Roll", Bold = true)]
        [SerializeField, Range(0.1f, 5f)] private float rollMoveDuration = 0.3f; // roll move duration
        [SerializeField, Range(0f, 10f)] private float rollMoveDistance = 2f; // roll move distance
        [SerializeField, PropertyRange(0f, "rollMoveDuration"), LabelText("Roll Rotation Duration For 180")]
        private float rollRotationDuration = 0.1f; // roll rotation duration for 180 degree
        [SerializeField, Range(0f, 0.1f), DisableInPlayMode] private float stayStableAfterRollDuration = 0.025f;

        [Title("Animations", Bold = true)]
        [SerializeField, Range(0f, 100f)] private float maxInputAcceleration = 40f; // max acceleration while moving

        [FoldoutGroup("Read Only Fields"), ShowInInspector, ReadOnly, LabelText("Current Player State")]
        private PlayerStates curPlayerState = PlayerStates.IDLE;
        [FoldoutGroup("Read Only Fields"), ShowInInspector, ReadOnly]
        private bool onGround, hitGroundFirstTime, lockedPlayerState, lockedAbilityInputs, isHitAble, isDieHappened;

        private float animInputX, animInputZ, rollAnimDuration;
        private Vector3 velocity, desiredMoveVelocity, rollVelocity, desiredRollVelocity, forward, rollDirection;
        private Quaternion rotation = Quaternion.identity;
        private IEnumerator rotationCor, abilityCor;
        private WaitForSeconds waitForLockedAbilityInputsDuration, waitForStayStableAfterRollDuration;
        private AnimData curAnimData;
        public YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        private Vector3 AxisInputs => new Vector3(InputCtrl.AxisInputs.x, 0, InputCtrl.AxisInputs.y); // direct axis inputs
        private Vector3 FixedAxisInputs => (AxisInputs.x * Vector3.Cross(Vector3.up,forward)) + (AxisInputs.z * forward); // axis inputs according to the player rotation
        private float JumpSpeedY => Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        private float JumpDuration => -2f * JumpSpeedY / Physics.gravity.y;
        private PlayerStates lastFramePlayerState = PlayerStates.JUMP;

        public int Health
        {
            get => health;
            set
            {
                if (value < 0) { health = 0; }
                else if (value > maxHealth) { health = maxHealth; }
                else { health = value; }
            }
        }
        public bool IsDeath => Health == 0;
        public Vector3 Forward => forward;

        #region Mono
        private void Awake()
        {
            rig = GetComponent<Rigidbody>();
            tra = GetComponent<Transform>();
            animX = GetComponent<AnimatorX>();

            waitForFixedUpdate = new WaitForFixedUpdate();
            waitForLockedAbilityInputsDuration = new WaitForSeconds(lockedAbilityInputsDuration);
            waitForStayStableAfterRollDuration = new WaitForSeconds(stayStableAfterRollDuration);

            isHitAble = true;
            Health = maxHealth;
        }
        private void Start()
        {
            GameManager.Cur.EventCtrl.onPlayerHealthChange?.Invoke(Health, maxHealth);
            AnimSequenceData rollSequence = animX.ReturnAnimSequenceData("Roll");
            rollAnimDuration = (rollSequence.Length * rollMoveDuration) / rollSequence.ReturnAnimData("Roll").Length;
        }
        private void Update()
        {
            desiredMoveVelocity = FixedAxisInputs * maxMoveSpeed; // Calculate the desiredMoveVelocity with the axisInputs.
            desiredRollVelocity = FixedAxisInputs.normalized * rollMoveDistance / rollMoveDuration; // Calculate the desiredRollVelocity with the axisInputs.

            MoveBlendTreeUpdate(); // Play move animations according to the input axis.

            if (!isDieHappened && IsDeath)
                Die();
        }
        private void FixedUpdate()
        {
            SetOnGround(); // Check the player is on the ground or not.
            SetForward(); // Set the player`s forward direction.

            MotionUpdate(); // All motion logic update.
            AnimationUpdate(); // All animation logic update.
            EnvironmentUpdate(); // All environment logic update.
           
            lastFramePlayerState = curPlayerState; // Set the last frame player state.
            rig.rotation = rotation; // Add the sum of all calculated rotations to the player`s rotation.
            rig.velocity = velocity; // Add the sum of all calculated velocities to the player`s velocity.
        }
        #endregion
        
        private void MotionUpdate()
        {
            if (lockedPlayerState) return;
            if (onGround)
            {
                // The player moves if the axis inputs are different than zero.
                Move();
                // The player jumps if the jump input button is clicked by the user.
                if (InputCtrl.jumpInput)
                {
                    InputCtrl.jumpInput = false;
                    Jump();
                }
                // The player rolls if the roll input button is clicked by the user.
                if (InputCtrl.rollInput && !lockedAbilityInputs)
                {
                    InputCtrl.rollInput = false;
                    if (desiredRollVelocity.magnitude >= 0.1f) // Run the function If there is axis inputs.
                        Roll();
                }
            }
            // The player attacks if the attack input button is clicked by the user.
            if (InputCtrl.attackInput > 0 && !lockedAbilityInputs)
            {
                Attack(InputCtrl.attackInput);
                if (InputCtrl.attackInput == InputCtrl.maxAttackInput)
                    InputCtrl.attackInput = 0;
            }
            Rotate(); // The player rotates according to the player animation state.
        }
        private void EnvironmentUpdate()
        {
            if (onGround)
            {
                // vvv...call these lines while the player on the ground...vvv

                // vvv...call these lines one time when the player hits the ground isStateUpdatedFirstTime time...vvv
                if (hitGroundFirstTime) return;
                hitGroundFirstTime = true;
                rig.useGravity = false; // Cancel the gravity while the player on the ground.
                velocity.y = 0f;
            }
            else // inAir
            {
                // vvv...call these lines while the player in the air...vvv
                velocity = rig.velocity;

                // vvv...call these lines one time when the player jumps isStateUpdatedFirstTime time...vvv
                if (!hitGroundFirstTime) return;
                hitGroundFirstTime = false;
                rig.useGravity = true; // Activate the gravity while the player in the air.
            }
        }
        private void AnimationUpdate()
        {
            // Call this method one time when the state is changed.
            if (lastFramePlayerState == curPlayerState) return;
            if (curPlayerState == PlayerStates.MOVE && lastFramePlayerState == PlayerStates.IDLE) return;
            if (curPlayerState == PlayerStates.IDLE && lastFramePlayerState == PlayerStates.MOVE) return;
            switch (curPlayerState) // Check the current state.
            {
                case PlayerStates.IDLE:
                case PlayerStates.MOVE:
                    curAnimData = animX.ReturnAnimData("Move");
                    animX.StartAnimation(curAnimData);
                    break;
                case PlayerStates.JUMP:
                    curAnimData = animX.ReturnAnimData("Jump");
                    animX.StartAnimation(curAnimData, JumpDuration);
                    break;
                case PlayerStates.ROLL:
                    animX.StartAnimationSequence("Roll", rollAnimDuration);
                    break;
                case PlayerStates.ATTACK0:
                    curAnimData = animX.ReturnAnimData("Attack0");
                    animX.StartAnimation(curAnimData);
                    break;
                case PlayerStates.ATTACK1:
                    curAnimData = animX.ReturnAnimData("Attack1");
                    animX.StartAnimation(curAnimData);
                    break;
                case PlayerStates.TAKE_DAMAGE:
                    curAnimData = animX.ReturnAnimData("Take Damage");
                    animX.StartAnimation(curAnimData);
                    break;
                case PlayerStates.DIE:
                    curAnimData = animX.ReturnAnimData("Die");
                    animX.StartAnimation(curAnimData);
                    break;
            }
        }

        #region Movement Methods
        private void Move()
        {
            ChangeState(new Vector3(rig.velocity.x, 0, rig.velocity.z).magnitude <= 0f ? PlayerStates.IDLE : PlayerStates.MOVE, false);
            float maxSpeedChange = maxMoveAcceleration * Time.fixedDeltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredMoveVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredMoveVelocity.z, maxSpeedChange);
        }

        private void Jump()
        {
            ChangeState(PlayerStates.JUMP, false);

            velocity += FixedAxisInputs.normalized * jumpForceXZ - velocity; // The player cannot move with input and has stable velocity on XZ axis while in the air.
            velocity.y += JumpSpeedY; // Add force to the player on Y axis for jumping. 
        }

        private void Roll()
        {
            ChangeState(PlayerStates.ROLL, true);
            LockAbilityInputs();

            // StartLeaf Roll Coroutine
            if (abilityCor != null)
                StopCoroutine(abilityCor);
            abilityCor = RollCor();
            StartCoroutine(abilityCor);
        }
        private IEnumerator RollCor()
        {
            rollDirection = desiredRollVelocity.normalized; // Rolling direction calculated with the axis inputs.
            rollVelocity = desiredRollVelocity; // Calculate the initial rolling velocity.
            yield return new WaitUntil(() => animX.CurrentAnim == "Roll Start");
            while (animX.CurrentAnim == "Roll Start")
            {
                velocity = Vector3.zero;
                yield return waitForFixedUpdate;
            }
            yield return new WaitUntil(()=>animX.CurrentAnim == "Roll");
            isHitAble = false;
            while (animX.CurrentAnim == "Roll") // rolling
            {
                velocity = rollVelocity; // The player`s velocity is constant while rolling.
                yield return waitForFixedUpdate;
            }
            isHitAble = true;
            yield return new WaitUntil(() => animX.CurrentAnim == "Roll End");
            while (animX.CurrentAnim == "Roll End") // rolling
            {
                velocity = Vector3.zero; // The player`s velocity is constant while rolling.
                yield return waitForFixedUpdate;
            }
            velocity = Vector3.zero; // Stop the player a little bit time after rolling completed.
            yield return waitForStayStableAfterRollDuration;
            UnlockPlayerState();
            yield return waitForLockedAbilityInputsDuration;
            UnlockAbilityInputs();
        }

        private void Attack(int index)
        {
            switch (index)
            {
                case 1:
                    ChangeState(PlayerStates.ATTACK0, true);
                    break;
                case 2:
                    ChangeState(PlayerStates.ATTACK1, true);
                    break;
            }
            LockAbilityInputs();

            if (abilityCor != null)
                StopCoroutine(abilityCor);
            abilityCor = AttackCor(index);
            StartCoroutine(abilityCor);
        }
        private IEnumerator AttackCor(int index)
        {
            float duration = animX.ReturnAnimData("Attack" + (index - 1).ToString()).duration;
            float percent = 0f;
            while (percent < duration) // attacking
            {
                percent += Time.fixedDeltaTime;
                velocity = Vector3.zero;

                yield return waitForFixedUpdate;
            }
            velocity = Vector3.zero;
            yield return waitForFixedUpdate;

            if (index == InputCtrl.maxAttackInput)
            {
                UnlockPlayerState();
                yield return waitForLockedAbilityInputsDuration;
                UnlockAbilityInputs();
            }
            else
            {
                UnlockPlayerState();
                yield return waitForFixedUpdate;
                UnlockAbilityInputs();
                if (index == InputCtrl.attackInput)
                    InputCtrl.attackInput = 0;
            }
        }

        private void Rotate()
        {
            if (curPlayerState == PlayerStates.ROLL)
            {
                if (rotationCor != null)
                    StopCoroutine(rotationCor);
                rotationCor = RotateCor(rollRotationDuration, rollDirection);
                StartCoroutine(rotationCor);
            }
            else
            {
                var targetRot = Quaternion.LookRotation(forward, Vector3.up); // Calculate target rotation value.
                float fixedMaxRotationAcceleration = maxRotationAcceleration * Quaternion.Angle(rotation, targetRot) / 180f; // Calculate acceleration value for the angle.
                float maxRotationChange = fixedMaxRotationAcceleration * Time.fixedDeltaTime; // Calculate acceleration needed for one frame.
                rotation = Quaternion.RotateTowards(rotation, targetRot, maxRotationChange); // Rotate the player to the target rotation value smoothly.
            }
        }
        private IEnumerator RotateCor(float duration, Vector3 targetDir)
        {
            var startRot = rotation;
            var targetRot = Quaternion.LookRotation(targetDir, Vector3.up); // Calculate target rotation value.
            float fixedDuration = duration * Quaternion.Angle(startRot, targetRot) / 180f; // Calculate the duration value needed to rotate to the target angle.
            float percent = 0f;
            while (percent < fixedDuration)
            {
                rotation = Quaternion.Lerp(startRot, targetRot, percent / fixedDuration); // Rotate the player to the target rotation value.
                percent += Time.deltaTime;
                yield return waitForFixedUpdate;
            }
            rotation = targetRot;
            yield return waitForFixedUpdate;
        }
        #endregion

        #region Environment Methods
        private void SetOnGround()
        {
            Vector3 startPos = rig.position + Vector3.up * 0.2f; // Starting position of the raycast.
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
            Vector3 fixedEnemyPos = new Vector3(EnemyPos.x, rig.position.y, EnemyPos.z); // Enemy`s Position on x and z axis.
            forward = Vector3.Normalize(fixedEnemyPos - rig.position); // Direction from the player`s position to the enemy`s position.
        }
        #endregion

        #region State Control Methods
        private void ChangeState(PlayerStates playerState, bool lockState)
        {
            if (lockedPlayerState) return;

            curPlayerState = playerState;
            if (lockState == true)
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

        private void MoveBlendTreeUpdate()
        {
            if (curPlayerState == PlayerStates.MOVE || curPlayerState == PlayerStates.IDLE)
            {
                float maxInputSpeedChange = maxInputAcceleration * Time.deltaTime;
                animInputX = Mathf.MoveTowards(animInputX, AxisInputs.x, maxInputSpeedChange);
                animInputZ = Mathf.MoveTowards(animInputZ, AxisInputs.z, maxInputSpeedChange);
            }
            else
            {
                animInputX = 0;
                animInputZ = 0;
            }
            animX.anim.SetFloat("InputX", animInputX);
            animX.anim.SetFloat("InputZ", animInputZ);
        }
        #endregion

        #region Take Damage
        public void TakeDamage(int damageValue)
        {
            if (!isHitAble) return;
            //isHitAble = false;

            Health -= damageValue;
            GameManager.Cur.EventCtrl.onPlayerHealthChange?.Invoke(Health, maxHealth);

            if (IsDeath || curPlayerState == PlayerStates.ROLL)
                return;

            ChangeState(PlayerStates.TAKE_DAMAGE, true);
            LockAbilityInputs();

            if (abilityCor != null)
                StopCoroutine(abilityCor);
            abilityCor = TakeDamageCor();
            StartCoroutine(abilityCor);
        }
        private IEnumerator TakeDamageCor()
        {
            float duration = animX.ReturnAnimData("Take Damage").duration;
            float percent = 0f;
            while (percent < duration)
            {
                percent += Time.fixedDeltaTime;
                velocity = Vector3.zero;
                yield return waitForFixedUpdate;
            }
            velocity = Vector3.zero;
            yield return waitForFixedUpdate;

            UnlockPlayerState();
            UnlockAbilityInputs();
            isHitAble = true;
            ChangeState(PlayerStates.IDLE, false);
        }

        public void Die()
        {
            isDieHappened = true;
            if (abilityCor != null)
                StopCoroutine(abilityCor);
            if (rotationCor != null)
                StopCoroutine(rotationCor);
            UnlockPlayerState();
            ChangeState(PlayerStates.DIE, true);
            LockAbilityInputs();
            velocity = Vector3.zero;
            isHitAble = false;

            GameManager.Cur.EventCtrl.onPlayerDie?.Invoke();
        }
        #endregion

        public void IsSwordHitEnable()
        {
            sword.IsHitEnable();
        }
        public void IsSwordHitDisable()
        {
            sword.IsHitDisable();
        }
    }
}
