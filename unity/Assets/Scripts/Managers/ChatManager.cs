using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro; 
using UnityEngine.UI; 

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatMessagePrefab; // ä�� �޽����� ǥ���� TextMeshPro ������
    [SerializeField] private Transform content; // Scroll View�� Content (ä�� �޽������� �߰��� �θ� ������Ʈ)
    [SerializeField] private ScrollRect scrollRect; // ScrollRect ������Ʈ
    private string userName; // �⺻ ����� �̸�
    private PhotonPlayerController localPlayerController; // ���� �÷��̾� ��Ʈ�ѷ� ����
    private static ChatManager _instance;
    public static ChatManager Instance { get { return _instance; } }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        userName = "Player" + Random.Range(1000, 9999);
        ConnectToPhotonChat();
        inputField.onEndEdit.AddListener(HandleInputEndEdit);
        inputField.onSelect.AddListener(HandleInputSelect);
        inputField.onDeselect.AddListener(HandleInputDeselect);
    }

    // ���� �÷��̾� ���� �޼���
    public void SetLocalPlayer(PhotonPlayerController playerController)
    {
        localPlayerController = playerController;
    }
    // Enter Ű�� ������ ä�� �޽��� ����
    private void HandleInputEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter Ű�� Ȯ��
        {
            SendMessage(); // �޽��� ����
        }
    }
    // �Է� �ʵ� ��Ŀ�� ȹ�� �� ȣ��Ǵ� �޼���
    private void HandleInputSelect(string input)
    {
        if (localPlayerController != null)
        {
            localPlayerController.SetState(PhotonPlayerController.PlayerState.Chatting); 
        }
    }

    // �Է� �ʵ� ��Ŀ�� ���� �� ȣ��Ǵ� �޼���
    private void HandleInputDeselect(string input)
    {
        if (localPlayerController != null)
        {
            localPlayerController.SetState(PhotonPlayerController.PlayerState.Idle); 
        }
    }
    // Photon Chat�� �����ϴ� �Լ�
    void ConnectToPhotonChat()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));
    }

    // Photon Chat ���� ���� ������Ʈ
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && localPlayerController.State != PhotonPlayerController.PlayerState.Chatting)
        {
            inputField.ActivateInputField();
        }
        if (chatClient != null)
        {
            chatClient.Service(); // �޼��� �ۼ��� ó��
        }

    }

    // ä�� �޽��� ����
    public void SendMessage()
    {
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message) && localPlayerController != null)
        {
            chatClient.PublishMessage("global", message); // 'global' ä�η� �޽��� ����
            inputField.text = ""; // �Է� �ʵ� �ʱ�ȭ

            // ���� �÷��̾��� PhotonView�� ���� ��ǳ�� ǥ��
            localPlayerController.PV.RPC("ShowChatBubble", RpcTarget.All, message);
        }
        inputField.ActivateInputField(); // �Է� �ʵ忡 �ٽ� ��Ŀ�� ���߱�
        
    }

    // ä�ο��� ���ŵ� �޽��� ó��
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            // ä�� �޽��� �������� �ν��Ͻ�ȭ�Ͽ� content�� �߰�
            TMP_Text newMessage = Instantiate(chatMessagePrefab, content);
            newMessage.text = $"{senders[i]}: {messages[i]}";

            // ��ũ���� �ڵ����� ���ϴ����� �̵�
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        
    }

    // ������ �������� �� ȣ��Ǵ� �Լ�
    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat!");
        chatClient.Subscribe(new string[] { "global" }); // 'global' ä�ο� ����
    }

    // ���� �̺�Ʈ ó�� �޼���
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user} has subscribed to {channel}.");
    }

    // ���� ���� �̺�Ʈ ó�� �޼���
    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user} has unsubscribed from {channel}.");
    }

    // ���� ó�� �޼���
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channel");
    }

    // ���� ���� ó�� �޼���
    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from channel");
    }

    // ������ ������ �� ȣ��Ǵ� �Լ�
    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    // ä�� ���� ���� ó�� �޼���
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat state changed: {state}");
    }

    // ���� �޽��� ���� ó�� �޼���
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"Private message from {sender}: {message}");
    }

    // ����� ���� ������Ʈ �޼���
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"User {user} status updated: {status}, message: {message}");
    }

    // ����� �޽��� ó�� �޼���
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Debug: {message}");
    }
}
