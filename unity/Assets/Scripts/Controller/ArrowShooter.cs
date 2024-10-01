using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowShooter : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject existingArrow;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 1f; // 속도를 더 낮춤
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private float arrowLifetime = 1.3f;

    [SerializeField] private Vector3 neckOffset = new Vector3(0, 1.5f, 0); // 목 위치 오프셋

    // 회전 오프셋 추가 (필요에 따라 조정)
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // 예: new Vector3(0, 90, 0)

    private Rigidbody arrowRb;
    private LineRenderer trajectoryLine;
    private Transform currentTarget;
    private Vector3 lastTargetPosition;

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    void Start()
    {
        SetupArrow();
        SetupTrajectoryLine();
    }

    void Update()
    {
        currentTarget = FindClosestTarget();

        if (currentTarget != null)
        {
            // 적의 목 위치를 계산하기 위해 오프셋을 추가
            lastTargetPosition = currentTarget.position + neckOffset;
            Debug.DrawLine(firePoint.position, lastTargetPosition, Color.red);
        }
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
        }
        else
        {
            Debug.LogError("Existing arrow not assigned in the inspector!");
        }
    }

    private void SetupTrajectoryLine()
    {
        trajectoryLine = gameObject.AddComponent<LineRenderer>();
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = Color.blue;
        trajectoryLine.endColor = Color.blue;
    }

    public void Shoot()
    {
        if (existingArrow == null || arrowRb == null)
        {
            Debug.LogError("Arrow or its Rigidbody is missing!");
            return;
        }

        existingArrow.transform.position = firePoint.position;

        // 화살의 초기 방향을 설정
        Vector3 targetPosition = lastTargetPosition;
        Vector3 direction = (targetPosition - firePoint.position).normalized;

        // 회전 오프셋을 적용하여 화살의 tip이 목표를 향하도록 설정
        Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
        existingArrow.transform.rotation = lookRotation;

        // 물리 시뮬레이션을 비활성화
        arrowRb.isKinematic = true;
        arrowRb.useGravity = false;

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("Shoot");
        }

        // 등속 운동으로 화살 발사
        StartCoroutine(MoveArrow(targetPosition));

        Debug.Log($"Arrow fired. Direction: {direction}, Target: {targetPosition}");
    }

    private IEnumerator MoveArrow(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float totalTime = 0.8f; // 화살이 목표에 도달하는 데 걸리는 총 시간
        Vector3 startPosition = firePoint.position;

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            existingArrow.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 이동 중 회전 업데이트를 제거하여 비정상적인 회전 방지
            // 필요 시 단 한 번만 회전 설정
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 목표 지점에 도달하면 화살 멈춤
        existingArrow.transform.position = targetPosition;

        yield return new WaitForSeconds(arrowLifetime);
        ResetArrow();
        trajectoryLine.positionCount = 0; // 궤적 초기화
    }

    private void ResetArrow()
    {
        existingArrow.transform.position = firePoint.position;

        // Reset rotation based on firePoint's forward direction with rotation offset
        Vector3 initialDirection = firePoint.forward;
        Quaternion resetRotation = Quaternion.LookRotation(initialDirection) * Quaternion.Euler(rotationOffset);
        existingArrow.transform.rotation = resetRotation;

        arrowRb.velocity = Vector3.zero;
        arrowRb.isKinematic = true;
    }

    private Transform FindClosestTarget()
    {
        return EnemyManager.Instance.GetNearestEnemy(transform.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, lastTargetPosition);
            Gizmos.DrawSphere(lastTargetPosition, 0.5f);
        }
    }
}
