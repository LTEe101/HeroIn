using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_History_Book: UI_Popup
{
    public int itemsPerPage = 1;  // 한 페이지에 보여줄 항목 수
    private int currentPage = 0;  // 현재 페이지 인덱스
    private int totalPages;       // 전체 페이지 수

    enum Buttons
    {
        NextButton,
        PreviousButton,
        CloseButton
    }

    enum Texts
    {
        HistoricalFigureNameText,
        DescriptionText,
    }

    enum Images
    {
        HistoricalFigureImg,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        // 버튼 이벤트 등록
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnNextButtonClicked);
        GetButton((int)Buttons.PreviousButton).gameObject.BindEvent(OnPreviousButtonClicked);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);

        // 전체 페이지 수 계산
        totalPages = Managers.Data.cards.Count;

        // 첫 페이지 표시
        ShowPage(currentPage);

    }

    // CloseButton 이벤트
    public void OnCloseButtonClicked(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }

    // NextButton 이벤트
    public void OnNextButtonClicked(PointerEventData data)
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    // PreviousButton 이벤트
    public void OnPreviousButtonClicked(PointerEventData data)
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    // 페이지에 맞는 히스토리 항목을 표시하는 함수
    private void ShowPage(int pageIndex)
    {
        Get<Text>((int)Texts.HistoricalFigureNameText).GetComponent<Text>().text = Managers.Data.cards[pageIndex].name;
        Get<Text>((int)Texts.DescriptionText).GetComponent<Text>().text = Managers.Data.cards[pageIndex].description;
        Get<Image>((int)Images.HistoricalFigureImg).sprite = Managers.Data.cards[pageIndex].imgNO;
    }

}