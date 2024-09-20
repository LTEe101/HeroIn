using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region User

[Serializable]
public class User
{
	public string name;
	public string title;
	public string imgNo;
}
#endregion

#region HistoryCard

[Serializable]
public class HistoryCard
{
	public int historyId;
	public string name;
	public string description;
	public string imgNO;
}

[Serializable]
public class HistoryCardData : ILoader<int, HistoryCard>
{
    public List<HistoryCard> cards = new List<HistoryCard>();

    public Dictionary<int, HistoryCard> MakeDict()
    {
        Dictionary<int, HistoryCard> dict = new Dictionary<int, HistoryCard>();
        foreach (HistoryCard card in cards)
            dict.Add(card.historyId, card);
        return dict;
    }
}

#endregion

