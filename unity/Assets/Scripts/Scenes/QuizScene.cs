using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizScene : BaseScene
{
    public CameraController _cameraController;
    private int _currentQuestionIndex = 0;
    private List<QuizQuestion> _quizQuestions;
    private UI_Quiz_Start _startUI;  // Start UI�� �����ϴ� ���� �߰�
    private UI_Quiz _quizUI;  // Quiz UI�� �����ϴ� ���� �߰�

    private void Start()
    {
        // ī�޶� ��Ʈ�ѷ� �ʱ�ȭ
        _cameraController = FindObjectOfType<CameraController>();

        if (_cameraController != null)
        {
            _cameraController.onCloseUpComplete = OnCloseUpComplete;
        }

        // Managers.Data�� ���� ���� �����͸� �ε�
        _quizQuestions = Managers.Data.QuizQuestions;

        if (_quizQuestions == null || _quizQuestions.Count == 0)
        {
            Debug.LogError("No quiz questions loaded.");
            return;
        }

        Debug.Log("Loaded quiz questions successfully.");
        ShowStartUI();
    }

    private void ShowStartUI()
    {
        Debug.Log("Showing UI_Quiz_Start");
        _startUI = Managers.UI.ShowPopupUI<UI_Quiz_Start>();  // Start UI ǥ��
        StartCoroutine(HideStartUIAfterDelay(_startUI, 3.0f));  // 3�� �� Start UI �����
    }

    private IEnumerator HideStartUIAfterDelay(UI_Quiz_Start startUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(startUI);  // Start UI �����

        MoveToNextSpot();
    }

    private void MoveToNextSpot()
    {
        if (_currentQuestionIndex >= _quizQuestions.Count)
        {
            EndQuiz();
            return;
        }

        // ī�޶� �̵� ����
        _cameraController.StartCloseUp(
            new Vector3(-13.1f, 55.7f, -79.8f), // ���� ��ġ
            new Vector3(34.9f, 26.5f, -59.3f), // �� ��ġ
            Quaternion.Euler(0, 0, 0),          // ���� ȸ��
            Quaternion.Euler(0, 0, 0),          // �� ȸ��
            1.0f                                // �ð�
        );
    }

    private void OnCloseUpComplete()
    {
        ShowQuizUI();
    }

    private void ShowQuizUI()
    {
        // ���� ���� ������ null�� �ƴ��� Ȯ��
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

        _quizUI = Managers.UI.ShowPopupUI<UI_Quiz>();  // Quiz UI ǥ��

        // ���� ������ null�� ��� Ȯ��
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

        // Start UI�� Ȱ��ȭ�� ��� �����
        if (_startUI != null)
        {
            Managers.UI.ClosePopupUI(_startUI);  // Start UI�� ����
        }

        // Quiz UI ����� (������ ������ ��)
        if (_quizUI != null)
        {
            Managers.UI.ClosePopupUI(_quizUI);  // Quiz UI ����
        }

        if (selectedAnswer == correctAnswer)
        {
            // ���� ó��
            Debug.Log("Correct answer selected.");
            UI_Quiz_Correct correctUI = Managers.UI.ShowPopupUI<UI_Quiz_Correct>();
            StartCoroutine(ProceedToNextQuestionAfterDelay(correctUI, 2.0f));  // Correct UI ǥ�� �� 2�� �� ���� ������ �̵�
        }
        else
        {
            // ���� ó��
            Debug.Log("Wrong answer selected.");
            UI_Quiz_Wrong wrongUI = Managers.UI.ShowPopupUI<UI_Quiz_Wrong>();
            StartCoroutine(ProceedToSameQuestionAfterDelay(wrongUI, 2.0f));  // Wrong UI ǥ�� �� 2�� �� ���� ���� �ݺ�
        }
    }

    private IEnumerator ProceedToNextQuestionAfterDelay(UI_Popup popupUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(popupUI);  // Correct UI �ݱ�

        _currentQuestionIndex++;  // ���� ������ �̵�
        if (_currentQuestionIndex < _quizQuestions.Count)
        {
            MoveToNextSpot();  // ���� ������ ī�޶� �̵�
        }
        else
        {
            EndQuiz();  // ���� ���� ó��
        }
    }

    private IEnumerator ProceedToSameQuestionAfterDelay(UI_Popup popupUI, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.UI.ClosePopupUI(popupUI);  // Wrong UI �ݱ�

        ShowQuizUI();  // ���� ���� �ٽ� ǥ��
    }

    private void EndQuiz()
    {
        UI_Quiz_Finish finishUI = Managers.UI.ShowPopupUI<UI_Quiz_Finish>();
        Debug.Log("Quiz finished!");
        Managers.Scene.LodingLoadScene(Define.Scene.Home);
    }

    public override void Clear()
    {
        Debug.Log("QuizScene Clear!");
    }
}
