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

        // 두 손 모두 인식된 경우
        if (leftHandPositions.Length == 21 && rightHandPositions.Length == 21)
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
            }
            // Aiming (두 손가락이 모아진 상태에서 앞뒤로 위치)
            else if ((leftFist || leftGathered) && (rightFist || rightGathered) && isOneHandBehind)
            {
                Debug.Log("Ready to aim");
                bowAnimator.SetBool("Aiming", true);
            }
            // Shooting (앞 손은 주먹 쥐고 뒷 손은 펼치면 발사 요청)
            else if (bowAnimator.GetBool("Aiming") && ((leftFist && isRightHandOpen) || (rightFist && isLeftHandOpen)))
            {
                Debug.Log("Request to shoot");
                bowAnimator.SetBool("Aiming", false);
                shootingManager.TryShoot(); // ShootingManager에 발사 요청
            }
        }
        // 한 손만 인식된 경우 (두 손이 인식되지 않으면 기본 상태로 복귀) 잘 안 됨 왜이카노
        else if (leftHandPositions.Length == 21 || rightHandPositions.Length == 21)
        {
            bool leftFist = leftHandPositions.Length == 21 && IsFistClosed(leftHandPositions);
            bool rightFist = rightHandPositions.Length == 21 && IsFistClosed(rightHandPositions);

            // Aiming (너무 포개져서 주먹쥔 한 손만 인식된 경우)
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

    // 손이 모아진 상태 확인
    // 첫 번째 조건: 손가락 끝과 기저부의 거리 계산 (주먹 쥔 제스처)
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

    // 두 번째 조건: 손가락 끝들의 중심 거리 계산 (이탈리아 제스처)
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
        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) < 0.5f) // 거리 기준 조정
            {
                return false;
            }
        }
        return true;
    }
}