using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://j11e101.p.ssafy.io/api/";

    // ȸ������ ��û�� ������ �Լ� (POST)
    public IEnumerator Join(string userLoginId, string userLoginPw, string userName, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON ������ ����
        JoinData joinData = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };

        string jsonData = JsonUtility.ToJson(joinData); // JSON �������� ��ȯ

        // POST ��û
        yield return SendRequest(apiUrl + "user/sign-up", "POST", jsonData, onSuccess, onError);
    }

    // �α��� ��û�� ������ �Լ� (POST)
    public IEnumerator Login(string userLoginId, string userLoginPw, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON ������ ����
        LoginData loginData = new LoginData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw
        };

        string jsonData = JsonUtility.ToJson(loginData); // JSON �������� ��ȯ

        // POST ��û
        yield return SendRequest(apiUrl + "user/login", "POST", jsonData, (response) =>
        {
            // �α��� ���� �� ��ū ����
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(response);

            if (tokenResponse != null)
            {
                // ��ū ���� (PlayerPrefs�� ����)
                PlayerPrefs.SetString("accessToken", tokenResponse.accessToken);
                PlayerPrefs.SetString("refreshToken", tokenResponse.refreshToken);
                PlayerPrefs.Save(); // ����

                Debug.Log("�α��� ����, ��ū ���� �Ϸ�");
            }

            // ���� �ݹ� ȣ��
            onSuccess?.Invoke(response);

        }, onError);
    }

    // ���� ���� ��û (GET) - JWT ��ū���� ���� ���� ��������
    public IEnumerator GetUserInfo(System.Action<UserInfoResponse> onSuccess, System.Action<string> onError)
    {
        // JWT ��ū ��������
        string accessToken = PlayerPrefs.GetString("accessToken", "");

        // ��ȿ�� ��ū�� �ִ��� Ȯ��
        if (string.IsNullOrEmpty(accessToken))
        {
            onError?.Invoke("��ū�� �����ϴ�.");
            yield break;
        }

        // GET ��û
        yield return SendRequest(apiUrl + "user/profile", "GET", null, (response) =>
        {
            // �����κ��� ���� ���� ������ �Ľ�
            UserInfoResponse userInfo = JsonUtility.FromJson<UserInfoResponse>(response);

            if (userInfo != null)
            {
                Debug.Log("���� ���� �������� ����: " + userInfo.userName);
                onSuccess?.Invoke(userInfo);
            }
            else
            {
                onError?.Invoke("���� ���� �Ľ� ����");
            }

        }, onError);
    }


    // �����͸� ��û�ϴ� �Լ� (GET)
    public IEnumerator GetData(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // GET ��û
        yield return SendRequest(apiUrl + endpoint, "GET", null, onSuccess, onError);
    }

    // �����͸� �����ϴ� �Լ� (DELETE)
    public IEnumerator DeleteData(string endpoint, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // DELETE ��û
        yield return SendRequest(apiUrl + endpoint, "DELETE", null, onSuccess, onError);
    }

    // ���� API ��û �޼��� (GET, POST, PUT, DELETE ��)
    private IEnumerator SendRequest(string url, string method, string jsonData, System.Action<string> onSuccess, System.Action<string> onError)
    {
        UnityWebRequest request;

        // ��û ����� ���� ó��
        if (method == "GET")
        {
            request = UnityWebRequest.Get(url);
            // JWT ��ū�� Authorization ����� �߰�
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
            Debug.LogError("�������� �ʴ� HTTP �޼���: " + method);
            yield break;
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        // ��û ���� �� ���� ���
        yield return request.SendWebRequest();

        // ���� ó��
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            // ���� �� ó��
            string responseData = request.downloadHandler.text;
            onSuccess?.Invoke(responseData);
        }
    }
}

// JSON ������ ���� ���� (ȸ������)
[System.Serializable]
public class JoinData
{
    public string userLoginId;   // ���� ID
    public string userLoginPw;   // ��й�ȣ
    public string userName;      // ����� �̸�
}

// �α��� ��û�� ���Ǵ� ������ Ŭ����
[System.Serializable]
public class LoginData
{
    public string userLoginId;   // ���� ID
    public string userLoginPw;   // ��й�ȣ
}

// ��ū ���信 ���Ǵ� ������ Ŭ���� (�α��� ���� �� �޴� ��ū ����)
[System.Serializable]
public class TokenResponse
{
    public string accessToken;
    public string refreshToken;
}

// ���� ���� ���信 ���Ǵ� ������ Ŭ����
[System.Serializable]
public class UserInfoResponse
{
    public string userName;
    public string img;
    public string title;
}
