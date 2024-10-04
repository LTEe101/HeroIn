using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Quiz : UI_Popup
{
    enum Texts
    {
        DescText,
    }

    enum Buttons
    {
        OButton,
        XButton
    }

    private QuizQuestion _currentQuestion;
    private System.Action<string> _onAnswerSelected;

    public void InitQuiz(QuizQuestion question, System.Action<string> onAnswerSelected)
    {
        _currentQuestion = question;
        _onAnswerSelected = onAnswerSelected;

        // 질문이 제대로 설정되지 않았을 때 null일 수 있으므로 확인
        if (_currentQuestion == null)
        {
            Debug.LogError("Current question is null");
            return;
        }

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        // O/X 버튼이 null인 경우를 방지하기 위해 확인
        var oButton = GetButton((int)Buttons.OButton);
        if (oButton == null)
        {
            Debug.LogError("OButton is null");
            return;
        }

        BindEvent(oButton.gameObject, (PointerEventData data) => OnAnswer("O"), Define.UIEvent.Click);

        var xButton = GetButton((int)Buttons.XButton);
        if (xButton == null)
        {
            Debug.LogError("XButton is null");
            return;
        }

        BindEvent(xButton.gameObject, (PointerEventData data) => OnAnswer("X"), Define.UIEvent.Click);

        // 문제 텍스트를 DescText UI 요소에 설정
        var descText = GetText((int)Texts.DescText);
        if (descText == null)
        {
            Debug.LogError("DescText is null");
            return;
        }

        descText.text = _currentQuestion.question;  // 퀴즈 질문을 DescText에 설정

        // 문제가 제대로 바인딩되었는지 로그 출력
        Debug.Log($"Question: {_currentQuestion.question}, Answer: {_currentQuestion.answer}");
    }



    private void OnAnswer(string selectedAnswer)
    {
        _onAnswerSelected?.Invoke(selectedAnswer);
    }
}
