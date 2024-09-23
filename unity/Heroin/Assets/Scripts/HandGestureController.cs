using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public Animator bowAnimator;  // 활 애니메이션을 위한 Animator
    private bool canShoot = true; // 활을 쏠 수 있는지 여부를 저장하는 플래그

    void Start()
    {
        bowAnimator.SetBool("Aiming", false);
    }

    void Update()
    {
        // HandTracking 인스턴스에서 좌표 데이터 가져오기
        Vector3[] leftHandPositions = HandTracking.Instance.leftHandPositions;
        Vector3[] rightHandPositions = HandTracking.Instance.rightHandPositions;
        canShoot = true;

        // 두 손 모두 인식된 경우
        if (leftHandPositions.Length == 21 && rightHandPositions.Length == 21)
        {
            bool leftFist = IsFistClosed(leftHandPositions);
            bool rightFist = IsFistClosed(rightHandPositions);
            bool handsOverlapping = IsHandOverlapping(leftHandPositions, rightHandPositions);
            bool isBackHandOpen = IsHandOpen(rightHandPositions);
            bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);


            // 활 쏘기 준비 상태 (두 손이 주먹을 쥔 상태에서 앞뒤로 포개짐)
            if (canShoot && leftFist && rightFist && handsOverlapping)
            {
                Debug.Log("Ready to aim");
                bowAnimator.SetBool("Aiming", true);
            }
            // 활을 쏘는 동작 (뒤쪽 손이 펴진 경우)
            else if (bowAnimator.GetBool("Aiming") && leftFist && isBackHandOpen && canShoot)
            {
                Debug.Log("Shot the arrow!");
                bowAnimator.SetBool("Aiming", false);
                bowAnimator.SetTrigger("Shoot");
                canShoot = false;  // 한 번 쏘고 나면 다시 쏠 수 없게 함
            }
        }
        // 한 손만 인식된 경우 (두 손이 인식되지 않으면 기본 상태로 복귀)
        else if (leftHandPositions.Length == 21 || rightHandPositions.Length == 21)
        {
            bool leftFist = leftHandPositions.Length == 21 && IsFistClosed(leftHandPositions);
            bool rightFist = rightHandPositions.Length == 21 && IsFistClosed(rightHandPositions);

            // 한 손만 주먹을 쥐고 있는 경우에도 Aiming 상태
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

    // 주먹 쥔 상태 확인
    bool IsFistClosed(Vector3[] points)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 fingerTip = points[8 + (i * 4)];
            Vector3 fingerBase = points[5 + (i * 4)];

            if (Vector3.Distance(fingerTip, fingerBase) > 0.5f) // 거리를 기준으로 판단 (값은 조정 필요)
            {
                return false;
            }
        }
        return true;
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

    // 한 손이 다른 한 손의 뒤에 있는지 확인 (포개지지 않음)
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

            if (Vector3.Distance(fingerTip, fingerBase) < 0.4f) // 거리 기준 조정
            {
                return false;
            }
        }
        return true;
    }
}
