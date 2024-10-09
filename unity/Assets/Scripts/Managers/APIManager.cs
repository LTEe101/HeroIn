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

    // ȸ������ ��û�� ������ �Լ� (�̹����� JSON ������ ����)
    public IEnumerator Join(string userName, string userLoginId, string userLoginPw, Image userImage, System.Action<string> onSuccess, System.Action<string> onError)
    {
        // JSON ������ ����
        JoinData userDto = new JoinData
        {
            userLoginId = userLoginId,
            userLoginPw = userLoginPw,
            userName = userName
        };
        Texture2D texture;
        // JSON �������� ��ȯ
        string jsonData = JsonUtility.ToJson(userDto);
        if (userImage.sprite.name == "UI_Icon_Plus")
        {
            // Resources �������� �̹��� �ε�
            texture = Resources.Load<Sprite>("Art/Image/Logo").texture;
        }
        else
        {
            texture = userImage.sprite.texture;
        }
           
        byte[] imageBytes = texture.EncodeToPNG();

        // ��Ƽ��Ʈ �� ������ ����
        WWWForm form = new WWWForm();

        // JSON �����͸� �ؽ�Ʈ �ʵ�� �߰� (�������� �ʵ�� "userDto"�� �޾ƾ� ��)
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        form.AddBinaryData("userDto", jsonBytes, "userDto.json", "application/json");

        // �̹��� ���� �߰� (����Ʈ �迭)
        if (imageBytes != null)
        {
            form.AddBinaryData("userImg", imageBytes, "userImage.png", "image/png");
        }

        // UnityWebRequest�� ��Ƽ��Ʈ �� ������ ����
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "user/sign-up", form))
        {
            // ��û ���� �� ���� ���
            yield return www.SendWebRequest();

            // ���� ó��
            if (www.result != UnityWebRequest.Result.Success)
            {
                // ��û ���� ó��
                Debug.LogError("��û ����: " + www.downloadHandler.text);
                onError?.Invoke(www.downloadHandler.text);
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
                User user = new User
                {
                    userId = userLoginId
                };
                // ���� ���� ����
                Managers.Data.SetUserInfo(user);
                Debug.Log("�α��� ����, ��ū ���� �Ϸ�");
            }
            
            // ���� �ݹ� ȣ��
            onSuccess?.Invoke(response);

        }, onError);
    }

    // ���� ���� ��û (POST) - JWT ��ū���� ���� ���� ��������
    public IEnumerator GetUserInfo(System.Action<User> onSuccess, System.Action<string> onError)
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
            UserInfoResponse userInfoResponse = JsonUtility.FromJson<UserInfoResponse>(response);

            if (userInfoResponse != null)
            {
                Debug.Log("���� ���� �������� ����: " + userInfoResponse.userName);

                // ���� ������ User ��ü�� ��ȯ
                User user = new User
                {
                    userId = Managers.Data.userInfo.userId,
                    name = userInfoResponse.userName,
                    title = userInfoResponse.title
                };

                // �̹��� URL�κ��� �̹����� �ٿ�ε��ؼ� Sprite�� ��ȯ �� User ��ü�� ����
                StartCoroutine(LoadImageFromUrl(userInfoResponse.img, (sprite) =>
                {
                    user.imgNo = sprite;
                    onSuccess?.Invoke(user);
                }, onError));
            }
            else
            {
                onError?.Invoke("���� ���� �Ľ� ����");
            }

        }, onError);
    }

    // ī�� �����͸� �޾ƿ� ó���ϴ� �޼���
    public IEnumerator GetUserCardInfo(string userId, System.Action onSuccess, System.Action<string> onError)
    {
        // GET ��û URL�� userId�� ���� �Ķ���ͷ� �߰�
        string endpoint = $"{apiUrl}user/card?userId={userId}";

        // ������ GET ��û ������
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<CardData> cardList = null;
                try
                {
                    // �������� ���� JSON �����͸� ����Ʈ�� �Ľ�
                    cardList = JsonHelper.FromJson<CardData>(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("ī�� ���� �Ľ� ����: " + ex.Message);
                    onError?.Invoke("ī�� ���� �Ľ� ����: " + ex.Message);
                }
                Managers.Data.cards = new List<HistoryCard>();
                // ī�� ����Ʈ�� ��ȸ�ϸ鼭 �����͸� ����
                foreach (CardData card in cardList)
                    {
                        // HistoryCard ��ü ����
                        HistoryCard historyCard = new HistoryCard
                        {
                            name = card.cardName,
                            description = card.cardDescription
                        };
                      
                        // �̹��� URL���� �̹����� �ٿ�ε��Ͽ� HistoryCard�� ����
                        yield return LoadImageFromUrl(card.cardImg, (sprite) =>
                        {
                            historyCard.imgNO = sprite;
                            Debug.Log("�̹��� �ٿ�ε� �� ���� �Ϸ�: " + card.cardName);
                        }, (error) =>
                        {
                            Debug.LogError("�̹��� �ٿ�ε� ����: " + error);
                            historyCard.imgNO = null;
                        });

                        // HistoryCard ����Ʈ�� �߰�
                        Managers.Data.cards.Add(historyCard);
                    }

                    // ���������� �Ϸ�Ǹ� onSuccess �ݹ� ȣ��
                    onSuccess?.Invoke();
                
            }
            else
            {
                Debug.LogError("��û ����: " + www.error);
                onError?.Invoke(www.error);
            }
        }
    }

    // ���� �����͸� �޾ƿ� ó���ϴ� �޼���
    public IEnumerator GetUserTitleInfo(string userId, System.Action onSuccess, System.Action<string> onError)
    {
        // GET ��û URL�� userId�� ���� �Ķ���ͷ� �߰�
        string endpoint = $"{apiUrl}user/title?userId={userId}";

        // ������ GET ��û ������
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<TitleData> titleList = null;
                try
                {
                    // �������� ���� JSON �����͸� ����Ʈ�� �Ľ�
                    titleList = JsonHelper.FromJson<TitleData>(www.downloadHandler.text);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("���� ���� �Ľ� ����: " + ex.Message);
                    onError?.Invoke("���� ���� �Ľ� ����: " + ex.Message);
                }
                Managers.Data.titles = new List<HistoryTitle>();
                // ī�� ����Ʈ�� ��ȸ�ϸ鼭 �����͸� ����
                foreach (TitleData title in titleList)
                {
                    // HistoryCard ��ü ����
                    HistoryTitle historyTitle = new HistoryTitle
                    {
                        userTitle = title.userTitle
                    };

                    // HistoryTitle ����Ʈ�� �߰�
                    Managers.Data.titles.Add(historyTitle);
                }

                // ���������� �Ϸ�Ǹ� onSuccess �ݹ� ȣ��
                onSuccess?.Invoke();

            }
            else
            {
                Debug.LogError("��û ����: " + www.error);
                onError?.Invoke(www.error);
            }
        }
    }

    // �̹��� �ٿ�ε� �� Sprite�� ��ȯ�ϴ� �޼���
    public IEnumerator LoadImageFromUrl(string url, System.Action<Sprite> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("�̹��� �ٿ�ε� ����: " + request.error);
                onError?.Invoke("�̹��� �ٿ�ε� ����: " + request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                onSuccess?.Invoke(sprite);
            }
        }
    }

    // ���� Ŭ���� �� ���� ���� �Լ�
    public IEnumerator AddHistoryTitle(long checkId, string userId, System.Action onSuccess)
    {
        // ���� ��Ʈ�� �������� URL�� �Ķ���� �߰�
        string urlWithParams = $"{apiUrl}user/addcard?checkId={checkId}&userId={userId}";
        // UnityWebRequest�� POST ��û�� �����ϴ� (���� ��Ʈ�� ���)
        UnityWebRequest request = new UnityWebRequest(urlWithParams, "POST");

        // ��û�� �����ϴ�.
        yield return request.SendWebRequest();

        onSuccess?.Invoke();
    }

    public IEnumerator UpdateUserTitle(string userId, string title, System.Action onSuccess, System.Action<string> onError)
    {
        // URL�� ���� �Ķ���� �߰�
        string url = apiUrl + $"user/titlepatch?userId={userId}&title={title}";

        // PATCH ��û ����
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        request.downloadHandler = new DownloadHandlerBuffer();

        // ��û ���� �� ���� ���
        yield return request.SendWebRequest();

        // ��û ���� Ȯ��
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            onError?.Invoke(request.error);
        }
        else
        {
            Managers.Data.userInfo.title = title;
            string responseData = request.downloadHandler.text;
            Debug.Log("���� ������: " + responseData);
            onSuccess?.Invoke();
        }
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
            Debug.LogError("�������� �ʴ� HTTP �޼���: " + method);
            yield break;
        }

        request.downloadHandler = new DownloadHandlerBuffer();

        // ��û ���� �� ���� ���
        yield return request.SendWebRequest();
        Debug.Log("��û ����: " + request.result);
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
    public static class JsonHelper
    {
        // JSON �迭�� ����Ʈ�� ��ȯ�ϴ� �Լ�
        public static List<T> FromJson<T>(string json)
        {
            // �迭�� ���δ� ��¥ Ŭ������ ���� ó���ϴ� ���
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        // ����Ʈ�� JSON �迭�� ��ȯ�ϴ� �Լ�
        public static string ToJson<T>(List<T> array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper);
        }

        // ����Ʈ�� ���� ���� ���˵� JSON �迭�� ��ȯ�ϴ� �Լ� (pretty print)
        public static string ToJson<T>(List<T> array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        // JSON �迭�� ���α� ���� Wrapper Ŭ����
        [Serializable]
        private class Wrapper<T>
        {
            public List<T> array;
        }
    }


}

