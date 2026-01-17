using UnityEngine;
using static Enums;

public class CombatAnimator : MonoBehaviour
{
    [Header("Combatant Transforms")]
    [Tooltip("The transform of the player's visual GameObject")]
    public Transform playerVisual;
    [Tooltip("The transform of the enemy's visual GameObject")]
    public Transform enemyVisual;

    [Header("Attack Animation Settings")]
    [Tooltip("Tilt angle when attacking (leaning forward)")]
    public float attackTiltAngle = 15f;
    [Tooltip("Distance to move forward during attack")]
    public float attackMoveDistance = 0.5f;
    [Tooltip("Duration of the forward tilt")]
    public float attackTiltDuration = 0.08f;
    [Tooltip("Duration of the forward movement")]
    public float attackMoveDuration = 0.1f;
    [Tooltip("Duration of the backward movement")]
    public float attackReturnMoveDuration = 0.1f;
    [Tooltip("Duration of the return to normal rotation")]
    public float attackReturnTiltDuration = 0.1f;

    [Header("Death Animation Settings")]
    [Tooltip("Number of full rotations on death")]
    public float deathSpinRotations = 2f;
    [Tooltip("Duration of the spin")]
    public float deathSpinDuration = 0.8f;
    [Tooltip("Duration of the shrink")]
    public float deathShrinkDuration = 0.5f;
    [Tooltip("Final scale after shrinking")]
    public float deathFinalScale = 0f;

    private Vector3 playerOriginalPosition;
    private Vector3 enemyOriginalPosition;
    private Vector3 playerOriginalRotation;
    private Vector3 enemyOriginalRotation;
    private Vector3 playerOriginalScale;
    private Vector3 enemyOriginalScale;

    // Animation state
    private enum AnimState { Idle, AttackTilt, AttackMove, AttackReturnMove, AttackReturnTilt, DeathSpin, DeathShrink }

    private AnimState playerState = AnimState.Idle;
    private AnimState enemyState = AnimState.Idle;

    private float playerTimer;
    private float enemyTimer;

    private void Start()
    {
        // Save original positions, rotations and scales
        if (playerVisual != null)
        {
            playerOriginalPosition = playerVisual.localPosition;
            playerOriginalRotation = playerVisual.localEulerAngles;
            playerOriginalScale = playerVisual.localScale;
        }
        if (enemyVisual != null)
        {
            enemyOriginalPosition = enemyVisual.localPosition;
            enemyOriginalRotation = enemyVisual.localEulerAngles;
            enemyOriginalScale = enemyVisual.localScale;
        }

        // Subscribe to CombatManager events
        CombatManager combat = CombatManager.Instance;
        if (combat != null)
        {
            combat.OnCombatStarted += OnCombatStarted;
            combat.OnPlayerAttack += OnPlayerAttack;
            combat.OnEnemyAttack += OnEnemyAttack;
            combat.OnCombatEnded += OnCombatEnded;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.OnCombatStarted -= OnCombatStarted;
            CombatManager.Instance.OnPlayerAttack -= OnPlayerAttack;
            CombatManager.Instance.OnEnemyAttack -= OnEnemyAttack;
            CombatManager.Instance.OnCombatEnded -= OnCombatEnded;
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        // Update player animation
        if (playerVisual != null && playerState != AnimState.Idle)
        {
            playerTimer += deltaTime;
            UpdateAnimation(playerVisual, playerOriginalPosition, playerOriginalRotation, playerOriginalScale, true, ref playerState, ref playerTimer);
        }

        // Update enemy animation
        if (enemyVisual != null && enemyState != AnimState.Idle)
        {
            enemyTimer += deltaTime;
            UpdateAnimation(enemyVisual, enemyOriginalPosition, enemyOriginalRotation, enemyOriginalScale, false, ref enemyState, ref enemyTimer);
        }
    }

    private void UpdateAnimation(Transform target, Vector3 originalPosition, Vector3 originalRotation, Vector3 originalScale, bool isPlayer, ref AnimState state, ref float timer)
    {
        float moveDirection = isPlayer ? 1f : -1f;
        float tiltDirection = isPlayer ? -1f : 1f;

        Vector3 tiltedRotation = originalRotation + new Vector3(0f, 0f, attackTiltAngle * tiltDirection);
        Vector3 forwardPosition = originalPosition + new Vector3(attackMoveDistance * moveDirection, 0f, 0f);

        switch (state)
        {
            case AnimState.AttackTilt:
                {
                    float t = Mathf.Clamp01(timer / attackTiltDuration);
                    t = 1f - Mathf.Pow(1f - t, 2f); // Ease-out
                    target.localEulerAngles = Vector3.Lerp(originalRotation, tiltedRotation, t);

                    if (timer >= attackTiltDuration)
                    {
                        target.localEulerAngles = tiltedRotation;
                        state = AnimState.AttackMove;
                        timer = 0f;
                    }
                }
                break;

            case AnimState.AttackMove:
                {
                    float t = Mathf.Clamp01(timer / attackMoveDuration);
                    t = 1f - Mathf.Pow(1f - t, 2f); // Ease-out
                    target.localPosition = Vector3.Lerp(originalPosition, forwardPosition, t);

                    if (timer >= attackMoveDuration)
                    {
                        target.localPosition = forwardPosition;
                        state = AnimState.AttackReturnMove;
                        timer = 0f;
                    }
                }
                break;

            case AnimState.AttackReturnMove:
                {
                    float t = Mathf.Clamp01(timer / attackReturnMoveDuration);
                    t = t * t; // Ease-in
                    target.localPosition = Vector3.Lerp(forwardPosition, originalPosition, t);

                    if (timer >= attackReturnMoveDuration)
                    {
                        target.localPosition = originalPosition;
                        state = AnimState.AttackReturnTilt;
                        timer = 0f;
                    }
                }
                break;

            case AnimState.AttackReturnTilt:
                {
                    float t = Mathf.Clamp01(timer / attackReturnTiltDuration);
                    t = t * t * (3f - 2f * t); // Ease-in-out
                    target.localEulerAngles = Vector3.Lerp(tiltedRotation, originalRotation, t);

                    if (timer >= attackReturnTiltDuration)
                    {
                        target.localEulerAngles = originalRotation;
                        state = AnimState.Idle;
                        timer = 0f;
                    }
                }
                break;

            case AnimState.DeathSpin:
                {
                    float t = Mathf.Clamp01(timer / deathSpinDuration);
                    float easedT = 1f - Mathf.Pow(1f - t, 2f); // Ease-out
                    float totalRotation = 360f * deathSpinRotations;
                    float currentRotation = Mathf.Lerp(0f, totalRotation, easedT);
                    target.localEulerAngles = originalRotation + new Vector3(0f, currentRotation, 0f);

                    if (timer >= deathSpinDuration)
                    {
                        state = AnimState.DeathShrink;
                        timer = 0f;
                    }
                }
                break;

            case AnimState.DeathShrink:
                {
                    float t = Mathf.Clamp01(timer / deathShrinkDuration);
                    t = t * t; // Ease-in
                    Vector3 finalScale = Vector3.one * deathFinalScale;
                    target.localScale = Vector3.Lerp(originalScale, finalScale, t);

                    if (timer >= deathShrinkDuration)
                    {
                        target.localScale = finalScale;
                        state = AnimState.Idle;
                        timer = 0f;
                    }
                }
                break;
        }
    }

    private void OnCombatStarted(ContractData contract)
    {
        ResetTransforms();
        playerState = AnimState.Idle;
        enemyState = AnimState.Idle;
        playerTimer = 0f;
        enemyTimer = 0f;
    }

    private void OnPlayerAttack(int damage)
    {
        if (playerVisual != null)
        {
            playerState = AnimState.AttackTilt;
            playerTimer = 0f;
        }
    }

    private void OnEnemyAttack(int damage)
    {
        if (enemyVisual != null)
        {
            enemyState = AnimState.AttackTilt;
            enemyTimer = 0f;
        }
    }

    private void OnCombatEnded(CombatState state, int reward)
    {
        if (state == CombatState.VICTORY && enemyVisual != null)
        {
            enemyState = AnimState.DeathSpin;
            enemyTimer = 0f;
        }
        else if (state == CombatState.DEFEAT && playerVisual != null)
        {
            playerState = AnimState.DeathSpin;
            playerTimer = 0f;
        }
    }

    public void ResetTransforms()
    {
        if (playerVisual != null)
        {
            playerVisual.localPosition = playerOriginalPosition;
            playerVisual.localEulerAngles = playerOriginalRotation;
            playerVisual.localScale = playerOriginalScale;
        }
        if (enemyVisual != null)
        {
            enemyVisual.localPosition = enemyOriginalPosition;
            enemyVisual.localEulerAngles = enemyOriginalRotation;
            enemyVisual.localScale = enemyOriginalScale;
        }
    }
}
