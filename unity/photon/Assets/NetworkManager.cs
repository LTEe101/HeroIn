using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        JoinMainScene();
    }

    void JoinMainScene()
    {
        // 기본 룸을 생성하거나 참가합니다.
        PhotonNetwork.JoinOrCreateRoom("MainSceneRoom", new Photon.Realtime.RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room. Loading MainScene.");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }

        // 채팅 매니저 활성화
        ChatManager chatManager = FindObjectOfType<ChatManager>();
        if (chatManager != null)
        {
            chatManager.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ChatManager not found in the scene.");
        }
    }
}