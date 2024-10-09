using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://j11e101.p.ssafy.io/api/";

    // 회원가입 요청을 보내는 함수 (이미지와 JSON 데이터 전송)
    public IEnumerator Join(string userName, string userLoginId, string userLoginPw, Image userImage, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON 데이터 생성
        JoinData userDto = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };
        Texture2D texture;
        // JSON 형식으로 변환
        string jsonData = JsonUtility.ToJson(userDto);
        if (userImage.sprite.name == "UI_Icon_Plus")
        {
            // Resources 폴더에서 이미지 로드
            texture = Resources.Load<Sprite>("Art/Image/Logo").texture;
        }
        else
        {
            texture = userImage.sprite.texture;
        }
           
        byte[] imageBytes = texture.EncodeToPNG();

        // 멀티파트 폼 데이터 생성
        WWWForm form = new WWWForm();

        // JSON 데이터를 텍스트 필드로 추가 (서버에서 필드명 "userDto"로 받아야 함)
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        form.AddBinaryData("userDto", jsonBytes, "userDto.json", "application/json");

        // 이미지 파일 추가 (바이트 배열)
        if (imageBytes != null)
        {
            form.AddBinaryData("userImg", imageBytes, "userImage.png", "image/png");
        }

        // UnityWebRequest로 멀티파트 폼 데이터 전송
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "user/sign-up", form))
        {
            // 요청 전송 및 응답 대기
            yield return www.SendWebRequest();

            // 오류 처리
            if (www.result != UnityWebRequest.Result.Success)
            {
                // 요청 실패 처리
                Debug.LogError("요청 실패: " + www.downloadHandler.text);
                onError?.Invoke(www.downloadHandler.text);
            }
            else
            {
                // 요청 성공 처리
                onSuccess?.Invoke(www.downloadHandler.text);
            }
        }
    }



    // 로그인 요청을 보내는 함수 (POST)
    public IEnumerator Login(string userLoginId, string userLoginPw, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON 데이터 생성
        LoginData loginData = new LoginData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw
        };

        string jsonData = JsonUtility.ToJson(loginData); // JSON 형식으로 변환

        // POST 요청
        yield return SendRequest(apiUrl + "user/login", "POST", jsonData, (response) =>
        {
            // 로그인 성공 후 토큰 저장
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(response);

            if (tokenResponse != null)
            {
                // 토큰 저장 (PlayerPrefs에 저장)
                PlayerPrefs.SetString("accessToken", tokenResponse.accessToken);
                PlayerPrefs.SetString("refreshToken", tokenResponse.refreshToken);
                PlayerPrefs.Save(); // 저장
                User user = new User
                {
                    userId = userLoginId
                };
                // 유저 정보 저장
                Managers.Data.SetUserInfo(user);
                Debug.Log("로그인 성공, 토큰 저장 완료");
            }
            
            // 성공 콜백 호출
            onSuccess?.Invoke(response);

        }, onError);
    }

    // 유저 정보 요청 (POST) - JWT 토큰으로 유저 정보 가져오기
    public IEnumerator GetUserInfo(System.Action<User> onSuccess, System.Action<string> onError)
    {
        // JWT 토큰 가져오기
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string refreshToken = PlayerPrefs.GetString("refreshToken", "");

        // 유효한 토큰이 있는지 확인
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke("토큰이 없습니다.");
            yield break;
        }

        // 요청 데이터 생성
        TokenRequest tokenRequest = new TokenRequest
        {
            grantType = "Bearer",
            accessToken = accessToken,
            refreshToken = refreshToken
        };

        // JSON 데이터로 직렬화
        string jsonData = JsonUtility.ToJson(tokenRequest);

        // POST 요청 보내기
        yield return SendRequest(apiUrl + "user/profile", "POST", jsonData, (response) =>
        {
            // 서버로부터 받은 유저 정보를 파싱
            UserInfoResponse userInfoResponse = JsonUtility.FromJson<UserInfoResponse>(response);

            if (userInfoResponse != null)
            {
                Debug.Log("유저 정보 가져오기 성공: " + userInfoResponse.userName);

                // 유저 정보를 User 객체로 변환
                User user = new User
                {
                    userId = Managers.Data.userInfo.userId,
                    name = userInfoResponse.userName,
                    title = userInfoResponse.title
                };

                // 이미지 URL로부터 이미지를 다운로드해서 Sprite로 변환 후 User 객체에 설정
                StartCoroutine(LoadImageFromUrl(userInfoResponse.img, (sprite) =>
                {
                    user.imgNo = sprite;
                    onSuccess?.Invoke(user);
                }, onError));
            }
            else
            {
                onError?.Invoke("유저 정보 파싱 실패");
            }

        }, onError);
    }

    // 카드 데이터를 받아와 처리하는 메서드
    public IEnumerator GetUserCardInfo(string userId, System.Action onSuccess, System.Action<string> onError)
    {
        // GET 요청 URL에 userId를 쿼리 파라미터로 추가
        string endpoint = $"{apiUrl}user/card?userId={userId}";

        // 서버에 GET 요청 보내기
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<CardData> cardList = null;
                try
                {
                    // 서버에서 받은 JSON 데이터를 리스트로 파싱
                    cardList = JsonHelper.FromJson<CardData>(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("카드 정보 파싱 실패: " + ex.Message);
                    onError?.Invoke("카드 정보 파싱 실패: " + ex.Message);
                }
                Managers.Data.cards = new List<HistoryCard>();
                // 카드 리스트를 순회하면서 데이터를 저장
                foreach (CardData card in cardList)
                    {
                        // HistoryCard 객체 생성
                        HistoryCard historyCard = new HistoryCard
                        {
                            name = card.cardName,
                            description = card.cardDescription
                        };
                      
                        // 이미지 URL에서 이미지를 다운로드하여 HistoryCard에 저장
                        yield return LoadImageFromUrl(card.cardImg, (sprite) =>
                        {
                            historyCard.imgNO = sprite;
                            Debug.Log("이미지 다운로드 및 저장 완료: " + card.cardName);
                        }, (error) =>
                        {
                            Debug.LogError("이미지 다운로드 실패: " + error);
                            historyCard.imgNO = null;
                        });

                        // HistoryCard 리스트에 추가
                        Managers.Data.cards.Add(historyCard);
                    }

                    // 성공적으로 완료되면 onSuccess 콜백 호출
                    onSuccess?.Invoke();
                
            }
            else
            {
                Debug.LogError("요청 실패: " + www.error);
                onError?.Invoke(www.error);
            }
        }
    }

    // 업적 데이터를 받아와 처리하는 메서드
    public IEnumerator GetUserTitleInfo(string userId, System.Action onSuccess, System.Action<string> onError)
    {
        // GET 요청 URL에 userId를 쿼리 파라미터로 추가
        string endpoint = $"{apiUrl}user/title?userId={userId}";

        // 서버에 GET 요청 보내기
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<TitleData> titleList = null;
                try
                {
                    // 서버에서 받은 JSON 데이터를 리스트로 파싱
                    titleList = JsonHelper.FromJson<TitleData>(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("업적 정보 파싱 실패: " + ex.Message);
                    onError?.Invoke("업적 정보 파싱 실패: " + ex.Message);
                }
                Managers.Data.titles = new List<HistoryTitle>();
                // 카드 리스트를 순회하면서 데이터를 저장
                foreach (TitleData title in titleList)
                {
                    // HistoryCard 객체 생성
                    HistoryTitle historyTitle = new HistoryTitle
                    {
                        userTitle = title.userTitle
                    };

                    // HistoryTitle 리스트에 추가
                    Managers.Data.titles.Add(historyTitle);
                }

                // 성공적으로 완료되면 onSuccess 콜백 호출
                onSuccess?.Invoke();

            }
            else
            {
                Debug.LogError("요청 실패: " + www.error);
                onError?.Invoke(www.error);
            }
        }
    }

    // 이미지 다운로드 후 Sprite로 변환하는 메서드
    public IEnumerator LoadImageFromUrl(string url, System.Action<Sprite> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("이미지 다운로드 실패: " + request.error);
                onError?.Invoke("이미지 다운로드 실패: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                onSuccess?.Invoke(sprite);
            }
        }
    }

    // 게임 클리어 시 업적 저장 함수
    public IEnumerator AddHistoryTitle(long checkId, string userId, System.Action onSuccess)
    {
        // 쿼리 스트링 형식으로 URL에 파라미터 추가
        string urlWithParams = $"{apiUrl}user/addcard?checkId={checkId}&userId={userId}";
        // UnityWebRequest로 POST 요청을 보냅니다 (쿼리 스트링 사용)
        UnityWebRequest request = new UnityWebRequest(urlWithParams, "POST");

        // 요청을 보냅니다.
        yield return request.SendWebRequest();

        onSuccess?.Invoke();
    }

    public IEnumerator UpdateUserTitle(string userId, string title, System.Action onSuccess, System.Action<string> onError)
    {
        // URL에 쿼리 파라미터 추가
        string url = apiUrl + $"user/titlepatch?userId={userId}&title={title}";

        // PATCH 요청 설정
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        request.downloadHandler = new DownloadHandlerBuffer();

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();

        // 요청 상태 확인
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            Managers.Data.userInfo.title = title;
            string responseData = request.downloadHandler.text;
            Debug.Log("응답 데이터: " + responseData);
            onSuccess?.Invoke();
        }
    }
    // 데이터를 요청하는 함수 (GET)
    public IEnumerator GetData(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // GET 요청
        yield return SendRequest(apiUrl + endpoint, "GET", null, onSuccess, onError);
    }

    // 데이터를 삭제하는 함수 (DELETE)
    public IEnumerator DeleteData(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // DELETE 요청
        yield return SendRequest(apiUrl + endpoint, "DELETE", null, onSuccess, onError);
    }

    // 공통 API 요청 메서드 (GET, POST, PUT, DELETE 등)
    private IEnumerator SendRequest(string url, string method, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        UnityWebRequest request;

        // 요청 방법에 따른 처리
        if (method == "GET")
        {
            request = UnityWebRequest.Get(url);
            // JWT 토큰을 Authorization 헤더에 추가
            string accessToken = PlayerPrefs.GetString("accessToken", "");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            }
        }
        else if (method == "POST" || method == "PUT" || method == "PATCH")
        {
            request = new UnityWebRequest(url, method);
            if (jsonData != null)
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }
        }
        else if (method == "DELETE")
        {
            request = UnityWebRequest.Delete(url);
        }
        else
        {
            Debug.LogError("지원하지 않는 HTTP 메서드: " + method);
            yield break;
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();
        Debug.Log("요청 상태: " + request.result);
        // 오류 처리
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            // 성공 시 처리
            string responseData = request.downloadHandler.text;
            onSuccess?.Invoke(responseData);
        }
    }
    public static class JsonHelper
    {
        // JSON 배열을 리스트로 변환하는 함수
        public static List<T> FromJson<T>(string json)
        {
            // 배열을 감싸는 가짜 클래스를 만들어서 처리하는 방식
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        // 리스트를 JSON 배열로 변환하는 함수
        public static string ToJson<T>(List<T> array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper);
        }

        // 리스트를 보기 좋게 포맷된 JSON 배열로 변환하는 함수 (pretty print)
        public static string ToJson<T>(List<T> array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        // JSON 배열을 감싸기 위한 Wrapper 클래스
        [Serializable]
        private class Wrapper<T>
        {
            public List<T> array;
        }
    }


}

