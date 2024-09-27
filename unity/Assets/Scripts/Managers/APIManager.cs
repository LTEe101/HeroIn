using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://j11e101.p.ssafy.io/api/";

    // 회원가입 요청을 보내는 함수 (POST)
    public IEnumerator Join(string userName, string userLoginId, string userLoginPw, Image userImage, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON 데이터 생성
        JoinData joinData = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };

        // JSON 형식으로 변환
        string jsonData = JsonUtility.ToJson(joinData);

        // Image의 텍스처에서 바이트 배열을 추출
        Texture2D texture = userImage.sprite.texture;

        // 이미지 크기를 256x256으로 줄이기 (필요에 따라 크기를 조정)
        Texture2D resizedTexture = new Texture2D(256, 256);
        resizedTexture.SetPixels(texture.GetPixels());
        resizedTexture.Apply();

        // PNG 형식으로 인코딩
        byte[] imageBytes = resizedTexture.EncodeToPNG();

        // 멀티파트 폼 데이터 생성
        WWWForm form = new WWWForm();

        // JSON 데이터를 바이너리 데이터로 추가 (application/json으로 보냄)
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        form.AddBinaryData("userDto", jsonBytes, "userData.json", "application/json");

        // 이미지 파일 추가 (바이트 배열)
        form.AddBinaryData("userImg", imageBytes, "userImage.png", "image/png");

        // UnityWebRequest를 사용한 POST 요청
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "user/sign-up", form))
        {
            // 요청 전송 및 응답 대기
            yield return www.SendWebRequest();

            // 오류 처리
            if (www.result != UnityWebRequest.Result.Success)
            {
                // 요청 실패 처리
                Debug.LogError("요청 실패: " + www.downloadHandler.text); // 오류 메시지 출력
                onError?.Invoke(www.downloadHandler.text); // 서버 응답 내용 전달
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

                Debug.Log("로그인 성공, 토큰 저장 완료");
            }

            // 성공 콜백 호출
            onSuccess?.Invoke(response);

        }, onError);
    }

    // 유저 정보 요청 (POST) - JWT 토큰으로 유저 정보 가져오기
    public IEnumerator GetUserInfo(System.Action<UserInfoResponse> onSuccess, System.Action<string> onError)
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

