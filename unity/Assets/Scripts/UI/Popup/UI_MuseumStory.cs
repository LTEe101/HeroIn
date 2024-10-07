using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static Define;

public class UI_MuseumStory: UI_Popup
{
    private List<string> _list;
    private int currentPage = 0;
    private Define.MuseumStoryType _storyType;

    enum Buttons
    {
        NextButton,
        PreviousButton,
        CloseButton,
        EnterButton,
    }

    enum Texts
    {
        DescriptionText,
        EnterText,
    }

    public void InitStory(Define.MuseumStoryType storyType)
    {
        switch (storyType)
        {
            case Define.MuseumStoryType.Story1592:
                _list = Managers.Data.MuseumStory1592;
                break;
            case Define.MuseumStoryType.Story1919:
                _list = Managers.Data.MuseumStory1919;
                break;
            case Define.MuseumStoryType.Story1443:
                _list = Managers.Data.MuseumStory1443;
                break;
        }
        _storyType = storyType;
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts)); 

        // 버튼 이벤트 등록
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        GetButton((int)Buttons.EnterButton).gameObject.BindEvent(OnEnterButtonClicked);
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnNextButtonClicked);
        GetButton((int)Buttons.PreviousButton).gameObject.BindEvent(OnPreviousButtonClicked);
        switch (_storyType)
        {

            case Define.MuseumStoryType.Story1919:
                _list = Managers.Data.MuseumStory1919;
                GetTMP((int)Texts.EnterText).GetComponent<TextMeshProUGUI>().text = "준비 중";
                break;
            case Define.MuseumStoryType.Story1443:
                _list = Managers.Data.MuseumStory1443;
                GetTMP((int)Texts.EnterText).GetComponent<TextMeshProUGUI>().text = "준비 중";
                break;
        }
        ShowPage(currentPage);
    }

    // CloseButton 이벤트
    void OnCloseButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        ClosePopupUI();
    }

    // EnterButton 이벤트
    void OnEnterButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        switch (_storyType)
        {
            case Define.MuseumStoryType.Story1592:
                Managers.Scene.LoadScene(Define.Scene.MetaverseWar);
                break;
            case Define.MuseumStoryType.Story1919:
                break;
            case Define.MuseumStoryType.Story1443:
                break;
        }
        // 메타버스 입장
        Debug.Log("메타버스 입장");
    }

    // NextButton 이벤트
    public void OnNextButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        if (currentPage < _list.Count - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    // PreviousButton 이벤트
    public void OnPreviousButtonClicked(PointerEventData data)
    {
        PlayButtonSound();
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    // 페이지에 맞는 히스토리 항목을 표시하는 함수
    private void ShowPage(int pageIndex)
    {
        GetButton((int)Buttons.PreviousButton).gameObject.SetActive(currentPage > 0); // 첫 번째면 Prev 버튼 비활성화
        GetButton((int)Buttons.NextButton).gameObject.SetActive(currentPage < _list.Count - 1); // 마지막이면 Next 버튼 비활성화
        GetTMP((int)Texts.DescriptionText).GetComponent<TextMeshProUGUI>().text = _list[pageIndex];
    }

    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
