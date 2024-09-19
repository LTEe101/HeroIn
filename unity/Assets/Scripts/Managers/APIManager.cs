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
        yield return SendRequest(apiUrl + "user/login", "POST", jsonData, onSuccess, onError);
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
