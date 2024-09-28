using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_History : UI_Popup
{
    private List<HistoryTitle> historyList;
    public int itemsPerPage = 3;  // 한 페이지에 보여줄 항목 수
    private int currentPage = 0;  // 현재 페이지 인덱스
    private int totalPages;       // 전체 페이지 수

    enum Buttons
    {
        NextButton,
        PreviousButton,
        CloseButton
    }
    enum GameObjects
    {
        Items
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        historyList = Managers.Data.titles;
        // 버튼 바인드
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        // 버튼 이벤트 등록
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnNextButtonClicked);
        GetButton((int)Buttons.PreviousButton).gameObject.BindEvent(OnPreviousButtonClicked);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnCloseButtonClicked);
        

        // 전체 페이지 수 계산
        totalPages = Mathf.CeilToInt((float)historyList.Count / itemsPerPage);

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
        if(currentPage < totalPages - 1)
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

        GameObject gridPanel = Get<GameObject>((int)GameObjects.Items);
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        // 현재 페이지에 맞는 히스토리 항목을 생성
        int startItem = pageIndex * itemsPerPage;
        int endItem = Mathf.Min(startItem + itemsPerPage, historyList.Count);

        float yOffset = 13f; // Y축 간격
        Vector2 startPosition = new Vector2(32, 30); // 초기 위치 설정

        for (int i = startItem; i < endItem; i++)
        {
            GameObject item = Managers.UI.MakeSubItem<UI_History_Item>(gridPanel.transform).gameObject;
            UI_History_Item historyItem = item.GetOrAddComponent<UI_History_Item>();
            historyItem.SetInfo(historyList[i].userTitle, this);

            // RectTransform을 통해 초기 위치와 Y축 간격 설정
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startPosition.x, startPosition.y - (i - startItem) * yOffset); // 초기 위치에서 Y축 간격 설정
        }
    }

}