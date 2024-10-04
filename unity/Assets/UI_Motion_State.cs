using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Motion_State : UI_Scene
{
    enum Texts
    {
        StateText,
    }

    private void Start()
    {
        Init(); // �ʱ�ȭ
        UpdateCurrentStateText("���� �غ� ��"); // �ʱ� ���� �ؽ�Ʈ ����
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts)); // UI ��� ���ε�
    }

    public void UpdateCurrentStateText(string state)
    {
        Text stateText = Get<Text>((int)Texts.StateText);
        if (stateText != null)
        {
            stateText.text = state; // ���� �ؽ�Ʈ ������Ʈ
            Debug.Log($"UI_Motion_State �ؽ�Ʈ ������Ʈ: {state}"); // �α� �߰�
        }
        else
        {
            Debug.LogError("���� �ؽ�Ʈ ������Ʈ�� ã�� �� �����ϴ�."); // ���� �޽���
        }
    }

    // Close �޼��� �߰�
    public void Close()
    {
        gameObject.SetActive(false); // UI�� ��Ȱ��ȭ�մϴ�.
        Debug.Log("UI_Motion_State�� �������ϴ�."); // ���� �α�
    }
}

