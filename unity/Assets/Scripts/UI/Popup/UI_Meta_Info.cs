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
    private int _currentMetaIndex = 0;           // ���� ������ MetaHistory �ε���
    public void InitMetaHistory(int index)
    {
        // MetaHistoryDatas���� ������ �ε����� �ش��ϴ� MetaHistory�� History ����Ʈ�� ������ _historyList�� ����
        if (Managers.Data.MetaHistoryDatas.TryGetValue(index, out MetaHistory metaHistory))
        {
            _historyList = metaHistory.history; // MetaHistory �ȿ� �ִ� History ����Ʈ�� ����
        }
        else
        {
            Debug.LogError("MetaHistory �����͸� ã�� �� �����ϴ�.");
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
            History currentHistory = _historyList[_currentMetaIndex]; // ���� �ε����� �ش��ϴ� History ��ü ��������
            GetText((int)Texts.TitleText).text = currentHistory.title;
            GetText((int)Texts.DescText).text = currentHistory.desc;
        }
    }
    // ��ư ���¸� ������Ʈ�ϴ� �Լ� (ó���� ���������� Prev/Next ��ư�� ����)
    private void UpdateButtons()
    {
        GetButton((int)Buttons.PrevButton).gameObject.SetActive(_currentMetaIndex > 0); // ù ��°�� Prev ��ư ��Ȱ��ȭ
        GetButton((int)Buttons.NextButton).gameObject.SetActive(_currentMetaIndex < _historyList.Count - 1); // �������̸� Next ��ư ��Ȱ��ȭ
    }

    // ���� MetaHistory�� ǥ���ϴ� �Լ�
    private void ShowNextMeta()
    {
        if (_currentMetaIndex < _historyList.Count - 1)
        {
            _currentMetaIndex++;
            UpdateMetaHistory();
            UpdateButtons();
        }
    }

    // ���� MetaHistory�� ǥ���ϴ� �Լ�
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
