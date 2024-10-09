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
        NameText,
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
            case Define.MuseumStoryType.Story1592:
                _list = Managers.Data.MuseumStory1592;

                for (int i = 0; i < _list.Count; i++)
                {
                    // 이순신 장군에 색상과 볼드 적용
                    _list[i] = _list[i].Replace("이순신 장군", "<color=#1E90FF><b>이순신 장군</b></color>");

                    // 거북선에 색상과 볼드 적용
                    _list[i] = _list[i].Replace("거북선", "<color=#1E90FF><b>거북선</b></color>");

                    // 학익진에 색상과 볼드 적용
                    _list[i] = _list[i].Replace("학익진", "<color=#1E90FF><b>학익진</b></color>");
                }

                GetTMP((int)Texts.NameText).GetComponent<TextMeshProUGUI>().text = "한산도 대첩";
                break;
            case Define.MuseumStoryType.Story1919:
                _list = Managers.Data.MuseumStory1919;
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i] = _list[i].Replace("일제강점기", "<color=#1E90FF><b>일제강점기</b></color>");
                    _list[i] = _list[i].Replace("유관순 열사", "<color=#1E90FF><b>유관순 열사</b></color>");
                    _list[i] = _list[i].Replace("안중근 의사", "<color=#1E90FF><b>안중근 의사</b></color>");
                    _list[i] = _list[i].Replace("김구 선생님", "<color=#1E90FF><b>김구 선생님</b></color>");
                    _list[i] = _list[i].Replace("독립운동", "<color=#1E90FF><b>독립운동</b></color>");
                    _list[i] = _list[i].Replace("대한독립 만세!", "<color=#1E90FF><b>대한독립 만세!</b></color>");
                    _list[i] = _list[i].Replace("광복절", "<color=#1E90FF><b>광복절</b></color>");
                }
                GetTMP((int)Texts.NameText).GetComponent<TextMeshProUGUI>().text = "3.1운동";
                GetTMP((int)Texts.EnterText).GetComponent<TextMeshProUGUI>().text = "준비 중";
                break;
            case Define.MuseumStoryType.Story1443:
                _list = Managers.Data.MuseumStory1443;
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i] = _list[i].Replace("세종대왕", "<color=#1E90FF><b>세종대왕</b></color>");
                    _list[i] = _list[i].Replace("측우기", "<color=#1E90FF><b>측우기</b></color>");
                    _list[i] = _list[i].Replace("해시계", "<color=#1E90FF><b>해시계</b></color>");
                    _list[i] = _list[i].Replace("물시계", "<color=#1E90FF><b>물시계</b></color>");
                    _list[i] = _list[i].Replace("한글", "<color=#1E90FF><b>한글</b></color>");
                }
                GetTMP((int)Texts.NameText).GetComponent<TextMeshProUGUI>().text = "훈민정음";
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
