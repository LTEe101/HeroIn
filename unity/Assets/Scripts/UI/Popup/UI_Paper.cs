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
private List<string> _narrationList;  // ���� ��� ���� �����̼� ����Ʈ
private int _currentNarrationIndex = 0; // ���� ������ �����̼� �ε���

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

        CursorEventHelper.Initialize();

        // PrevButton Ŭ�� �� ���� �����̼� ǥ��
        BindEvent(GetButton((int)Buttons.PrevButton).gameObject, (PointerEventData data) => { ShowPreviousNarration(); }, Define.UIEvent.Click);
        CursorEventHelper.AddCursorChangeEvents(GetButton((int)Buttons.PrevButton).gameObject);

        // NextButton Ŭ�� �� ���� �����̼� ǥ��
        BindEvent(GetButton((int)Buttons.NextButton).gameObject, (PointerEventData data) => { ShowNextNarration(); }, Define.UIEvent.Click);
        CursorEventHelper.AddCursorChangeEvents(GetButton((int)Buttons.NextButton).gameObject);

        BindEvent(GetButton((int)Buttons.StartButton).gameObject, (PointerEventData data) => { ShowNextNarration(); }, Define.UIEvent.Click);
        CursorEventHelper.AddCursorChangeEvents(GetButton((int)Buttons.StartButton).gameObject);
    }
    private void Update()
    {
        CursorEventHelper.UpdateCursor();
    }

    // �����̼��� ������Ʈ�ϴ� �Լ�
    private void UpdateNarration()
    {
        if (_currentNarrationIndex < _narrationList.Count)
        {
            GetText((int)Texts.DescText).text = _narrationList[_currentNarrationIndex];
        }
    }
    // ��ư ���¸� ������Ʈ�ϴ� �Լ� (ó���� ���������� Prev/Next ��ư�� ����)
    private void UpdateButtons()
    {
        GetButton((int)Buttons.PrevButton).gameObject.SetActive(_currentNarrationIndex > 0); // ù ��°�� Prev ��ư ��Ȱ��ȭ
        GetButton((int)Buttons.NextButton).gameObject.SetActive(_currentNarrationIndex < _narrationList.Count - 1); // �������̸� Next ��ư ��Ȱ��ȭ
        GetButton((int)Buttons.StartButton).gameObject.SetActive(_currentNarrationIndex == _narrationList.Count - 1); // �������̸� Start ��ư Ȱ��ȭ
    }

    // ���� �����̼��� ǥ���ϴ� �Լ�
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
                // Ÿ�Ӹӽ����� ���ư���
                Managers.Scene.LodingLoadScene(Define.Scene.Home);
            }
        }
    }
    // ���� �����̼��� ǥ���ϴ� �Լ�
    private void ShowPreviousNarration()
    {
        if (_currentNarrationIndex > 0)
        {
            _currentNarrationIndex--;
            UpdateButtons();
            UpdateNarration();
        }
    }
}

