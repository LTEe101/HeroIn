using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 5f;

    // �ֱ� 3�������� �� ���¸� �����ϱ� ���� ť
    private Queue<string> leftHandStateHistory = new Queue<string>(3);
    private Queue<string> rightHandStateHistory = new Queue<string>(3);

    // ���� stable ���¸� ����
    private string previousLeftHandStableState = "Open";
    private string previousRightHandStableState = "Open";

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
            ProcessBothHands(leftHandPositions, rightHandPositions);
        }
        //else if (isLeftHandValid)
        //{
        //    ProcessSingleHand(leftHandPositions, "Left");
        //}
        //else if (isRightHandValid)
        //{
        //    ProcessSingleHand(rightHandPositions, "Right");
        //}
        else
        {
            bowAnimator.SetBool("Aiming", false);
        }
    }

    private void ProcessBothHands(Vector3[] leftHandPositions, Vector3[] rightHandPositions)
    {
        float leftAverageAngle = GetAverageFingerAngle(leftHandPositions);
        float rightAverageAngle = GetAverageFingerAngle(rightHandPositions);

        string currentLeftHandState = GetHandState(leftAverageAngle);
        string currentRightHandState = GetHandState(rightAverageAngle);

        UpdateHandStateHistory(leftHandStateHistory, currentLeftHandState);
        UpdateHandStateHistory(rightHandStateHistory, currentRightHandState);

        string stableLeftHandState = GetStableHandState(leftHandStateHistory, previousLeftHandStableState);
        string stableRightHandState = GetStableHandState(rightHandStateHistory, previousRightHandStableState);

        // ���� stable ���¸� ������Ʈ
        previousLeftHandStableState = stableLeftHandState;
        previousRightHandStableState = stableRightHandState;

        Debug.Log($"Left Hand - Average Angle: {leftAverageAngle:F2} degrees - Stable State: {stableLeftHandState}");
        Debug.Log($"Right Hand - Average Angle: {rightAverageAngle:F2} degrees - Stable State: {stableRightHandState}");

        bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

        if (stableLeftHandState == "Fist" && stableRightHandState == "Fist" && isOneHandBehind)
        {
            bowAnimator.SetBool("Aiming", true);
            Debug.Log("����");
        }
        else if (bowAnimator.GetBool("Aiming") && ((stableLeftHandState == "Fist" && stableRightHandState == "Open") || (stableLeftHandState == "Open" && stableRightHandState == "Fist")))
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("�߻�");
            shootingManager.TryShoot(); // �߻� ��û
            StartCoroutine(GestureCooldown());
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("�׳� �־�");
        }
    }

    private void ProcessSingleHand(Vector3[] handPositions, string handType)
    {
        float averageAngle = GetAverageFingerAngle(handPositions);
        string currentHandState = GetHandState(averageAngle);

        if (handType == "Left")
        {
            UpdateHandStateHistory(leftHandStateHistory, currentHandState);
            string stableLeftHandState = GetStableHandState(leftHandStateHistory, previousLeftHandStableState);
            previousLeftHandStableState = stableLeftHandState;

            //Debug.Log($"Left Hand - Average Angle: {averageAngle:F2} degrees - Stable State: {stableLeftHandState}");

            if (stableLeftHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
                Debug.Log("����");
            }
            else if (stableLeftHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    Debug.Log("�߻�");
                    shootingManager.TryShoot(); // �߻� ��û
                    StartCoroutine(GestureCooldown());
                }
            }
        }
        else if (handType == "Right")
        {
            UpdateHandStateHistory(rightHandStateHistory, currentHandState);
            string stableRightHandState = GetStableHandState(rightHandStateHistory, previousRightHandStableState);
            previousRightHandStableState = stableRightHandState;

            //Debug.Log($"Right Hand - Average Angle: {averageAngle:F2} degrees - Stable State: {stableRightHandState}");

            if (stableRightHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
            }
            else if (stableRightHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    shootingManager.TryShoot(); // �߻� ��û
                    StartCoroutine(GestureCooldown());
                }
            }
        }
    }

    // �� ���� ��� ������Ʈ
    private void UpdateHandStateHistory(Queue<string> handStateHistory, string currentState)
    {
        if (handStateHistory.Count >= 3)
        {
            handStateHistory.Dequeue();
        }
        handStateHistory.Enqueue(currentState);
    }

    // 3������ ���� ���� ������ �� stable ���·� �Ǵ�
    private string GetStableHandState(Queue<string> handStateHistory, string previousStableState)
    {
        string[] states = handStateHistory.ToArray();

        if (states.Length == 3 && states[0] == states[1] && states[1] == states[2])
        {
            return states[0];
        }

        return previousStableState;
    }

    // �� ���¸� ������ �Ǵ� (0-40�� ���̸� Open, �� �ܴ� Fist)
    private string GetHandState(float averageAngle)
    {
        if (averageAngle <= 40f)
        {
            return "Open";
        }
        return "Fist";
    }

    // �� �հ����� ��� ������ ����ϴ� �Լ� (�����հ��� ����)
    private float GetAverageFingerAngle(Vector3[] points)
    {
        if (points == null || points.Length != 21)
            return -1f; // ��ȿ���� ���� ������

        float totalAngle = 0f;
        int angleCount = 0;

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

        return totalAngle / angleCount;
    }

    // �հ����� ������ ����ϴ� �Լ�
    private float GetFingerAngle(Vector3[] points, int index1, int index2, int index3)
    {
        Vector3 point1 = points[index1];
        Vector3 point2 = points[index2];
        Vector3 point3 = points[index3];

        Vector3 vector1 = (point2 - point1).normalized;
        Vector3 vector2 = (point3 - point2).normalized;

        return Vector3.Angle(vector1, vector2);
    }

    // ���� �յڷ� ���ƴ��� Ȯ���ϴ� �Լ�
    private bool IsHandOverlapping(Vector3[] leftPoints, Vector3[] rightPoints)
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
    private bool IsOneHandBehind(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        Vector3 leftHandCenter = GetHandCenter(leftPoints);
        Vector3 rightHandCenter = GetHandCenter(rightPoints);

        return Mathf.Abs(leftHandCenter.z - rightHandCenter.z) > 0.05f;
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