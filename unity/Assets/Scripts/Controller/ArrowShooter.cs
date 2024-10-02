using UnityEngine;
using System.Collections;

public class ArrowShooter : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject existingArrow;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 25f; // 화살 속도 증가
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private float arrowLifetime = 0.5f;

    [SerializeField] private Vector3 neckOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    private Rigidbody arrowRb;
    private Transform currentTarget;
    private Vector3 lastTargetPosition;

    private Coroutine moveArrowCoroutine;
    private bool isArrowActive = false; // 화살이 발사 중인지 여부 추적

    private Vector3 initialArrowPosition;
    private Quaternion initialArrowRotation;

    private Vector3 AimingArrowPosition = new Vector3(-386.18f, 15.04f, -18.68f);
    private Quaternion AimingArrowRotation = new Quaternion(0.52234f, 0.45622f, -0.54248f, -0.47407f);

    [SerializeField] private float positionLerpSpeed = 5f;
    [SerializeField] private float rotationLerpSpeed = 5f;

    private bool isInitialSetupComplete = false;

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    void Start()
    {
        SetupArrow();

        if (existingArrow != null)
        {
            initialArrowPosition = existingArrow.transform.position;
            initialArrowRotation = existingArrow.transform.rotation;
        }
    }

    void Update()
    {
        // 화살이 발사 중이면 애니메이션 상태를 확인하지 않고 바로 리턴
        if (isArrowActive)
        {
            return;
        }

        if (!isArrowActive)
        {
            currentTarget = FindClosestTarget();

            if (currentTarget != null)
            {
                lastTargetPosition = currentTarget.position + neckOffset;
                // 빨간 선 관련 코드 제거됨
            }
        }

        AnimatorStateInfo stateInfo = bowAnimator.GetCurrentAnimatorStateInfo(0);

        // 부드러운 초기 위치 세팅을 진행
        if (!isInitialSetupComplete)
        {
            existingArrow.transform.position = Vector3.Lerp(
                existingArrow.transform.position,
                initialArrowPosition,
                Time.deltaTime * positionLerpSpeed
            );

            existingArrow.transform.rotation = Quaternion.Slerp(
                existingArrow.transform.rotation,
                initialArrowRotation,
                Time.deltaTime * rotationLerpSpeed
            );

            if (Vector3.Distance(existingArrow.transform.position, initialArrowPosition) < 0.01f &&
                Quaternion.Angle(existingArrow.transform.rotation, initialArrowRotation) < 1f)
            {
                isInitialSetupComplete = true;
            }
        }

        // Aiming 상태일 때 부드럽게 이동
        if (stateInfo.IsName("Aiming") && isInitialSetupComplete)
        {
            existingArrow.transform.position = Vector3.Lerp(
                existingArrow.transform.position,
                AimingArrowPosition,
                Time.deltaTime * positionLerpSpeed
            );

            existingArrow.transform.rotation = Quaternion.Slerp(
                existingArrow.transform.rotation,
                AimingArrowRotation,
                Time.deltaTime * rotationLerpSpeed
            );
        }
        // Idle 상태로 돌아갈 때 초기 위치로 부드럽게 이동
        else if (stateInfo.IsName("Idle_Battle_BowAndArrow") && isInitialSetupComplete)
        {
            existingArrow.transform.position = Vector3.Lerp(
                existingArrow.transform.position,
                initialArrowPosition,
                Time.deltaTime * positionLerpSpeed
            );

            existingArrow.transform.rotation = Quaternion.Slerp(
                existingArrow.transform.rotation,
                initialArrowRotation,
                Time.deltaTime * rotationLerpSpeed
            );
        }
    }

    public void Shoot()
    {
        if (existingArrow == null || arrowRb == null)
        {
            Debug.LogError("Arrow or its Rigidbody is missing!");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogWarning("No target to shoot at!");
            return;
        }

        existingArrow.transform.parent = null;

        float spawnOffsetDistance = 0.2f;
        Vector3 spawnPosition = firePoint.position + firePoint.forward * spawnOffsetDistance;
        existingArrow.transform.position = spawnPosition;

        Vector3 targetPosition = lastTargetPosition;
        Vector3 direction = (targetPosition - spawnPosition).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
        existingArrow.transform.rotation = lookRotation;

        arrowRb.isKinematic = false;
        arrowRb.useGravity = false; // 중력은 끄고 일직선으로 발사
        arrowRb.velocity = direction * arrowSpeed;

        isArrowActive = true; // 발사 중 상태로 설정

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("Shoot");
        }

        if (moveArrowCoroutine != null)
        {
            StopCoroutine(moveArrowCoroutine);
        }

        moveArrowCoroutine = StartCoroutine(DeactivateArrowAfterLifetime());

        Debug.Log($"Arrow fired. Direction: {direction}, Target: {targetPosition}");
    }

    private IEnumerator DeactivateArrowAfterLifetime()
    {
        yield return new WaitForSeconds(arrowLifetime);
        ResetArrow();
    }

    private void ResetArrow()
    {
        if (existingArrow == null || arrowRb == null)
            return;

        Debug.Log("Resetting arrow to fire point.");

        if (moveArrowCoroutine != null)
        {
            StopCoroutine(moveArrowCoroutine);
            moveArrowCoroutine = null;
        }

        existingArrow.transform.parent = firePoint;

        arrowRb.velocity = Vector3.zero;
        arrowRb.angularVelocity = Vector3.zero;
        arrowRb.isKinematic = true;
        arrowRb.useGravity = false;

        existingArrow.transform.position = initialArrowPosition;
        existingArrow.transform.rotation = initialArrowRotation;

        // 스케일 초기화 (필요 시)
        existingArrow.transform.localScale = Vector3.one * 2;

        // trajectoryLine.positionCount = 0; // 제거됨

        isArrowActive = false; // 발사 상태 해제

        Debug.Log("Arrow has been reset to fire point.");
    }

    private void SetupArrow()
    {
        if (existingArrow != null)
        {
            arrowRb = existingArrow.GetComponent<Rigidbody>();
            if (arrowRb == null)
            {
                Debug.LogError("No Rigidbody found on the arrow!");
            }
            else
            {
                arrowRb.isKinematic = true;
                arrowRb.useGravity = false;
            }

            Collider arrowCollider = existingArrow.GetComponent<Collider>();
            if (arrowCollider != null)
            {
                PhysicMaterial noBounceMaterial = Resources.Load<PhysicMaterial>("NoBounce");
                if (noBounceMaterial != null)
                {
                    arrowCollider.material = noBounceMaterial;
                }
                else
                {
                    Debug.LogError("NoBounce PhysicMaterial not found in Resources!");
                }

                arrowCollider.isTrigger = false;
            }
            else
            {
                Debug.LogError("No Collider found on the arrow!");
            }
        }
        else
        {
            Debug.LogError("Existing arrow not assigned in the inspector!");
        }
    }

    private Transform FindClosestTarget()
    {
        return EnemyManager.Instance.GetNearestEnemy(transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isArrowActive)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Arrow hit the enemy!");

            if (moveArrowCoroutine != null)
            {
                StopCoroutine(moveArrowCoroutine);
                moveArrowCoroutine = null;
            }

            existingArrow.transform.position = lastTargetPosition;
            existingArrow.transform.parent = collision.transform;

            arrowRb.isKinematic = true;
            arrowRb.velocity = Vector3.zero;
            arrowRb.angularVelocity = Vector3.zero;
            arrowRb.useGravity = false;

            isArrowActive = false;

            // trajectoryLine.positionCount = 0; // 제거됨
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            // Gizmos.DrawLine(firePoint.position, lastTargetPosition); // 제거됨
            // Gizmos.DrawSphere(lastTargetPosition, 0.5f); // 제거됨
        }
    }
}
