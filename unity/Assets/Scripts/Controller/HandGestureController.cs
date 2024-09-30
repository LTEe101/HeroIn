using System;
using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 5f;

    // ���� �������� �� ���� ����
    private bool previousLeftFist = false;
    private bool previousRightFist = false;
    private bool previousIsLeftHandOpen = false;
    private bool previousIsRightHandOpen = false;

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
        }
    }

    private void ProcessBothHands(Vector3[] leftHandPositions, Vector3[] rightHandPositions)
    {
        float leftAverageAngle = GetAverageFingerAngle(leftHandPositions);
        float rightAverageAngle = GetAverageFingerAngle(rightHandPositions);

        bool isLeftHandOpen = IsHandOpen(leftAverageAngle);
        bool isRightHandOpen = IsHandOpen(rightAverageAngle);

        // 'Open' �ƴϸ� 'Fist'�� �Ǵ�
        bool leftFist = !isLeftHandOpen;
        bool rightFist = !isRightHandOpen;

        bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

        // ���� ���¿� ���Ͽ� ������ ��쿡�� ������Ʈ
        bool stableLeftFist = (leftFist == previousLeftFist) ? leftFist : previousLeftFist;
        bool stableRightFist = (rightFist == previousRightFist) ? rightFist : previousRightFist;
        bool stableIsLeftHandOpen = (isLeftHandOpen == previousIsLeftHandOpen) ? isLeftHandOpen : previousIsLeftHandOpen;
        bool stableIsRightHandOpen = (isRightHandOpen == previousIsRightHandOpen) ? isRightHandOpen : previousIsRightHandOpen;

        // ���� ���� ������Ʈ
        previousLeftFist = leftFist;
        previousRightFist = rightFist;
        previousIsLeftHandOpen = isLeftHandOpen;
        previousIsRightHandOpen = isRightHandOpen;

        // �¿� �� ���¿� ��� ���� ��� (����׿�)
        // Debug.Log($"Left Hand - State: {(stableIsLeftHandOpen ? "Open" : "Fist")}, Average Angle: {leftAverageAngle:F2} degrees");
        // Debug.Log($"Right Hand - State: {(stableIsRightHandOpen ? "Open" : "Fist")}, Average Angle: {rightAverageAngle:F2} degrees");

        if (IsHandOverlapping(leftHandPositions, rightHandPositions))
        {
            // �� ���� ��ħ, ���� ���� ����
        }
        else if (stableLeftFist && stableRightFist && isOneHandBehind)
        {
            bowAnimator.SetBool("Aiming", true);
        }
        else if (bowAnimator.GetBool("Aiming") && ((stableLeftFist && stableIsRightHandOpen) || (stableRightFist && stableIsLeftHandOpen)))
        {
            bowAnimator.SetBool("Aiming", false);
            shootingManager.TryShoot(); // �߻� ��û
            StartCoroutine(GestureCooldown());
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
        }
    }

    private void ProcessSingleHand(Vector3[] handPositions, string handType)
    {
        float averageAngle = GetAverageFingerAngle(handPositions);

        bool isHandOpen = IsHandOpen(averageAngle);
        bool fist = !isHandOpen;

        // ���� ���¿� ���Ͽ� ������ ��쿡�� ������Ʈ
        bool stableFist = (fist == (handType == "Left" ? previousLeftFist : previousRightFist)) ? fist : (handType == "Left" ? previousLeftFist : previousRightFist);

        // ���� ���� ������Ʈ
        if (handType == "Left")
        {
            previousLeftFist = fist;
            previousIsLeftHandOpen = isHandOpen;
        }
        else
        {
            previousRightFist = fist;
            previousIsRightHandOpen = isHandOpen;
        }

        // �� ���¿� ��� ���� ��� (����׿�)
        // Debug.Log($"{handType} Hand - State: {(isHandOpen ? "Open" : "Fist")}, Average Angle: {averageAngle:F2} degrees");

        if (stableFist)
        {
            bowAnimator.SetBool("Aiming", true);
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
        }
    }

    // �� �հ����� ��� ������ ����ϴ� �Լ� (�����հ��� ����)
    float GetAverageFingerAngle(Vector3[] points)
    {
        if (points == null || points.Length != 21)
            return -1f; // ��ȿ���� ���� ������

        float totalAngle = 0f;
        int angleCount = 0;

        // �� �հ����� ���� �� ���� ���� ��� (�����հ��� ����)
        totalAngle += GetFingerAngle(points, 0, 5, 6);   // ����
        totalAngle += GetFingerAngle(points, 0, 9, 10);  // ����
        totalAngle += GetFingerAngle(points, 0, 13, 14); // ����
        totalAngle += GetFingerAngle(points, 0, 17, 18); // ����
        angleCount += 4;

        totalAngle += GetFingerAngle(points, 5, 6, 7);    // ����
        totalAngle += GetFingerAngle(points, 9, 10, 11);  // ����
        totalAngle += GetFingerAngle(points, 13, 14, 15); // ����
        totalAngle += GetFingerAngle(points, 17, 18, 19); // ����
        angleCount += 4;

        // ��� ���� ���
        float averageAngle = totalAngle / angleCount;

        return averageAngle;
    }

    // �� ���� ����Ʈ�� ���ǵ� �� ���� ������ ������ ����ϴ� �Լ�
    float GetFingerAngle(Vector3[] points, int index1, int index2, int index3)
    {
        Vector3 point1 = points[index1];
        Vector3 point2 = points[index2];
        Vector3 point3 = points[index3];

        Vector3 vector1 = (point2 - point1).normalized;
        Vector3 vector2 = (point3 - point2).normalized;

        float angle = Vector3.Angle(vector1, vector2);

        return angle;
    }

    // ��� ������ ���� ���� ������ �ִ��� Ȯ���ϴ� �Լ�
    bool IsHandOpen(float averageAngle)
    {
        if (averageAngle < 0f)
            return false; // ��ȿ���� ���� ������

        // ���� ��ģ ������ �Ǵ��ϴ� �Ӱ谪 ���� (�ʿ信 ���� ����)
        return averageAngle >= 0f && averageAngle <= 40f;
    }

    // �� ���� ������ �ִ��� Ȯ���ϴ� �Լ�
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

    // �� ���� �ٸ� �� ���� �ڿ� �ִ��� Ȯ���ϴ� �Լ�
    bool IsOneHandBehind(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        Vector3 leftHandCenter = GetHandCenter(leftPoints);
        Vector3 rightHandCenter = GetHandCenter(rightPoints);

        return Mathf.Abs(leftHandCenter.z - rightHandCenter.z) > 0.1f; // �Ӱ谪 ���� ����
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

    private IEnumerator GestureCooldown()
    {
        canDetectGesture = false;
        yield return new WaitForSeconds(GESTURE_COOLDOWN);
        canDetectGesture = true;
    }
}
