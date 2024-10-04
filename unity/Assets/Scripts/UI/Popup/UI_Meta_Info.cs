using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;
public class UI_Meta_Info : UI_Popup
{
    enum Texts
    {
        DescText,
        TitleText,
    }
    enum Buttons
    {
        PrevButton,
        NextButton,
    }
    private List<History> _historyList;
    private int _currentMetaIndex = 0;           // 현재 보여줄 MetaHistory 인덱스
    public void InitMetaHistory(int index)
    {
        // MetaHistoryDatas에서 지정한 인덱스에 해당하는 MetaHistory의 History 리스트를 가져와 _historyList에 저장
        if (Managers.Data.MetaHistoryDatas.TryGetValue(index, out MetaHistory metaHistory))
        {
            _historyList = metaHistory.history; // MetaHistory 안에 있는 History 리스트를 저장
        }
        else
        {
            Debug.LogError("MetaHistory 데이터를 찾을 수 없습니다.");
            return;
        }


        Init();

        UpdateMetaHistory();
        UpdateButtons();
    }
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));


        BindEvent(GetButton((int)Buttons.PrevButton).gameObject, (PointerEventData data) => { PlayButtonSound();  ShowPreviousMeta(); }, Define.UIEvent.Click);
        BindEvent(GetButton((int)Buttons.NextButton).gameObject, (PointerEventData data) => { PlayButtonSound();  ShowNextMeta(); }, Define.UIEvent.Click);

        UpdateMetaHistory();
        UpdateButtons();
    }
    private void UpdateMetaHistory()
    {
        if (_currentMetaIndex < _historyList.Count)
        {
            History currentHistory = _historyList[_currentMetaIndex]; // 현재 인덱스에 해당하는 History 객체 가져오기
            GetText((int)Texts.TitleText).text = currentHistory.title;
            GetText((int)Texts.DescText).text = currentHistory.desc;
        }
    }
    // 버튼 상태를 업데이트하는 함수 (처음과 마지막에는 Prev/Next 버튼을 숨김)
    private void UpdateButtons()
    {
        GetButton((int)Buttons.PrevButton).gameObject.SetActive(_currentMetaIndex > 0); // 첫 번째면 Prev 버튼 비활성화
        GetButton((int)Buttons.NextButton).gameObject.SetActive(_currentMetaIndex < _historyList.Count - 1); // 마지막이면 Next 버튼 비활성화
    }

    // 다음 MetaHistory를 표시하는 함수
    private void ShowNextMeta()
    {
        if (_currentMetaIndex < _historyList.Count - 1)
        {
            _currentMetaIndex++;
            UpdateMetaHistory();
            UpdateButtons();
        }
    }

    // 이전 MetaHistory를 표시하는 함수
    private void ShowPreviousMeta()
    {
        if (_currentMetaIndex > 0)
        {
            _currentMetaIndex--;
            UpdateMetaHistory();
            UpdateButtons();
        }
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("ProEffect/User_Interface_Menu/ui_menu_button_keystroke_01", Define.Sound.Effect, 0.2f);
    }
}
