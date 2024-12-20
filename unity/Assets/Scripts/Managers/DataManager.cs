﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public User userInfo { get; private set; } = new User();
    public bool ControlPannel { get; set; } = false;
    public List<HistoryCard> cards = new List<HistoryCard>();
    public List<HistoryTitle> titles = new List<HistoryTitle>();
    public List<string> BeforeStoryList { get; private set; } = new List<string>();
    public List<string> AfterStoryList { get; private set; } = new List<string>();
    public Dictionary<int, List<Dialog>> DialogDataMap { get; private set; } = new Dictionary<int, List<Dialog>>();
    public Dictionary<int, GameInfo> GameInfos { get; private set; } = new Dictionary<int, GameInfo>();
    public List<QuizQuestion> QuizQuestions { get; private set; } = new List<QuizQuestion>();
    public Dictionary<int, MetaHistory> MetaHistoryDatas { get; private set; } = new Dictionary<int, MetaHistory>();
    public List<string> MuseumStory1592 { get; private set; } = new List<string>();
    public List<string> MuseumStory1919 { get; private set; } = new List<string>();
    public List<string> MuseumStory1443 { get; private set; } = new List<string>();
    public void Init()
    {
        NarrationData narrationData = LoadJson<NarrationData>("NarrationData");
        GameInfos = LoadJson<GameInfoData>("GameInfoData").MakeDict();
        MetaHistoryDatas = LoadJson<MetaHistoryData>("MetaHistoryData").MakeDict();
        StoryData storyData = LoadJson<StoryData>("StoryData");

        BeforeStoryList = narrationData.beforeStory;
        AfterStoryList = narrationData.afterStory;
        MuseumStory1592 = storyData.story1592;
        MuseumStory1919 = storyData.story1919;
        MuseumStory1443 = storyData.story1443;

        LoadAllScenarios("DialogsData");
        LoadGameInfos("GameInfoData");
        LoadQuizData("QuizData");
    }
    public void SetUserInfo(User user)
    {
        userInfo = user;
    }

    T LoadJson<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        if (textAsset == null)
        {
            Debug.LogError($"Failed to load: Data/{path}");
            return default(T);
        }
        return JsonUtility.FromJson<T>(textAsset.text);
    }
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
     private void LoadGameInfos(string path)
     {
         var gameInfoData = LoadJson<GameInfoData>(path);
         if (gameInfoData != null)
         {
             GameInfos = gameInfoData.MakeDict();
         }
     }
    // 퀴즈 데이터를 로드하는 메서드
    private void LoadQuizData(string path)
    {
        QuizQuestionList quizQuestionList = LoadJson<QuizQuestionList>(path);
        if (quizQuestionList != null && quizQuestionList.questions != null)
        {
            QuizQuestions = quizQuestionList.questions;
            Debug.Log($"Loaded {QuizQuestions.Count} quiz questions.");
        }
        else
        {
            Debug.LogError("Failed to load quiz questions or quiz data is empty.");
        }
    }
    private void LoadAllScenarios(string path)
 {
     var allScenarios = LoadJson<AllScenariosData>(path);
     if (allScenarios != null && allScenarios.scenarios != null)
     {
         for (int i = 0; i < allScenarios.scenarios.Count; i++)
         {
             DialogDataMap[i + 1] = allScenarios.scenarios[i].dialogs; // 각 시나리오 ID에 맞게 저장
         }
     }
 }
 
}
