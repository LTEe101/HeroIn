using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region User

[Serializable]
public class User
{
    public string userId;
	public string name;
	public string title;
    public Sprite imgNo;
}
#endregion

#region HistoryTitle
[Serializable]
public class HistoryTitle
{
    public string userTitle;
}

[Serializable]
public class HistoryTitleData
{
    public List<HistoryTitle> titles = new List<HistoryTitle>();
}
#endregion

#region 업적 저장 요청 데이터
// 업적 데이터를 담는 클래스
[Serializable]
public class HistoryTitleRequest
{
    public int checkId;
    public string userId;
}
#endregion

#region 유저 업적 변경 요청 데이터
[Serializable]
public class TitlePatchData
{
    public string userId;
    public string title;
}
#endregion

#region 업적 응답 데이터
// 카드 데이터를 저장할 클래스
[Serializable]
public class TitleData
{
    public string userTitle;
    public string createAt;
}

// 전체 카드 리스트
[System.Serializable]
public class TitleListResponse
{
    public List<TitleData> titles;
}
#endregion

#region HistoryCard

[Serializable]
public class HistoryCard
{
	public string name;
	public string description;
	public Sprite imgNO;
}

[Serializable]
public class HistoryCardData
{
    public List<HistoryCard> cards = new List<HistoryCard>();
}

#endregion


#region HistoryCard 응답 데이터
// 카드 데이터를 저장할 클래스
[Serializable]
public class CardData
{
    public string username;           
    public string creationDate;     
    public string cardDescription;   
    public string cardImg;            
    public string cardName;           
}

// 전체 카드 리스트
[System.Serializable]
public class CardListResponse
{
    public List<CardData> cards;
}
#endregion

#region 회원가입 요청 데이터
// JSON 데이터 구조 정의 (회원가입)
[Serializable]
public class JoinData
{
    public string userLoginId;   // 계정 ID
    public string userLoginPw;   // 비밀번호
    public string userName;      // 사용자 이름
}
#endregion

#region 로그인 요청 데이터
// 로그인 요청에 사용되는 데이터 클래스
[Serializable]
public class LoginData
{
    public string userLoginId;   // 계정 ID
    public string userLoginPw;   // 비밀번호
}
#endregion

#region 토큰 응답 데이터
// 토큰 응답에 사용되는 데이터 클래스 (로그인 성공 시 받는 토큰 정보)
[Serializable]
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
}
#endregion

#region 유저 정보 응답 데이터
// 유저 정보 응답에 사용되는 데이터 클래스
[Serializable]
public class UserInfoResponse
{
    public string userName;
    public string img;
    public string title;
}
#endregion

#region 토큰 요청 데이터
[Serializable]
public class TokenRequest
{
    public string grantType;
    public string accessToken;
    public string refreshToken;
}
#endregion

#region Narration
[Serializable]
public class NarrationData
{
    public List<string> beforeStory;  // 시작 전 내레이션 리스트
    public List<string> afterStory;   // 끝난 후 내레이션 리스트
}
#endregion

#region Dialog

[Serializable]
public class Choice
{
    public string text;  // 선택지 텍스트
    public int next;     // 선택 후 연결되는 다음 대사 ID
}


[Serializable]
public class Dialog
{
    public int id;                 // 대사 ID
    public string speaker;         // 대사하는 인물
    public string text;            // 대사 내용
    public List<Choice> choices;   // 선택지 리스트 (선택지가 있을 경우)
    public int next;              // 다음 대사 ID (선택지가 없을 경우)
}

[Serializable]
public class DialogData : ILoader<int, Dialog>
{
    public List<Dialog> dialogs = new List<Dialog>();

    public Dictionary<int, Dialog> MakeDict()
    {
        Dictionary<int, Dialog> dict = new Dictionary<int, Dialog>();
        foreach (Dialog dialog in dialogs)
        {
            dict.Add(dialog.id, dialog);
        }
        return dict;
    }
}
[Serializable]
public class AllScenariosData : ILoader<int, DialogData>
{
    public List<DialogData> scenarios; // 시나리오 리스트

    public Dictionary<int, DialogData> MakeDict()
    {
        Dictionary<int, DialogData> dict = new Dictionary<int, DialogData>();
        for (int i = 0; i < scenarios.Count; i++)
        {
            dict.Add(i + 1, scenarios[i]); // 인덱스를 시나리오 ID로 사용
        }
        return dict;
    }
}
#endregion

#region GameInfo

[Serializable]
public class GameDescription
{
    public string desc;
    public string image;
}

[Serializable]
public class GameInfo
{
    public int id;
    public string title;
    public List<GameDescription> descriptions;
}

[Serializable]
public class GameInfoData : ILoader<int, GameInfo>
{
    public List<GameInfo> infos = new List<GameInfo>();

    public Dictionary<int, GameInfo> MakeDict()
    {
        Dictionary<int, GameInfo> dict = new Dictionary<int, GameInfo>();
        foreach (GameInfo info in infos)
        {
            dict.Add(info.id, info);
        }
        return dict;
    }
}

#endregion
