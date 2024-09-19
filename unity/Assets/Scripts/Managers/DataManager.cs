using System;
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
    public Dictionary<int, HistoryCard> CardDict { get; private set; } = new Dictionary<int, HistoryCard>();
    public void Init()
    {
        CardDict = LoadJson<HistoryCardData, int, HistoryCard>("HistoryCardData").MakeDict();
     
    }
    public void SetUserInfo(User user)
    {
        userInfo = user;
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
