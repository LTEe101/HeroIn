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
    private Coroutine _typingCoroutine;
    private bool _isTyping;
    private void InitSpeakerCameras()
    {
        _speakerCameras["병사"] = GameObject.Find("SoldierCamera").GetComponent<Camera>();
        _speakerCameras["이순신 장군(나)"] = GameObject.Find("BossCamera").GetComponent<Camera>();
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
            Debug.Log($"Switching to camera for speaker: {speaker}");
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
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        _typingCoroutine = StartCoroutine(TypeTextEffect(dialog.text));

        // speaker에 맞는 카메라로 전환
        SwitchToSpeakerCamera(dialog.speaker);

        textPanel.SetActive(true);
        choicePanel.SetActive(false);

        // TextPanel 클릭 이벤트를 초기화하여 중복 방지
        var textPanelButton = textPanel.GetComponent<Button>();
        if (textPanelButton != null)
        {
            textPanelButton.onClick.RemoveAllListeners(); // 기존 이벤트 제거
        }
        else
        {
            // Button 컴포넌트가 없으면 추가
            textPanelButton = textPanel.AddComponent<Button>();
        }

        if (dialog.choices != null && dialog.choices.Count > 0)
        {

            textPanelButton.onClick.AddListener(() => {
                TextPanelSound();
                HandleTextClick(dialog.text, () => ShowChoices(dialog.choices));
            });
        }
        else
        {
            textPanelButton.onClick.AddListener(() => {
                TextPanelSound();
                HandleTextClick(dialog.text, () => OnDialogClick(dialog.next));
            });
        }
    }
    IEnumerator TypeTextEffect(string fullText)
    {
        _isTyping = true;
        Text storyText = Get<Text>((int)Texts.StoryText).GetComponent<Text>();
        storyText.text = "";  // 텍스트를 비우고 시작

        foreach (char letter in fullText.ToCharArray())
        {
            storyText.text += letter;  // 한 글자씩 추가
            LayoutRebuilder.ForceRebuildLayoutImmediate(storyText.rectTransform);  // 텍스트 레이아웃 강제 업데이트
            yield return new WaitForSeconds(0.04f);  // 글자 간격 조절 (0.1초)

            if (!_isTyping)
                yield break;
        }
        _isTyping = false;
        _typingCoroutine = null;
    }
    private void FinishTyping(string fullText)
    {
        // 타이핑 효과 중인 Coroutine 중지
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        // 전체 텍스트를 한 번에 출력
        Text storyText = Get<Text>((int)Texts.StoryText).GetComponent<Text>();
        storyText.text = fullText;
        _isTyping = false;
    }
    private void HandleTextClick(string fullText, System.Action onTypingComplete)
    {
        if (_isTyping)
        {
            // 타이핑 중이면 전체 텍스트를 한 번에 보여줌
            FinishTyping(fullText);
        }
        else
        {
            // 타이핑이 끝났으면 다음 이벤트 실행
            onTypingComplete.Invoke();
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
                BindEvent(firstChoice.gameObject, (PointerEventData data) => { PlayButtonSound();  OnChoiceSelected(choices[choiceIndex].next); }, Define.UIEvent.Click);
            }
            else if (i == 1)
            {
                secondChoice.gameObject.SetActive(true);
                secondChoice.GetComponentInChildren<Text>().text = choices[i].text;
                BindEvent(secondChoice.gameObject, (PointerEventData data) => { PlayButtonSound(); OnChoiceSelected(choices[choiceIndex].next); }, Define.UIEvent.Click);
            }
        }
    }
    private void PlayButtonSound()
    {
        Managers.Sound.Play("ProEffect/User_Interface_Menu/ui_menu_button_keystroke_01", Define.Sound.Effect, 0.2f);
    }
    private void TextPanelSound()
    {
        Managers.Sound.Play("ProEffect/User_Interface_Menu/ui_button_simple_click_03", Define.Sound.Effect, 0.1f);
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
            switch (_scenarioNumber)
            {
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
                    Managers.Scene.LoadScene(Define.Scene.ShipView);
                    break;
                case 5:
                    Managers.Scene.LoadScene(Define.Scene.MotionGame);
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
