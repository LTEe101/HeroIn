using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://j11e101.p.ssafy.io/api/";

    // 회원가입 요청을 보내는 함수 (POST)
    public IEnumerator Join(string userLoginId, string userLoginPw, string userName, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON 데이터 생성
        JoinData joinData = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };

        string jsonData = JsonUtility.ToJson(joinData); // JSON 형식으로 변환

        // POST 요청
        yield return SendRequest(apiUrl + "user/sign-up", "POST", jsonData, onSuccess, onError);
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

                Debug.Log("로그인 성공, 토큰 저장 완료");
            }

            // 성공 콜백 호출
            onSuccess?.Invoke(response);

        }, onError);
    }

    // 유저 정보 요청 (GET) - JWT 토큰으로 유저 정보 가져오기
    public IEnumerator GetUserInfo(System.Action<UserInfoResponse> onSuccess, System.Action<string> onError)
    {
        // JWT 토큰 가져오기
        string accessToken = PlayerPrefs.GetString("accessToken", "");

        // 유효한 토큰이 있는지 확인
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("토큰이 없습니다.");
            yield break;
        }

        // GET 요청
        yield return SendRequest(apiUrl + "user/profile", "GET", null, (response) =>
        {
            // 서버로부터 받은 유저 정보를 파싱
            UserInfoResponse userInfo = JsonUtility.FromJson<UserInfoResponse>(response);

            if (userInfo != null)
            {
                Debug.Log("유저 정보 가져오기 성공: " + userInfo.userName);
                onSuccess?.Invoke(userInfo);
            }
            else
            {
                onError?.Invoke("유저 정보 파싱 실패");
            }

        }, onError);
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
        else if (method == "POST" || method == "PUT")
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
}

// JSON 데이터 구조 정의 (회원가입)
[System.Serializable]
public class JoinData
{
    public string userLoginId;   // 계정 ID
    public string userLoginPw;   // 비밀번호
    public string userName;      // 사용자 이름
}

// 로그인 요청에 사용되는 데이터 클래스
[System.Serializable]
public class LoginData
{
    public string userLoginId;   // 계정 ID
    public string userLoginPw;   // 비밀번호
}

// 토큰 응답에 사용되는 데이터 클래스 (로그인 성공 시 받는 토큰 정보)
[System.Serializable]
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
}

// 유저 정보 응답에 사용되는 데이터 클래스
[System.Serializable]
public class UserInfoResponse
{
    public string userName;
    public string img;
    public string title;
}
