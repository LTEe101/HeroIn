using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 5f;

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    void Update()
    {
        DetectHandGesture();
    }

    private void DetectHandGesture()
    {
        if (!canDetectGesture) return;

        HandTracking handTracking = HandTracking.Instance;

        bool isLeftHandValid = handTracking.leftHandDataValid;
        bool isRightHandValid = handTracking.rightHandDataValid;

        Vector3[] leftHandPositions = handTracking.leftHandPositions;
        Vector3[] rightHandPositions = handTracking.rightHandPositions;

        if (isLeftHandValid && isRightHandValid)
        {
            // �� �� ��� �νĵ� ���
            ProcessBothHands(leftHandPositions, rightHandPositions);
        }
        else if (isLeftHandValid || isRightHandValid)
        {
            // �� �ո� �νĵ� ���
            ProcessSingleHand(isLeftHandValid ? leftHandPositions : rightHandPositions, isLeftHandValid ? "Left" : "Right");
        }
        else
        {
            // �� �����Ͱ� ��� ��ȿ���� ����
            bowAnimator.SetBool("Aiming", false);
            Debug.LogWarning("�� �����Ͱ� ��ȿ���� �ʽ��ϴ�.");
        }
    }

    private void ProcessBothHands(Vector3[] leftHandPositions, Vector3[] rightHandPositions)
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
            Debug.Log("�� �� ������, ���� ���� ����");
        }
        // Aiming (�� �հ����� ����� ���¿��� �յڷ� ��ġ)
        else if ((leftFist || leftGathered) && (rightFist || rightGathered) && isOneHandBehind)
        {
            Debug.Log("�� �հ����� ����� ���¿��� �յڷ� ��ġ, ����");
            bowAnimator.SetBool("Aiming", true);
        }
        // Shooting (�� ���� �ָ� ��� �� ���� ��ġ�� �߻� ��û)
        else if (bowAnimator.GetBool("Aiming") && ((leftFist && isRightHandOpen) || (rightFist && isLeftHandOpen)))
        {
            Debug.Log("�� �� �ָ� �� �� ��ħ, �߻� ��û");
            bowAnimator.SetBool("Aiming", false);
            shootingManager.TryShoot(); // ShootingManager�� �߻� ��û
            StartCoroutine(GestureCooldown());
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("�� �� �ν�, ��� ����");
        }
    }

    private void ProcessSingleHand(Vector3[] handPositions, string handType)
    {
        bool fist = IsFistClosed(handPositions);
        bool gathered = IsFingersGathered(handPositions);

        // Aiming (�ָ� �� �� �Ǵ� �հ����� ����� ��)
        if (fist || gathered)
        {
            Debug.Log($"{handType} �� �ָ� �Ǵ� �հ��� ���� �ν�, ����");
            bowAnimator.SetBool("Aiming", true);
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log($"{handType} �� �ν�, ��� ����");
        }
    }

    // �Ʒ����ʹ� ���� �޼���� (���� ���� ����)
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

    // ���� ����� ���� Ȯ�� (�ָ� �� ����ó)
    bool IsFistClosed(Vector3[] points)
    {
        // ����Ʈ�� ��ȿ���� Ȯ��
        if (points == null || points.Length != 21 || points[0] == Vector3.zero)
            return false;

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

    // �հ��� ������ ���ִ��� Ȯ�� (��Ż���� ����ó)
    bool IsFingersGathered(Vector3[] points)
    {
        // ����Ʈ�� ��ȿ���� Ȯ��
        if (points == null || points.Length != 21 || points[0] == Vector3.zero)
            return false;

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
        // ����Ʈ�� ��ȿ���� Ȯ��
        if (points == null || points.Length != 21 || points[0] == Vector3.zero)
            return false;

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

    private IEnumerator GestureCooldown()
    {
        Debug.Log("�߻� �� ��� ����");
        canDetectGesture = false;
        yield return new WaitForSeconds(GESTURE_COOLDOWN);
        canDetectGesture = true;
        Debug.Log("�߻� �� ��� ��");
    }
}
