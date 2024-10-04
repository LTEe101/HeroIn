using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    public string question;  // 문제 텍스트
    public string answer;    // 정답 텍스트 (O 또는 X)
}

[System.Serializable]
public class QuizQuestionList
{
    public List<QuizQuestion> questions;  // 퀴즈 질문 리스트
}
