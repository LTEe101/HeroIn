using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    [SerializeField] private Animator bowAnimator; 
    [SerializeField] private ShootingManager shootingManager; 

    void Update()
    {
        DetectHandGesture();
    }

    private void DetectHandGesture()
    {
        Vector3[] leftHandPositions = HandTracking.Instance.leftHandPositions;
        Vector3[] rightHandPositions = HandTracking.Instance.rightHandPositions;

        // �� �� ��� �νĵ� ���
        if (leftHandPositions.Length == 21 && rightHandPositions.Length == 21)
        {
            bool leftFist = IsFistClosed(leftHandPositions);
            bool rightFist = IsFistClosed(rightHandPositions);
            bool leftGathered = IsFingersGathered(leftHandPositions);
            bool rightGathered = IsFingersGathered(rightHandPositions);
            bool isLeftHandOpen = IsHandOpen(leftHandPositions);
            bool isRightHandOpen = IsHandOpen(rightHandPositions);
            bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

            // �� �� ���� ���� (�� ���� �������� ��ǥ �ν��� ����� �� �Ǵ� ���)
            if (IsHandOverlapping(leftHandPositions, rightHandPositions))
            {
            }
            // Aiming (�� �հ����� ����� ���¿��� �յڷ� ��ġ)
            else if ((leftFist || leftGathered) && (rightFist || rightGathered) && isOneHandBehind)
            {
                Debug.Log("Ready to aim");
                bowAnimator.SetBool("Aiming", true);
            }
            // Shooting (�� ���� �ָ� ��� �� ���� ��ġ�� �߻� ��û)
            else if (bowAnimator.GetBool("Aiming") && ((leftFist && isRightHandOpen) || (rightFist && isLeftHandOpen)))
            {
                Debug.Log("Request to shoot");
                bowAnimator.SetBool("Aiming", false);
                shootingManager.TryShoot(); // ShootingManager�� �߻� ��û
            }
        }
        // �� �ո� �νĵ� ��� (�� ���� �νĵ��� ������ �⺻ ���·� ����) �� �� �� ����ī��
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

    // ���� ����� ���� Ȯ��
    // ù ��° ����: �հ��� ���� �������� �Ÿ� ��� (�ָ� �� ����ó)
    bool IsFistClosed(Vector3[] points)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) > 0.5f) 
            {
                return false;
            }
        }

        return true;

    }

    // �� ��° ����: �հ��� ������ �߽� �Ÿ� ��� (��Ż���� ����ó)
    bool IsFingersGathered(Vector3[] points)
    {
        int[] fingerTipIndices = { 4, 8, 12, 16, 20 };

        Vector3 middleTip = points[12]; 
        float threshold = 0.5f; 

        foreach (int index in fingerTipIndices)
        {
            if (Vector3.Distance(points[index], middleTip) >= threshold)
            {
                return false;
            }
        }

        return true;
    }


    // �� ���� �ٸ� �� ���� �ڿ� �ִ��� Ȯ��
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

            if (Vector3.Distance(fingerTip, fingerBase) < 0.5f) // �Ÿ� ���� ����
            {
                return false;
            }
        }
        return true;
    }
}