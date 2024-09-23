using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public Animator bowAnimator;  // Ȱ �ִϸ��̼��� ���� Animator
    private bool canShoot = true; // Ȱ�� �� �� �ִ��� ���θ� �����ϴ� �÷���
    private float shootCooldownTime = 3.0f; // Ȱ�� ��� �ٽ� �� �� �ְ� �Ǵ� ��� �ð�
    private float shootTimer = 0f; // ��Ÿ�� Ÿ�̸�

    void Start()
    {
        bowAnimator.SetBool("Aiming", false);
    }

    void Update()
    {
        // ���簡 �Ұ����ϵ��� Ÿ�̸� ����
        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootCooldownTime)
            {
                canShoot = true;
                shootTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }

        // HandTracking �ν��Ͻ����� ��ǥ ������ ��������
        Vector3[] leftHandPositions = HandTracking.Instance.leftHandPositions;
        Vector3[] rightHandPositions = HandTracking.Instance.rightHandPositions;

        // �� �� ��� �νĵ� ���
        if (leftHandPositions.Length == 21 && rightHandPositions.Length == 21)
        {
            bool leftFist = IsFistClosed(leftHandPositions);
            bool rightFist = IsFistClosed(rightHandPositions);
            bool handsOverlapping = IsHandOverlapping(leftHandPositions, rightHandPositions);
            bool isLeftHandOpen = IsHandOpen(leftHandPositions);
            bool isRightHandOpen = IsHandOpen(rightHandPositions);
            bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

            // Aiming (�� ���� �ָ��� �� ���¿��� �յڷ� ������)
            if (canShoot && leftFist && rightFist && handsOverlapping)
            {
                Debug.Log("Ready to aim");
                bowAnimator.SetBool("Aiming", true);
            }
            // Shooting (���� ���� ���� ���)
            else if (bowAnimator.GetBool("Aiming") && leftFist && (isLeftHandOpen || isRightHandOpen) && canShoot)
            {
                Debug.Log("Shot the arrow!");
                bowAnimator.SetBool("Aiming", false);
                bowAnimator.SetTrigger("Shoot");
                canShoot = false;  // �� �� ��� ���� �ٽ� �� �� ���� ��
            }
        }
        // �� �ո� �νĵ� ��� (�� ���� �νĵ��� ������ �⺻ ���·� ����)
        else if (leftHandPositions.Length == 21 || rightHandPositions.Length == 21)
        {
            bool leftFist = leftHandPositions.Length == 21 && IsFistClosed(leftHandPositions);
            bool rightFist = rightHandPositions.Length == 21 && IsFistClosed(rightHandPositions);

            // Aiming (�ʹ� �������� �ָ��� �� �ո� �νĵ� ���)
            if (leftFist || rightFist)
            {
                Debug.Log("One hand is making a fist. Ready to aim.");
                bowAnimator.SetBool("Aiming", true);
            }
            else
            {
                bowAnimator.SetBool("Aiming", false);
            }
        }
    }

    // �ָ� �� ���� Ȯ��
    bool IsFistClosed(Vector3[] points)
    {
        // ù ��° ����: �հ��� ���� �������� �Ÿ� ���
        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) > 0.5f) // �Ÿ��� �������� �Ǵ� (���� ���� �ʿ�)
            {
                return false;
            }
        }

        // �� ��° ����: �հ��� �������� �Ÿ��� ������ Ȯ��
        for (int i = 0; i < 3; i++)
        {
            Vector3 fingerTip1 = points[8 + (i * 4)];
            Vector3 fingerTip2 = points[8 + ((i + 1) * 4)];

            if (Vector3.Distance(fingerTip1, fingerTip2) > 0.3f) // �հ��� �������� �Ÿ�
            {
                return false;
            }
        }

        return true;
    }

    // ���� �յڷ� ���������� Ȯ��
    bool IsHandOverlapping(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        Vector3 leftHandMin = GetHandMin(leftPoints);
        Vector3 leftHandMax = GetHandMax(leftPoints);
        Vector3 rightHandMin = GetHandMin(rightPoints);
        Vector3 rightHandMax = GetHandMax(rightPoints);

        bool xOverlap = rightHandMin.x < leftHandMax.x && rightHandMax.x > leftHandMin.x;
        bool yOverlap = rightHandMin.y < leftHandMax.y && rightHandMax.y > leftHandMin.y;
        bool zOverlap = rightHandMin.z < leftHandMax.z && rightHandMax.z > leftHandMin.z;

        return xOverlap && yOverlap && zOverlap;
    }

    // �� ���� �ٸ� �� ���� �ڿ� �ִ��� Ȯ�� (�������� ����)
    bool IsOneHandBehind(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        Vector3 leftHandCenter = GetHandCenter(leftPoints);
        Vector3 rightHandCenter = GetHandCenter(rightPoints);

        return leftHandCenter.z > rightHandCenter.z || rightHandCenter.z > leftHandCenter.z;
    }

    Vector3 GetHandMin(Vector3[] points)
    {
        Vector3 min = points[0];
        foreach (var point in points)
        {
            min = Vector3.Min(min, point);
        }
        return min;
    }

    Vector3 GetHandMax(Vector3[] points)
    {
        Vector3 max = points[0];
        foreach (var point in points)
        {
            max = Vector3.Max(max, point);
        }
        return max;
    }

    Vector3 GetHandCenter(Vector3[] points)
    {
        Vector3 sum = Vector3.zero;
        foreach (var point in points)
        {
            sum += point;
        }
        return sum / points.Length;
    }

    // ���� �������� Ȯ��
    bool IsHandOpen(Vector3[] points)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) < 0.4f) // �Ÿ� ���� ����
            {
                return false;
            }
        }
        return true;
    }
}
