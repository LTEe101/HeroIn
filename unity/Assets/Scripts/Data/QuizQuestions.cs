using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    public string question;  // ���� �ؽ�Ʈ
    public string answer;    // ���� �ؽ�Ʈ (O �Ǵ� X)
}

[System.Serializable]
public class QuizQuestionList
{
    public List<QuizQuestion> questions;  // ���� ���� ����Ʈ
}
