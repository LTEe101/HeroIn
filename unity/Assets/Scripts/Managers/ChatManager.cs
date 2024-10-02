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
    [SerializeField] private TMP_Text chatMessagePrefab; // 채팅 메시지를 표시할 TextMeshPro 프리팹
    [SerializeField] private Transform content; // Scroll View의 Content (채팅 메시지들이 추가될 부모 오브젝트)
    [SerializeField] private ScrollRect scrollRect; // ScrollRect 컴포넌트
    private string userName; // 기본 사용자 이름
    private PhotonPlayerController localPlayerController; // 로컬 플레이어 컨트롤러 저장
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

    // 로컬 플레이어 설정 메서드
    public void SetLocalPlayer(PhotonPlayerController playerController)
    {
        localPlayerController = playerController;
    }
    // Enter 키를 누르면 채팅 메시지 전송
    private void HandleInputEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter 키를 확인
        {
            SendMessage(); // 메시지 전송
        }
    }
    // 입력 필드 포커스 획득 시 호출되는 메서드
    private void HandleInputSelect(string input)
    {
        if (localPlayerController != null)
        {
            localPlayerController.SetState(PhotonPlayerController.PlayerState.Chatting); 
        }
    }

    // 입력 필드 포커스 해제 시 호출되는 메서드
    private void HandleInputDeselect(string input)
    {
        if (localPlayerController != null)
        {
            localPlayerController.SetState(PhotonPlayerController.PlayerState.Idle); 
        }
    }
    // Photon Chat에 연결하는 함수
    void ConnectToPhotonChat()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));
    }

    // Photon Chat 연결 상태 업데이트
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && localPlayerController.State != PhotonPlayerController.PlayerState.Chatting)
        {
            inputField.ActivateInputField();
        }
        if (chatClient != null)
        {
            chatClient.Service(); // 메세지 송수신 처리
        }

    }

    // 채팅 메시지 전송
    public void SendMessage()
    {
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message) && localPlayerController != null)
        {
            chatClient.PublishMessage("global", message); // 'global' 채널로 메시지 전송
            inputField.text = ""; // 입력 필드 초기화

            // 로컬 플레이어의 PhotonView를 통해 말풍선 표시
            localPlayerController.PV.RPC("ShowChatBubble", RpcTarget.All, message);
        }
        inputField.ActivateInputField(); // 입력 필드에 다시 포커스 맞추기
        
    }

    // 채널에서 수신된 메시지 처리
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            // 채팅 메시지 프리팹을 인스턴스화하여 content에 추가
            TMP_Text newMessage = Instantiate(chatMessagePrefab, content);
            newMessage.text = $"{senders[i]}: {messages[i]}";

            // 스크롤을 자동으로 최하단으로 이동
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        
    }

    // 연결이 성공했을 때 호출되는 함수
    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat!");
        chatClient.Subscribe(new string[] { "global" }); // 'global' 채널에 가입
    }

    // 구독 이벤트 처리 메서드
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user} has subscribed to {channel}.");
    }

    // 구독 해제 이벤트 처리 메서드
    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user} has unsubscribed from {channel}.");
    }

    // 구독 처리 메서드
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channel");
    }

    // 구독 해제 처리 메서드
    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from channel");
    }

    // 연결이 끊겼을 때 호출되는 함수
    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    // 채팅 상태 변경 처리 메서드
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat state changed: {state}");
    }

    // 개인 메시지 수신 처리 메서드
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"Private message from {sender}: {message}");
    }

    // 사용자 상태 업데이트 메서드
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"User {user} status updated: {status}, message: {message}");
    }

    // 디버그 메시지 처리 메서드
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Debug: {message}");
    }
}
