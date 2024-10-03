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

        // ������ ����� �������� �ʾ��� �� null�� �� �����Ƿ� Ȯ��
        if (_currentQuestion == null)
        {
            Debug.LogError("Current question is null");
            return;
        }

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        // O/X ��ư�� null�� ��츦 �����ϱ� ���� Ȯ��
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

        // ���� �ؽ�Ʈ�� DescText UI ��ҿ� ����
        var descText = GetText((int)Texts.DescText);
        if (descText == null)
        {
            Debug.LogError("DescText is null");
            return;
        }

        descText.text = _currentQuestion.question;  // ���� ������ DescText�� ����

        // ������ ����� ���ε��Ǿ����� �α� ���
        Debug.Log($"Question: {_currentQuestion.question}, Answer: {_currentQuestion.answer}");
    }



    private void OnAnswer(string selectedAnswer)
    {
        _onAnswerSelected?.Invoke(selectedAnswer);
    }
}
