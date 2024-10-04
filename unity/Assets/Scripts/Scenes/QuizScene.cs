using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizScene : BaseScene
{
    public CameraController _cameraController;
    private int _currentQuestionIndex = 0;
    private List<QuizQuestion> _quizQuestions;
    private UI_Quiz_Start _startUI;  // Start UI를 관리하는 변수 추가
    private UI_Quiz _quizUI;  // Quiz UI를 관리하는 변수 추가

    private void Start()
    {
        // 카메라 컨트롤러 초기화
        _cameraController = FindObjectOfType<CameraController>();

        if (_cameraController != null)
        {
            _cameraController.onCloseUpComplete = OnCloseUpComplete;
        }

        // Managers.Data를 통해 퀴즈 데이터를 로드
        _quizQuestions = Managers.Data.QuizQuestions;

        if (_quizQuestions == null || _quizQuestions.Count == 0)
        {
            Debug.LogError("No quiz questions loaded.");
            return;
        }

        Debug.Log("Loaded quiz questions successfully.");

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-13.1f, 55.7f, -79.8f), new Vector3(46.77f, 17.83f, -45.87f),
            new Quaternion(-0.24f, -0.54f, 0.16f, -0.8f), new Quaternion(-0.0227f, 0.717f, 0.0235f, 0.7f),
             1.0f);

        ShowStartUI();
    }

    private void ShowStartUI()
    {
        Debug.Log("Showing UI_Quiz_Start");

        _startUI = Managers.UI.ShowPopupUI<UI_Quiz_Start>();  // Start UI 표시
        StartCoroutine(HideStartUIAfterDelay(_startUI, 5.0f));  // 3초 후 Start UI 숨기기
    }

    private IEnumerator HideStartUIAfterDelay(UI_Quiz_Start startUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(startUI);  // Start UI 숨기기

        MoveToNextSpot();
    }

    private void MoveToNextSpot()
    {
        if (_currentQuestionIndex >= _quizQuestions.Count)
        {
            EndQuiz();
            return;
        }
        switch (_currentQuestionIndex)
        {
            case 0:
                _cameraController.StartCloseUp(
                    new Vector3(46.77f, 17.83f, -45.87f), // 시작 위치
                    new Vector3(46.77f, 17.83f, -45.87f), // 끝 위치
                    new Quaternion(-0.0227f, 0.717f, 0.0235f, 0.7f),          // 시작 회전
                    new Quaternion(-0.0227f, 0.717f, 0.0235f, 0.7f),          // 끝 회전
                    1.0f                                // 시간
                );
                break;
            case 1:
                _cameraController.StartCloseUp(
                    new Vector3(46.77f, 17.83f, -45.87f), // 시작 위치
                    new Vector3(111.85f, 22.97f, -45.32f), // 끝 위치
                    new Quaternion(-0.0227f, 0.717f, 0.0235f, 0.7f),          // 시작 회전
                    new Quaternion(-0.003f, 0.71f, 0.003f, 0.7f),          // 끝 회전
                    1.0f                                // 시간
                );
                break;
            case 2:
                _cameraController.StartCloseUp(
                    new Vector3(111.85f, 22.97f, -45.32f), // 시작 위치
                    new Vector3(143.476f, 31.22f, -45.7f), // 끝 위치
                    new Quaternion(-0.003f, 0.71f, 0.003f, 0.7f),          // 시작 회전
                    new Quaternion(-0.003f, 0.71f, 0.003f, 0.7f),          // 끝 회전
                    1.0f                                // 시간
                );
                break;
        }

        
    }

    private void OnCloseUpComplete()
    {
        ShowQuizUI();
    }

    private void ShowQuizUI()
    {
        // 현재 퀴즈 질문이 null이 아닌지 확인
        if (_quizQuestions == null || _quizQuestions.Count == 0)
        {
            Debug.LogError("Quiz questions are not loaded or empty.");
            return;
        }

        if (_currentQuestionIndex >= _quizQuestions.Count)
        {
            Debug.LogError("Current question index is out of range.");
            return;
        }

        _quizUI = Managers.UI.ShowPopupUI<UI_Quiz>();  // Quiz UI 표시

        // 현재 질문이 null인 경우 확인
        var currentQuestion = _quizQuestions[_currentQuestionIndex];
        if (currentQuestion == null)
        {
            Debug.LogError("Current quiz question is null.");
            return;
        }

        _quizUI.InitQuiz(currentQuestion, OnAnswerSelected);
    }

    private void OnAnswerSelected(string selectedAnswer)
    {
        string correctAnswer = _quizQuestions[_currentQuestionIndex].answer;

        // Start UI가 활성화된 경우 숨기기
        if (_startUI != null)
        {
            Managers.UI.ClosePopupUI(_startUI);  // Start UI를 숨김
        }

        // Quiz UI 숨기기 (정답을 선택할 때)
        if (_quizUI != null)
        {
            Managers.UI.ClosePopupUI(_quizUI);  // Quiz UI 숨김
        }

        if (selectedAnswer == correctAnswer)
        {
            // 정답 처리
            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_02", Define.Sound.Effect, 0.8f);
            UI_Quiz_Correct correctUI = Managers.UI.ShowPopupUI<UI_Quiz_Correct>();
            StartCoroutine(ProceedToNextQuestionAfterDelay(correctUI, 2.0f));  // Correct UI 표시 후 2초 후 다음 문제로 이동
        }
        else
        {
            // 오답 처리
            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/jingle_chime_22_negative", Define.Sound.Effect, 0.8f);
            UI_Quiz_Wrong wrongUI = Managers.UI.ShowPopupUI<UI_Quiz_Wrong>();
            StartCoroutine(ProceedToSameQuestionAfterDelay(wrongUI, 2.0f));  // Wrong UI 표시 후 2초 후 같은 문제 반복
        }
    }

    private IEnumerator ProceedToNextQuestionAfterDelay(UI_Popup popupUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(popupUI);  // Correct UI 닫기

        _currentQuestionIndex++;  // 다음 문제로 이동
        if (_currentQuestionIndex < _quizQuestions.Count)
        {
            MoveToNextSpot();  // 다음 문제로 카메라 이동
        }
        else
        {
            EndQuiz();  // 퀴즈 종료 처리
        }
    }

    private IEnumerator ProceedToSameQuestionAfterDelay(UI_Popup popupUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(popupUI);  // Wrong UI 닫기

        ShowQuizUI();  // 같은 문제 다시 표시
    }
    private IEnumerator WaitForFinishUIAndLoadScene(UI_Popup finishUI, float delay)
    {
        yield return new WaitForSeconds(delay);  // 일정 시간 대기

        // UI가 완전히 표시되었다고 판단되면 해당 UI를 닫고 다음 씬으로 넘어감
        Managers.UI.ClosePopupUI(finishUI);
        Debug.Log("Quiz finished!");
        Managers.Sound.StopBGM();
        Managers.Scene.LodingLoadScene(Define.Scene.Home);  // 다음 씬으로 전환
    }
    private void EndQuiz()
    {
        Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_single_02", Define.Sound.Effect, 0.8f);
        UI_Quiz_Finish finishUI = Managers.UI.ShowPopupUI<UI_Quiz_Finish>();
        Debug.Log("Quiz finished!");
        StartCoroutine(WaitForFinishUIAndLoadScene(finishUI, 4.0f));
    }

    public override void Clear()
    {
        Debug.Log("QuizScene Clear!");
    }

}
