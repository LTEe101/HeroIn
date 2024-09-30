using System;
using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 5f;

    // 이전 프레임의 손 상태 저장
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
            // 두 손 모두 인식된 경우
            ProcessBothHands(leftHandPositions, rightHandPositions);
        }
        else if (isLeftHandValid || isRightHandValid)
        {
            // 한 손만 인식된 경우
            ProcessSingleHand(isLeftHandValid ? leftHandPositions : rightHandPositions, isLeftHandValid ? "Left" : "Right");
        }
        else
        {
            // 손 데이터가 모두 유효하지 않음
            bowAnimator.SetBool("Aiming", false);
        }
    }

    private void ProcessBothHands(Vector3[] leftHandPositions, Vector3[] rightHandPositions)
    {
        float leftAverageAngle = GetAverageFingerAngle(leftHandPositions);
        float rightAverageAngle = GetAverageFingerAngle(rightHandPositions);

        bool isLeftHandOpen = IsHandOpen(leftAverageAngle);
        bool isRightHandOpen = IsHandOpen(rightAverageAngle);

        // 'Open' 아니면 'Fist'로 판단
        bool leftFist = !isLeftHandOpen;
        bool rightFist = !isRightHandOpen;

        bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

        // 이전 상태와 비교하여 동일한 경우에만 업데이트
        bool stableLeftFist = (leftFist == previousLeftFist) ? leftFist : previousLeftFist;
        bool stableRightFist = (rightFist == previousRightFist) ? rightFist : previousRightFist;
        bool stableIsLeftHandOpen = (isLeftHandOpen == previousIsLeftHandOpen) ? isLeftHandOpen : previousIsLeftHandOpen;
        bool stableIsRightHandOpen = (isRightHandOpen == previousIsRightHandOpen) ? isRightHandOpen : previousIsRightHandOpen;

        // 이전 상태 업데이트
        previousLeftFist = leftFist;
        previousRightFist = rightFist;
        previousIsLeftHandOpen = isLeftHandOpen;
        previousIsRightHandOpen = isRightHandOpen;

        // 좌우 손 상태와 평균 각도 출력 (디버그용)
        // Debug.Log($"Left Hand - State: {(stableIsLeftHandOpen ? "Open" : "Fist")}, Average Angle: {leftAverageAngle:F2} degrees");
        // Debug.Log($"Right Hand - State: {(stableIsRightHandOpen ? "Open" : "Fist")}, Average Angle: {rightAverageAngle:F2} degrees");

        if (IsHandOverlapping(leftHandPositions, rightHandPositions))
        {
            // 두 손이 겹침, 이전 상태 유지
        }
        else if (stableLeftFist && stableRightFist && isOneHandBehind)
        {
            bowAnimator.SetBool("Aiming", true);
        }
        else if (bowAnimator.GetBool("Aiming") && ((stableLeftFist && stableIsRightHandOpen) || (stableRightFist && stableIsLeftHandOpen)))
        {
            bowAnimator.SetBool("Aiming", false);
            shootingManager.TryShoot(); // 발사 요청
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

        // 이전 상태와 비교하여 동일한 경우에만 업데이트
        bool stableFist = (fist == (handType == "Left" ? previousLeftFist : previousRightFist)) ? fist : (handType == "Left" ? previousLeftFist : previousRightFist);

        // 이전 상태 업데이트
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

        // 손 상태와 평균 각도 출력 (디버그용)
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

    // 네 손가락의 평균 각도를 계산하는 함수 (엄지손가락 제외)
    float GetAverageFingerAngle(Vector3[] points)
    {
        if (points == null || points.Length != 21)
            return -1f; // 유효하지 않은 데이터

        float totalAngle = 0f;
        int angleCount = 0;

        // 각 손가락에 대해 두 개의 각도 계산 (엄지손가락 제외)
        totalAngle += GetFingerAngle(points, 0, 5, 6);   // 검지
        totalAngle += GetFingerAngle(points, 0, 9, 10);  // 중지
        totalAngle += GetFingerAngle(points, 0, 13, 14); // 약지
        totalAngle += GetFingerAngle(points, 0, 17, 18); // 새끼
        angleCount += 4;

        totalAngle += GetFingerAngle(points, 5, 6, 7);    // 검지
        totalAngle += GetFingerAngle(points, 9, 10, 11);  // 중지
        totalAngle += GetFingerAngle(points, 13, 14, 15); // 약지
        totalAngle += GetFingerAngle(points, 17, 18, 19); // 새끼
        angleCount += 4;

        // 평균 각도 계산
        float averageAngle = totalAngle / angleCount;

        return averageAngle;
    }

    // 세 개의 포인트로 정의된 두 벡터 사이의 각도를 계산하는 함수
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

    // 평균 각도에 따라 손이 펼쳐져 있는지 확인하는 함수
    bool IsHandOpen(float averageAngle)
    {
        if (averageAngle < 0f)
            return false; // 유효하지 않은 데이터

        // 손을 펼친 것으로 판단하는 임계값 설정 (필요에 따라 조정)
        return averageAngle >= 0f && averageAngle <= 40f;
    }

    // 두 손이 겹쳐져 있는지 확인하는 함수
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

    // 한 손이 다른 한 손의 뒤에 있는지 확인하는 함수
    bool IsOneHandBehind(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        Vector3 leftHandCenter = GetHandCenter(leftPoints);
        Vector3 rightHandCenter = GetHandCenter(rightPoints);

        return Mathf.Abs(leftHandCenter.z - rightHandCenter.z) > 0.1f; // 임계값 조정 가능
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
