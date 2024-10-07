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
    private List<Dialog> _currentDialogList;  // ���� ��� ����Ʈ
    private int _currentDialogIndex;  // ���� ��� �ε���
    private int _scenarioNumber;
    private Dictionary<string, Camera> _speakerCameras = new Dictionary<string, Camera>();
    private Coroutine _typingCoroutine;
    private bool _isTyping;
    private void InitSpeakerCameras()
    {
        _speakerCameras["����"] = GameObject.Find("SoldierCamera").GetComponent<Camera>();
        _speakerCameras["�̼��� �屺(��)"] = GameObject.Find("BossCamera").GetComponent<Camera>();
        _speakerCameras["���� ���ְ�"] = GameObject.Find("ArmyCamera").GetComponent<Camera>();
    }
    private void SwitchToSpeakerCamera(string speaker)
    {
        // ��� ī�޶� ��Ȱ��ȭ
        foreach (var camera in _speakerCameras.Values)
        {
            camera.enabled = false;
        }

        // speaker�� �´� ī�޶� Ȱ��ȭ
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
        // DataManager���� �ó������� �ش��ϴ� ��ȭ �����͸� �ε�
        if (Managers.Data.DialogDataMap.TryGetValue(scenarioId, out var dialogList))
        {
            _currentDialogList = dialogList;  // ���� ��� ����Ʈ ����
            _currentDialogIndex = 0;  // �ʱ�ȭ
            DisplayDialog(_currentDialogList[_currentDialogIndex]);  // ù ��° ������ ����
        }
        else
        {
            Debug.LogError($"Dialog scenario {scenarioId} not found!");
        }
    }

    // ��縦 UI�� ǥ���ϴ� �Լ�
    private void DisplayDialog(Dialog dialog)
    {
        Get<Text>((int)Texts.SpeakerText).GetComponent<Text>().text = dialog.speaker;
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        _typingCoroutine = StartCoroutine(TypeTextEffect(dialog.text));

        // speaker�� �´� ī�޶�� ��ȯ
        SwitchToSpeakerCamera(dialog.speaker);

        textPanel.SetActive(true);
        choicePanel.SetActive(false);

        // TextPanel Ŭ�� �̺�Ʈ�� �ʱ�ȭ�Ͽ� �ߺ� ����
        var textPanelButton = textPanel.GetComponent<Button>();
        if (textPanelButton != null)
        {
            textPanelButton.onClick.RemoveAllListeners(); // ���� �̺�Ʈ ����
        }
        else
        {
            // Button ������Ʈ�� ������ �߰�
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
        storyText.text = "";  // �ؽ�Ʈ�� ���� ����

        foreach (char letter in fullText.ToCharArray())
        {
            storyText.text += letter;  // �� ���ھ� �߰�
            LayoutRebuilder.ForceRebuildLayoutImmediate(storyText.rectTransform);  // �ؽ�Ʈ ���̾ƿ� ���� ������Ʈ
            yield return new WaitForSeconds(0.04f);  // ���� ���� ���� (0.1��)

            if (!_isTyping)
                yield break;
        }
        _isTyping = false;
        _typingCoroutine = null;
    }
    private void FinishTyping(string fullText)
    {
        // Ÿ���� ȿ�� ���� Coroutine ����
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }

        // ��ü �ؽ�Ʈ�� �� ���� ���
        Text storyText = Get<Text>((int)Texts.StoryText).GetComponent<Text>();
        storyText.text = fullText;
        _isTyping = false;
    }
    private void HandleTextClick(string fullText, System.Action onTypingComplete)
    {
        if (_isTyping)
        {
            // Ÿ���� ���̸� ��ü �ؽ�Ʈ�� �� ���� ������
            FinishTyping(fullText);
        }
        else
        {
            // Ÿ������ �������� ���� �̺�Ʈ ����
            onTypingComplete.Invoke();
        }
    }
    private void ShowChoices(List<Choice> choices)
    {
        textPanel.SetActive(false);  // ��ȭ �г��� ����
        choicePanel.SetActive(true);  // ������ �г��� ����

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
        // ���� ���� �̵�
        if (dialogId > 0 && dialogId <= _currentDialogList.Count)
        {
            _currentDialogIndex = dialogId - 1;  // ����Ʈ�� 0���� �����ϹǷ� -1
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
