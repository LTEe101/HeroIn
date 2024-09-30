using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 7f;

    // 각 손의 최근 프레임 상태를 저장하기 위한 Queue (3개의 프레임 비교)
    private Queue<string> leftHandStateHistory = new Queue<string>(3);
    private Queue<string> rightHandStateHistory = new Queue<string>(3);

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
        else if (isLeftHandValid)
        {
            ProcessSingleHand(leftHandPositions, "Left");
        }
        else if (isRightHandValid)
        {
            ProcessSingleHand(rightHandPositions, "Right");
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
        }
    }

    private void ProcessBothHands(Vector3[] leftHandPositions, Vector3[] rightHandPositions)
    {
        float leftAverageAngle = GetAverageFingerAngle(leftHandPositions);
        float rightAverageAngle = GetAverageFingerAngle(rightHandPositions);

        // 디버깅: 두 손의 각도를 로그로 출력
        //Debug.Log($"[Both Hands] Left Hand Average Angle: {leftAverageAngle}");
        //Debug.Log($"[Both Hands] Right Hand Average Angle: {rightAverageAngle}");

        string currentLeftHandState = GetHandState(leftAverageAngle);
        string currentRightHandState = GetHandState(rightAverageAngle);

        UpdateHandStateHistory(leftHandStateHistory, currentLeftHandState);
        UpdateHandStateHistory(rightHandStateHistory, currentRightHandState);

        string stableLeftHandState = GetStableHandState(leftHandStateHistory);
        string stableRightHandState = GetStableHandState(rightHandStateHistory);

        Debug.Log($"[Both] Left - Average Angle: {leftAverageAngle} - Stable State: {stableLeftHandState}");
        //Debug.Log($"Right Hand - Stable State: {stableRightHandState}");

        if (stableLeftHandState == "Fist" && stableRightHandState == "Fist")
        {
            bowAnimator.SetBool("Aiming", true);
        }
        else if (stableLeftHandState == "Open" && stableRightHandState == "Open")
        {
            if (bowAnimator.GetBool("Aiming"))
            {
                bowAnimator.SetBool("Aiming", false);
                shootingManager.TryShoot(); // 발사 요청
                StartCoroutine(GestureCooldown());
            }
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
        }
    }

    private void ProcessSingleHand(Vector3[] handPositions, string handType)
    {
        float averageAngle = GetAverageFingerAngle(handPositions);

        string currentHandState = GetHandState(averageAngle);

        if (handType == "Left")
        {
            UpdateHandStateHistory(leftHandStateHistory, currentHandState);
            string stableLeftHandState = GetStableHandState(leftHandStateHistory);
            Debug.Log($"[Single] Left - Average Angle: {averageAngle} - Stable State: {stableLeftHandState}");

            if (stableLeftHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
            }
            else if (stableLeftHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    shootingManager.TryShoot(); // 발사 요청
                    StartCoroutine(GestureCooldown());
                }
            }
        }
        else if (handType == "Right")
        {
            UpdateHandStateHistory(rightHandStateHistory, currentHandState);
            string stableRightHandState = GetStableHandState(rightHandStateHistory);
            //Debug.Log($"{handType} Hand - Stable State: {stableRightHandState} Hand Average Angle: {averageAngle}");

            if (stableRightHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
            }
            else if (stableRightHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    shootingManager.TryShoot(); // 발사 요청
                    StartCoroutine(GestureCooldown());
                }
            }
        }
    }

    private void UpdateHandStateHistory(Queue<string> handStateHistory, string currentState)
    {
        if (handStateHistory.Count >= 3)
        {
            handStateHistory.Dequeue();
        }
        handStateHistory.Enqueue(currentState);
    }

    private string GetStableHandState(Queue<string> handStateHistory)
    {
        int fistCount = 0;
        int openCount = 0;

        foreach (string state in handStateHistory)
        {
            if (state == "Fist")
            {
                fistCount++;
            }
            else if (state == "Open")
            {
                openCount++;
            }
        }

        if (fistCount >= 2)
        {
            return "Fist";
        }
        else if (openCount >= 2)
        {
            return "Open";
        }
        else
        {
            return "Unknown";
        }
    }

    private string GetHandState(float averageAngle)
    {
        if (averageAngle < 0f)
        {
            return "Unknown";
        }

        if (averageAngle <= 40f)
        {
            return "Open";
        }
        else if (averageAngle >= 60f && averageAngle <= 120f)
        {
            return "Fist";
        }
        else
        {
            return "Unknown";
        }
    }

    float GetAverageFingerAngle(Vector3[] points)
    {
        if (points == null || points.Length != 21)
        {
            return -1f; // 유효하지 않은 데이터
        }

        float totalAngle = 0f;
        int angleCount = 0;

        totalAngle += GetFingerAngle(points, 0, 5, 6);   // 검지
        totalAngle += GetFingerAngle(points, 0, 9, 10);  // 중지
        totalAngle += GetFingerAngle(points, 0, 13, 14); // 약지
        totalAngle += GetFingerAngle(points, 0, 17, 18); // 새끼
        angleCount += 4;

        totalAngle += GetFingerAngle(points, 5, 6, 7);   // 검지
        totalAngle += GetFingerAngle(points, 9, 10, 11); // 중지
        totalAngle += GetFingerAngle(points, 13, 14, 15); // 약지
        totalAngle += GetFingerAngle(points, 17, 18, 19); // 새끼
        angleCount += 4;

        float averageAngle = totalAngle / angleCount;

        return averageAngle;
    }

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

    private IEnumerator GestureCooldown()
    {
        canDetectGesture = false;
        yield return new WaitForSeconds(GESTURE_COOLDOWN);
        canDetectGesture = true;
    }
}
