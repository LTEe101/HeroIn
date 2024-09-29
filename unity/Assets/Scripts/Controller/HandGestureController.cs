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
            Debug.LogWarning("손 데이터가 유효하지 않습니다.");
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

        // 앞 전 상태 유지 (두 손이 겹쳐져서 좌표 인식이 제대로 안 되는 경우)
        if (IsHandOverlapping(leftHandPositions, rightHandPositions))
        {
            Debug.Log("두 손 겹쳐짐, 이전 상태 유지");
        }
        // Aiming (두 손가락이 모아진 상태에서 앞뒤로 위치)
        else if ((leftFist || leftGathered) && (rightFist || rightGathered) && isOneHandBehind)
        {
            Debug.Log("두 손가락이 모아진 상태에서 앞뒤로 위치, 조준");
            bowAnimator.SetBool("Aiming", true);
        }
        // Shooting (앞 손은 주먹 쥐고 뒷 손은 펼치면 발사 요청)
        else if (bowAnimator.GetBool("Aiming") && ((leftFist && isRightHandOpen) || (rightFist && isLeftHandOpen)))
        {
            Debug.Log("앞 손 주먹 뒷 손 펼침, 발사 요청");
            bowAnimator.SetBool("Aiming", false);
            shootingManager.TryShoot(); // ShootingManager에 발사 요청
            StartCoroutine(GestureCooldown());
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("두 손 인식, 대기 상태");
        }
    }

    private void ProcessSingleHand(Vector3[] handPositions, string handType)
    {
        bool fist = IsFistClosed(handPositions);
        bool gathered = IsFingersGathered(handPositions);

        // Aiming (주먹 쥔 손 또는 손가락이 모아진 손)
        if (fist || gathered)
        {
            Debug.Log($"{handType} 손 주먹 또는 손가락 모음 인식, 조준");
            bowAnimator.SetBool("Aiming", true);
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log($"{handType} 손 인식, 대기 상태");
        }
    }

    // 아래부터는 기존 메서드들 (변경 사항 없음)
    // 손이 앞뒤로 포개졌는지 확인
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

    // 손이 모아진 상태 확인 (주먹 쥔 제스처)
    bool IsFistClosed(Vector3[] points)
    {
        // 포인트가 유효한지 확인
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

    // 손가락 끝들이 모여있는지 확인 (이탈리아 제스처)
    bool IsFingersGathered(Vector3[] points)
    {
        // 포인트가 유효한지 확인
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

    // 한 손이 다른 한 손의 뒤에 있는지 확인
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

    // 손이 펴졌는지 확인
    bool IsHandOpen(Vector3[] points)
    {
        // 포인트가 유효한지 확인
        if (points == null || points.Length != 21 || points[0] == Vector3.zero)
            return false;

        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) < 0.4f) // 거리 기준 조정
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator GestureCooldown()
    {
        Debug.Log("발사 후 대기 시작");
        canDetectGesture = false;
        yield return new WaitForSeconds(GESTURE_COOLDOWN);
        canDetectGesture = true;
        Debug.Log("발사 후 대기 끝");
    }
}
