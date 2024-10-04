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
        Init(); // 초기화
        UpdateCurrentStateText("게임 준비 중"); // 초기 상태 텍스트 설정
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts)); // UI 요소 바인딩
    }

    public void UpdateCurrentStateText(string state)
    {
        Text stateText = Get<Text>((int)Texts.StateText);
        if (stateText != null)
        {
            stateText.text = state; // 상태 텍스트 업데이트
            Debug.Log($"UI_Motion_State 텍스트 업데이트: {state}"); // 로그 추가
        }
        else
        {
            Debug.LogError("상태 텍스트 컴포넌트를 찾을 수 없습니다."); // 오류 메시지
        }
    }

    // Close 메서드 추가
    public void Close()
    {
        gameObject.SetActive(false); // UI를 비활성화합니다.
        Debug.Log("UI_Motion_State가 닫혔습니다."); // 닫힘 로그
    }
}

