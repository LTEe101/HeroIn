using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
public class UI_Paper : UI_Popup { 
    enum Texts
    {
        DescText,
    }

    enum Images
    {
        PaperImage,
    }

    enum Buttons
    {
        NextButton,
        PrevButton,
        StartButton
    }
private List<string> _narrationList;  // 현재 사용 중인 내레이션 리스트
private int _currentNarrationIndex = 0; // 현재 보여줄 내레이션 인덱스

    private Define.StoryType _storyType;
    public void InitStory(Define.StoryType storyType)
    {
        if (storyType == Define.StoryType.Before)
        {
            _narrationList = Managers.Data.BeforeStoryList;
        }
        else if (storyType == Define.StoryType.After)
        {
            _narrationList = Managers.Data.AfterStoryList;
        }
        _storyType = storyType;
        Init();

        UpdateNarration();
        UpdateButtons();
    }
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));


        // PrevButton 클릭 시 이전 내레이션 표시
        BindEvent(GetButton((int)Buttons.PrevButton).gameObject, (PointerEventData data) => { PlayButtonSound(); ShowPreviousNarration(); }, Define.UIEvent.Click);

        // NextButton 클릭 시 다음 내레이션 표시
        BindEvent(GetButton((int)Buttons.NextButton).gameObject, (PointerEventData data) => { PlayButtonSound(); ShowNextNarration();  }, Define.UIEvent.Click);

        BindEvent(GetButton((int)Buttons.StartButton).gameObject, (PointerEventData data) => { PlayButtonSound(); ShowNextNarration();  }, Define.UIEvent.Click);
    }
    private void Update()
    {
    }

    // 내레이션을 업데이트하는 함수
    private void UpdateNarration()
    {
        if (_currentNarrationIndex < _narrationList.Count)
        {
            GetText((int)Texts.DescText).text = _narrationList[_currentNarrationIndex];
        }
    }
    // 버튼 상태를 업데이트하는 함수 (처음과 마지막에는 Prev/Next 버튼을 숨김)
    private void UpdateButtons()
    {
        GetButton((int)Buttons.PrevButton).gameObject.SetActive(_currentNarrationIndex > 0); // 첫 번째면 Prev 버튼 비활성화
        GetButton((int)Buttons.NextButton).gameObject.SetActive(_currentNarrationIndex < _narrationList.Count - 1); // 마지막이면 Next 버튼 비활성화
        GetButton((int)Buttons.StartButton).gameObject.SetActive(_currentNarrationIndex == _narrationList.Count - 1); // 마지막이면 Start 버튼 활성화
    }

    // 다음 내레이션을 표시하는 함수
    private void ShowNextNarration()
    {
        if (_currentNarrationIndex < _narrationList.Count - 1)
        {
            _currentNarrationIndex++;
            UpdateButtons();
            UpdateNarration();
        }
        else
        {
            ClosePopupUI();
            if (_storyType == Define.StoryType.Before)
            {
                Managers.Scene.LoadScene(Define.Scene.StoryOne);
            }
            else
            {
                StartCoroutine(Managers.API.AddHistoryTitle(
                    1, 
                    Managers.Data.userInfo.userId, 
                    () => {
                        // 업적 저장에 성공했을 때 처리
                        Debug.Log("업적 저장 성공: ");
                    }
                ));

                Managers.Sound.StopBGM();
                // 타임머신으로 돌아가기
                Managers.Scene.LodingLoadScene(Define.Scene.Home);
            }
        }
    }
    // 이전 내레이션을 표시하는 함수
    private void ShowPreviousNarration()
    {
        if (_currentNarrationIndex > 0)
        {
            _currentNarrationIndex--;
            UpdateButtons();
            UpdateNarration();
        }
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("ProEffect/User_Interface_Menu/ui_menu_button_keystroke_01", Define.Sound.Effect, 0.2f);
    }
}

