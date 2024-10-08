using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingScreen;  // �ε� ȭ�� UI ������Ʈ
    [SerializeField] private GameObject chtting;  // �ε� ȭ�� UI ������Ʈ
    [SerializeField] private Button chttingButton;  // �ε� ȭ�� UI ������Ʈ
    [SerializeField] private RectTransform chatScrollView;  // ��ũ�� ���� RectTransform
    [SerializeField] private RectTransform topIcon;  // ��ũ�� ���� RectTransform

    private bool isChatMinimized = false; // ä���� ��ҵǾ����� ���θ� ����
                                          // �̱��� �ν��Ͻ�
    public static PhotonManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        ShowLoadingScreen(true);  // ���� �õ� �� �ε� ȭ�� Ȱ��ȭ
        PhotonNetwork.ConnectUsingSettings();

        // ��ư Ŭ�� �̺�Ʈ�� �޼��� ����
        chttingButton.onClick.AddListener(ToggleChatting);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.NickName = Managers.Data.userInfo.name;

        // 1�� ������ �ְ� ���� ������ ����
        StartCoroutine(DelayedJoinMainScene());
    }

    IEnumerator DelayedJoinMainScene()
    {
        yield return new WaitForSeconds(1f); // 1�� ����
        JoinMainScene();
        ShowLoadingScreen(false);  // ���� �Ϸ� �� �ε� ȭ�� �����
    }

    void JoinMainScene()
    {
        // �⺻ ���� �����ϰų� �����մϴ�.
        PhotonNetwork.JoinOrCreateRoom("MainSceneRoom", new Photon.Realtime.RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room. Loading MainScene.");
        // ��ġ�� ȸ���� �����մϴ�.
        Vector3 spawnPosition = new Vector3(-11.6700001f, 8.22999954f, -61.1100006f);
        Quaternion spawnRotation = Quaternion.Euler(0, 83.0210037f, 0); // ȸ���� Quaternion���� ��ȯ

        // �ν��Ͻ�ȭ ��ŵ�ϴ�.
        GameObject PI = PhotonNetwork.Instantiate("Prefabs/Player", spawnPosition, spawnRotation);
    }

    // ���� ������ ���� �������� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"���� ���� ���� ����: {cause}");
        Managers.Scene.LoadScene(Define.Scene.Museum);
    }

    // �ε� ȭ���� �����ְų� ����� �Լ�
    private void ShowLoadingScreen(bool show)
    {
        loadingScreen.SetActive(show);  // true�� �ε� ȭ�� Ȱ��ȭ, false�� ��Ȱ��ȭ
        chtting.SetActive(!show);
    }

    // ä�� UI Ȱ��/��Ȱ�� ��� �Լ�
    public void ToggleChatting()
    {
        if (!isChatMinimized)
        {
            chatScrollView.gameObject.SetActive(false);
            topIcon.anchorMin = new Vector2(0, 0);  // ���� �ϴ�
            topIcon.anchorMax = new Vector2(1, 0);  // ������ �ϴ�
            topIcon.pivot = new Vector2(0.5f, 0);   // �ǹ��� �ϴ� �߾����� ����
            topIcon.anchoredPosition = Vector2.zero; // ��Ŀ ���� ��ġ�� (0, 0)���� ����
        }
        else
        {
            chatScrollView.gameObject.SetActive(true);
            topIcon.anchorMin = new Vector2(0, 1);  // ���� ���
            topIcon.anchorMax = new Vector2(1, 1);  // ������ ���
            topIcon.pivot = new Vector2(0.5f, 1);   // �ǹ��� ��� �߾����� ����
            topIcon.anchoredPosition = Vector2.zero; // ��Ŀ ���� ��ġ�� (0, 0)���� ����


        }
        isChatMinimized = !isChatMinimized; // ���¸� ���
    }
}
