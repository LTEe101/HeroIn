using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Game_Desc : UI_Popup
{
    enum Texts
    {
        TitleText,
        DescText,
    }

    enum Buttons
    {
        StartButton,
        NextButton,
        PrevButton,
    }
    enum Images
    {
        DescPaper,
        GameScenePhoto,
    }

    private int currentIndex = 0;
    private GameInfo currentGameInfo;
    public event Action OnStartGame;

    public override void Init()
    {
        Debug.Log("UI_Game_Desc Init() called");
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        Debug.Log($"TitleText bound: {Get<Text>((int)Texts.TitleText) != null}");
        Debug.Log($"DescText bound: {Get<Text>((int)Texts.DescText) != null}");
        Debug.Log($"GameScenePhoto bound: {Get<Image>((int)Images.GameScenePhoto) != null}");

        BindEvent(GetButton((int)Buttons.StartButton).gameObject, OnStartButtonClicked);
        BindEvent(GetButton((int)Buttons.NextButton).gameObject, OnNextButtonClicked);
        BindEvent(GetButton((int)Buttons.PrevButton).gameObject, OnPrevButtonClicked);
    }
    public void LoadInfos(int gameId)
    {
        Debug.Log($"LoadInfos called with gameId: {gameId}");
        if (Managers.Data.GameInfos.TryGetValue(gameId, out currentGameInfo))
        {
            Debug.Log($"GameInfo found: {currentGameInfo.title}");
            Init();
            currentIndex = 0;
            UpdateContent();
        }
        else
        {
            Debug.LogWarning($"GameInfo with ID {gameId} not found.");
        }
    }

    private void UpdateContent()
    {
        if (currentGameInfo == null)
        {
            Debug.LogError("currentGameInfo is null");
            return;
        }

        Debug.Log($"Updating content for game: {currentGameInfo.title}, Description count: {currentGameInfo.descriptions.Count}");

        if (currentIndex >= 0 && currentIndex < currentGameInfo.descriptions.Count)
        {
            GameDescription currentDesc = currentGameInfo.descriptions[currentIndex];

            Text titleText = Get<Text>((int)Texts.TitleText);
            Text descText = Get<Text>((int)Texts.DescText);
            Image gameScenePhoto = Get<Image>((int)Images.GameScenePhoto);

            Debug.Log($"TitleText: {titleText != null}, DescText: {descText != null}, GameScenePhoto: {gameScenePhoto != null}");

            if (titleText != null && descText != null && gameScenePhoto != null)
            {
                titleText.text = currentGameInfo.title;
                descText.text = currentDesc.desc;

                if (!string.IsNullOrEmpty(currentDesc.image))
                {
                    Sprite sprite = Resources.Load<Sprite>(currentDesc.image);
                    if (sprite != null)
                    {
                        gameScenePhoto.sprite = sprite;
                        gameScenePhoto.gameObject.SetActive(true);  // 이미지 표시
                    }
                    else
                    {
                        gameScenePhoto.gameObject.SetActive(false); // 이미지 숨김
                    }
                }
                else
                {
                    gameScenePhoto.gameObject.SetActive(false);     // 이미지 숨김
                }

                GetButton((int)Buttons.PrevButton).gameObject.SetActive(currentIndex > 0);
                GetButton((int)Buttons.NextButton).gameObject.SetActive(currentIndex < currentGameInfo.descriptions.Count - 1);
                GetButton((int)Buttons.StartButton).gameObject.SetActive(currentIndex == currentGameInfo.descriptions.Count - 1);
            }
            else
            {
                Debug.LogError("One or more UI components are null");
            }
        }
        else
        {
            Debug.LogError($"Invalid currentIndex: {currentIndex}");
        }
    }

    private void OnStartButtonClicked(PointerEventData data)
    {
        if (currentGameInfo != null && currentGameInfo.id == 2)
        {
            OnStartGame?.Invoke();  // 게임 시작 이벤트 발생 (gameId가 2일 때만)
        }

        ClosePopupUI();
        Managers.UI.ShowSceneUI<UI_Game_Score>();
    }
    private void OnNextButtonClicked(PointerEventData data)
    {
        if (currentIndex < currentGameInfo.descriptions.Count - 1)
        {
            currentIndex++;
            UpdateContent();
        }
    }

    private void OnPrevButtonClicked(PointerEventData data)
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateContent();
        }
    }
}