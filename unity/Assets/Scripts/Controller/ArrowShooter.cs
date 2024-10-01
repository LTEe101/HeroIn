using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowShooter : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject existingArrow;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 1f; // �ӵ��� �� ����
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private float arrowLifetime = 1.3f;

    [SerializeField] private Vector3 neckOffset = new Vector3(0, 1.5f, 0); // �� ��ġ ������

    // ȸ�� ������ �߰� (�ʿ信 ���� ����)
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // ��: new Vector3(0, 90, 0)

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
            // ���� �� ��ġ�� ����ϱ� ���� �������� �߰�
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

        // ȭ���� �ʱ� ������ ����
        Vector3 targetPosition = lastTargetPosition;
        Vector3 direction = (targetPosition - firePoint.position).normalized;

        // ȸ�� �������� �����Ͽ� ȭ���� tip�� ��ǥ�� ���ϵ��� ����
        Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
        existingArrow.transform.rotation = lookRotation;

        // ���� �ùķ��̼��� ��Ȱ��ȭ
        arrowRb.isKinematic = true;
        arrowRb.useGravity = false;

        if (bowAnimator != null)
        {
            bowAnimator.SetTrigger("Shoot");
        }

        // ��� ����� ȭ�� �߻�
        StartCoroutine(MoveArrow(targetPosition));

        Debug.Log($"Arrow fired. Direction: {direction}, Target: {targetPosition}");
    }

    private IEnumerator MoveArrow(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float totalTime = 0.8f; // ȭ���� ��ǥ�� �����ϴ� �� �ɸ��� �� �ð�
        Vector3 startPosition = firePoint.position;

        while (elapsedTime < totalTime)
        {
            float t = elapsedTime / totalTime;
            existingArrow.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // �̵� �� ȸ�� ������Ʈ�� �����Ͽ� ���������� ȸ�� ����
            // �ʿ� �� �� �� ���� ȸ�� ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ǥ ������ �����ϸ� ȭ�� ����
        existingArrow.transform.position = targetPosition;

        yield return new WaitForSeconds(arrowLifetime);
        ResetArrow();
        trajectoryLine.positionCount = 0; // ���� �ʱ�ȭ
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
