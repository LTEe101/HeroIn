using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    public TMP_InputField chatInput;
    public TMP_Text chatOutput;
    private string userName;

    void Start()
    {
        userName = "Player" + Random.Range(1000, 9999);
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));
        chatOutput.text = "Test message";  // 시작 시 테스트 메시지 표시
    }

    void Update()
    {
        chatClient.Service();
    }

    public void SendChatMessage()
    {
        Debug.Log($"Current chatInput.text: '{chatInput.text}'");
        if (chatInput.text.Length > 0)
        {
            Debug.Log($"Attempting to send message: {chatInput.text}");
            chatClient.PublishMessage("GlobalChannel", chatInput.text);
            Debug.Log("Message sent to Photon Chat server");
            chatInput.text = "";
        }
    }

    // IChatClientListener 인터페이스 구현
    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat");
        chatClient.Subscribe(new string[] { "GlobalChannel" });
        Debug.Log("Subscribed to GlobalChannel");
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
       Debug.Log($"Received messages on channel: {channelName}");
        for (int i = 0; i < senders.Length; i++)
        {
            string msg = $"{senders[i]}: {messages[i]}";
            Debug.Log($"Message received: {msg}");
            chatOutput.text += msg + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"Private message received from {sender}: {message}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Subscribed to channels: {string.Join(", ", channels)}");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log($"Unsubscribed from channels: {string.Join(", ", channels)}");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Status update for {user}: {status}");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"User {user} subscribed to channel {channel}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"User {user} unsubscribed from channel {channel}");
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Photon Chat Debug ({level}): {message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Chat state changed to: {state}");
    }
}