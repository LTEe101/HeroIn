using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandGestureController : MonoBehaviour, IMotionGameScript
{
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private ShootingManager shootingManager;
    [SerializeField] private UI_Motion_State uiMotionState;

    private bool canDetectGesture = true;
    private const float GESTURE_COOLDOWN = 5f;

    // 최근 3프레임의 손 상태를 저장하기 위한 큐
    private Queue<string> leftHandStateHistory = new Queue<string>(3);
    private Queue<string> rightHandStateHistory = new Queue<string>(3);

    // 이전 stable 상태를 저장
    private string previousLeftHandStableState = "Open";
    private string previousRightHandStableState = "Open";

    private bool isAimingSoundPlayed = false; // 사운드 재생 여부를 체크하는 플래그

    public bool IsEnabled
    {
        get { return enabled; }
        set { enabled = value; }
    }
    private void Start()
    {
        // UI_Motion_State 인스턴스를 찾기
        if (uiMotionState == null)
        {
            uiMotionState = FindObjectOfType<UI_Motion_State>();
        }

        if (uiMotionState != null)
        {
            uiMotionState.UpdateCurrentStateText("카메라에 두 손이 잘 보이도록 해주세요"); // 초기 상태 설정
            Debug.Log("UI_Motion_State 인스턴스가 할당되었습니다."); // 로그 추가
            Debug.Log($"UI_Motion_State 활성화 상태: {uiMotionState.gameObject.activeSelf}"); // 활성화 상태 로그
        }
        else
        {
            Debug.LogError("UI_Motion_State reference is not set."); // 오류 메시지
        }
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
        else
        {
            bowAnimator.SetBool("Aiming", false);
            UpdateHandStateText("카메라에 두 손이 잘 보이도록 해주세요");
        }
    }

    // 공통 함수로 사운드 재생을 처리 (지연 재생 추가)
    private void PlayAimingSound()
    {
        if (!isAimingSoundPlayed)
        {
            StartCoroutine(PlaySoundWithDelay("ProEffect/Guns_Weapons/Bow_Arrow/bow_crossbow_arrow_draw_stretch2_01후보", 0.8f, 3.0f, 0.4f));
            isAimingSoundPlayed = true; // 소리가 재생되었음을 기록
            Debug.Log("조준 소리 재생 대기");
        }
    }

    // 지연 후 사운드를 재생하는 코루틴
    private IEnumerator PlaySoundWithDelay(string soundPath, float volume, float pitch, float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        Managers.Sound.Play(soundPath, Define.Sound.Effect, volume, pitch); // 사운드 재생
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

        previousLeftHandStableState = stableLeftHandState;
        previousRightHandStableState = stableRightHandState;

        bool isOneHandBehind = IsOneHandBehind(leftHandPositions, rightHandPositions);

        if (stableLeftHandState == "Fist" && stableRightHandState == "Fist" && isOneHandBehind)
        {
            if (!bowAnimator.GetBool("Aiming"))
            {
                bowAnimator.SetBool("Aiming", true);
                isAimingSoundPlayed = false;
                UpdateHandStateText("조준 중");
            }

            PlayAimingSound(); // 조준 소리 재생
        }
        else if (bowAnimator.GetBool("Aiming") &&
                 ((stableLeftHandState == "Fist" && stableRightHandState == "Open") ||
                  (stableLeftHandState == "Open" && stableRightHandState == "Fist")))
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("발사");
            UpdateHandStateText("발사");
            shootingManager.TryShoot();
            StartCoroutine(GestureCooldown());
            isAimingSoundPlayed = false;
        }
        else
        {
            bowAnimator.SetBool("Aiming", false);
            Debug.Log("그냥 있어");
            UpdateHandStateText("카메라에 두 손이 잘 보이도록 해주세요");
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

            if (stableLeftHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
                PlayAimingSound(); // 조준 소리 재생
                Debug.Log("조준");
                UpdateHandStateText("조준 중");
            }
            else if (stableLeftHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    Debug.Log("발사");
                    UpdateHandStateText("발사");
                    shootingManager.TryShoot();
                    StartCoroutine(GestureCooldown());
                }
            }
            else
            {
                UpdateHandStateText("카메라에 두 손이 잘 보이도록 해주세요");
            }
        }
        else if (handType == "Right")
        {
            UpdateHandStateHistory(rightHandStateHistory, currentHandState);
            string stableRightHandState = GetStableHandState(rightHandStateHistory, previousRightHandStableState);
            previousRightHandStableState = stableRightHandState;

            if (stableRightHandState == "Fist")
            {
                bowAnimator.SetBool("Aiming", true);
                PlayAimingSound(); // 조준 소리 재생
                Debug.Log("조준");
                UpdateHandStateText("조준 중");
            }
            else if (stableRightHandState == "Open")
            {
                if (bowAnimator.GetBool("Aiming"))
                {
                    bowAnimator.SetBool("Aiming", false);
                    Debug.Log("발사");
                    UpdateHandStateText("발사");
                    shootingManager.TryShoot();
                    StartCoroutine(GestureCooldown());
                }
            }
            else
            {
                UpdateHandStateText("카메라에 두 손이 잘 보이도록 해주세요");
            }
        }
    }
    private void UpdateHandStateText(string state)
    {
        Debug.Log($"상태 텍스트 업데이트 시도: {state}"); // 업데이트 시도 로그

        if (uiMotionState != null)
        {
            uiMotionState.UpdateCurrentStateText(state); // 상태 텍스트 업데이트
            Debug.Log("상태 텍스트가 업데이트되었습니다."); // 업데이트 완료 로그
        }
        else
        {
            Debug.LogError("UI_Motion_State reference is not set."); // 오류 메시지
        }
    }

    // 손 상태 기록 업데이트
    private void UpdateHandStateHistory(Queue<string> handStateHistory, string currentState)
    {
        if (handStateHistory.Count >= 3)
        {
            handStateHistory.Dequeue();
        }
        handStateHistory.Enqueue(currentState);
    }

    // 3프레임 동안 같은 상태일 때 stable 상태로 판단
    private string GetStableHandState(Queue<string> handStateHistory, string previousStableState)
    {
        string[] states = handStateHistory.ToArray();

        if (states.Length == 3 && states[0] == states[1] && states[1] == states[2])
        {
            return states[0];
        }

        return previousStableState;
    }

    // 손 상태를 각도로 판단 (0-40도 사이면 Open, 그 외는 Fist)
    private string GetHandState(float averageAngle)
    {
        if (averageAngle <= 40f)
        {
            return "Open";
        }
        return "Fist";
    }

    // 네 손가락의 평균 각도를 계산하는 함수 (엄지손가락 제외)
    private float GetAverageFingerAngle(Vector3[] points)
    {
        if (points == null || points.Length != 21)
            return -1f; // 유효하지 않은 데이터

        float totalAngle = 0f;
        int angleCount = 0;

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

        return totalAngle / angleCount;
    }

    // 손가락의 각도를 계산하는 함수
    private float GetFingerAngle(Vector3[] points, int index1, int index2, int index3)
    {
        Vector3 point1 = points[index1];
        Vector3 point2 = points[index2];
        Vector3 point3 = points[index3];

        Vector3 vector1 = (point2 - point1).normalized;
        Vector3 vector2 = (point3 - point2).normalized;

        return Vector3.Angle(vector1, vector2);
    }

    // 손이 앞뒤로 겹쳤는지 확인하는 함수
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

    // 한 손이 다른 한 손의 뒤에 있는지 확인하는 함수
    private bool IsOneHandBehind(Vector3[] leftPoints, Vector3[] rightPoints)
    {
        // 왼손과 오른손의 손바닥 중심 좌표를 구합니다.
        Vector3 leftHandCenter = GetHandCenter(leftPoints);
        Vector3 rightHandCenter = GetHandCenter(rightPoints);

        // 손바닥의 중심 좌표와 손가락 끝의 z축 값 차이도 확인합니다.
        float leftHandZ = leftHandCenter.z;
        float rightHandZ = rightHandCenter.z;

        // 손바닥 중심의 z축 차이를 기준으로 한 손이 뒤에 있는지 판단합니다.
        if (leftHandZ < rightHandZ)
        {
            return AreFingersBehind(leftPoints, rightPoints);
        }
        else if (rightHandZ < leftHandZ)
        {
            return AreFingersBehind(rightPoints, leftPoints);
        }

        return false;
    }

    private bool AreFingersBehind(Vector3[] backHandPoints, Vector3[] frontHandPoints)
    {
        for (int i = 5; i <= 20; i += 4)
        {
            if (backHandPoints[i].z >= frontHandPoints[i].z)
            {
                return false;
            }
        }
        return true;
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
