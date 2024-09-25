using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Story : UI_Popup
{
    enum Texts
    {
        StoryText,
        SpeakerText,
    }

    enum Buttons
    {
        FirstChoice,
        SecondChoice,
    }
    enum GameObjects
    {
        ChoicePanel,
        TextPanel
    }

    public GameObject choicePanel, textPanel;
    public Button firstChoice, secondChoice;
    private List<Dialog> _currentDialogList;  // 현재 대사 리스트
    private int _currentDialogIndex;  // 현재 대사 인덱스
    private int _scenarioNumber;
    private Dictionary<string, Camera> _speakerCameras = new Dictionary<string, Camera>();
    private void InitSpeakerCameras()
    {
        _speakerCameras["병사"] = GameObject.Find("SoldierCamera").GetComponent<Camera>();
        _speakerCameras["이순신 장군"] = GameObject.Find("BossCamera").GetComponent<Camera>();
        _speakerCameras["전투 지휘관"] = GameObject.Find("ArmyCamera").GetComponent<Camera>();
    }
    private void SwitchToSpeakerCamera(string speaker)
    {
        // 모든 카메라를 비활성화
        foreach (var camera in _speakerCameras.Values)
        {
            camera.enabled = false;
        }

        // speaker에 맞는 카메라만 활성화
        if (_speakerCameras.ContainsKey(speaker))
        {
            _speakerCameras[speaker].enabled = true;
        }
        else
        {
            Debug.LogWarning($"Speaker {speaker} does not have a corresponding Camera.");
        }
    }

    public void LoadDialogs(int scenarioId)
    {
        Init();
        _scenarioNumber = scenarioId;
        // DataManager에서 시나리오에 해당하는 대화 데이터를 로드
        if (Managers.Data.DialogDataMap.TryGetValue(scenarioId, out var dialogList))
        {
            _currentDialogList = dialogList;  // 현재 대사 리스트 저장
            _currentDialogIndex = 0;  // 초기화
            DisplayDialog(_currentDialogList[_currentDialogIndex]);  // 첫 번째 대사부터 시작
        }
        else
        {
            Debug.LogError($"Dialog scenario {scenarioId} not found!");
        }
    }

    // 대사를 UI에 표시하는 함수
    private void DisplayDialog(Dialog dialog)
    {
        Get<Text>((int)Texts.SpeakerText).GetComponent<Text>().text = dialog.speaker;
        Get<Text>((int)Texts.StoryText).GetComponent<Text>().text = dialog.text;

        // speaker에 맞는 카메라로 전환
        SwitchToSpeakerCamera(dialog.speaker);

        textPanel.SetActive(true);
        choicePanel.SetActive(false);
        if (dialog.choices != null && dialog.choices.Count > 0)
        {

            BindEvent(textPanel, (PointerEventData data) => { ShowChoices(dialog.choices); }, Define.UIEvent.Click);
        }
        else
        {
            BindEvent(textPanel, (PointerEventData data) => { OnDialogClick(dialog.next); }, Define.UIEvent.Click);
        }
    }

    private void ShowChoices(List<Choice> choices)
    {
        textPanel.SetActive(false);  // 대화 패널을 숨김
        choicePanel.SetActive(true);  // 선택지 패널을 보임

        for (int i = 0; i < choices.Count; i++)
        {
            int choiceIndex = i;
            if (i == 0)
            {
                firstChoice.gameObject.SetActive(true);
                firstChoice.GetComponentInChildren<Text>().text = choices[i].text;
                BindEvent(firstChoice.gameObject, (PointerEventData data) => { OnChoiceSelected(choices[choiceIndex].next); }, Define.UIEvent.Click);
            }
            else if (i == 1)
            {
                secondChoice.gameObject.SetActive(true);
                secondChoice.GetComponentInChildren<Text>().text = choices[i].text;
                BindEvent(secondChoice.gameObject, (PointerEventData data) => { OnChoiceSelected(choices[choiceIndex].next); }, Define.UIEvent.Click);
            }
        }
    }
    public void OnChoiceSelected(int nextDialogId)
    {
        choicePanel.SetActive(false);
        StartDialog(nextDialogId);
    }

    private void OnDialogClick(int nextDialogId)
    {
        if (nextDialogId == 0)
        {
            switch(_scenarioNumber){
                case 1:
                    Managers.Scene.LoadScene(Define.Scene.StoryTwo);
                    break;
                case 2:
                    Managers.Scene.LoadScene(Define.Scene.StoryThree);
                    break;
                case 3:
                    Managers.Scene.LoadScene(Define.Scene.GameOne);
                    break;
                case 4:
                    Managers.Scene.LoadScene(Define.Scene.StoryFive);
                    break;
                case 5:
                    Managers.Scene.LoadScene(Define.Scene.StorySix);
                    break;
                case 6:
                    Managers.Scene.LoadScene(Define.Scene.AfterStory);
                    break;
            }
        }
        else
        {
            StartDialog(nextDialogId);
        } 
    }

    private void StartDialog(int dialogId)
    {
        // 다음 대사로 이동
        if (dialogId > 0 && dialogId <= _currentDialogList.Count)
        {
            _currentDialogIndex = dialogId - 1;  // 리스트는 0부터 시작하므로 -1
            DisplayDialog(_currentDialogList[_currentDialogIndex]);
        }
    }

    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        textPanel = GetObject((int)GameObjects.TextPanel);
        choicePanel = GetObject((int)GameObjects.ChoicePanel);
        firstChoice = GetButton((int)Buttons.FirstChoice);
        secondChoice = GetButton((int)Buttons.SecondChoice);

        choicePanel.SetActive(false);
        InitSpeakerCameras();
    }
} 
