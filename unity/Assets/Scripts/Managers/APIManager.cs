using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://j11e101.p.ssafy.io/api/";

    // ȸ������ ��û�� ������ �Լ� (POST)
    public IEnumerator Join(string userName, string userLoginId, string userLoginPw, Image userImage, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON ������ ����
        JoinData joinData = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };

        // JSON �������� ��ȯ
        string jsonData = JsonUtility.ToJson(joinData);

        // Image�� �ؽ�ó���� ����Ʈ �迭�� ����
        Texture2D texture = userImage.sprite.texture;

        // �̹��� ũ�⸦ 256x256���� ���̱� (�ʿ信 ���� ũ�⸦ ����)
        Texture2D resizedTexture = new Texture2D(256, 256);
        resizedTexture.SetPixels(texture.GetPixels());
        resizedTexture.Apply();

        // PNG �������� ���ڵ�
        byte[] imageBytes = resizedTexture.EncodeToPNG();

        // ��Ƽ��Ʈ �� ������ ����
        WWWForm form = new WWWForm();

        // JSON �����͸� ���̳ʸ� �����ͷ� �߰� (application/json���� ����)
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        form.AddBinaryData("userDto", jsonBytes, "userData.json", "application/json");

        // �̹��� ���� �߰� (����Ʈ �迭)
        form.AddBinaryData("userImg", imageBytes, "userImage.png", "image/png");

        // UnityWebRequest�� ����� POST ��û
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "user/sign-up", form))
        {
            // ��û ���� �� ���� ���
            yield return www.SendWebRequest();

            // ���� ó��
            if (www.result != UnityWebRequest.Result.Success)
            {
                // ��û ���� ó��
                Debug.LogError("��û ����: " + www.downloadHandler.text); // ���� �޽��� ���
                onError?.Invoke(www.downloadHandler.text); // ���� ���� ���� ����
            }
            else
            {
                // ��û ���� ó��
                onSuccess?.Invoke(www.downloadHandler.text);
            }
        }
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

    // ���� ���� ��û (POST) - JWT ��ū���� ���� ���� ��������
    public IEnumerator GetUserInfo(System.Action<UserInfoResponse> onSuccess, System.Action<string> onError)
    {
        // JWT ��ū ��������
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string refreshToken = PlayerPrefs.GetString("refreshToken", "");

        // ��ȿ�� ��ū�� �ִ��� Ȯ��
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke("��ū�� �����ϴ�.");
            yield break;
        }

        // ��û ������ ����
        TokenRequest tokenRequest = new TokenRequest
        {
            grantType = "Bearer",
            accessToken = accessToken,
            refreshToken = refreshToken
        };

        // JSON �����ͷ� ����ȭ
        string jsonData = JsonUtility.ToJson(tokenRequest);

        // POST ��û ������
        yield return SendRequest(apiUrl + "user/profile", "POST", jsonData, (response) =>
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

