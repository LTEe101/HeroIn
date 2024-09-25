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