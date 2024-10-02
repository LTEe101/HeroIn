using UnityEngine;
using Photon.Pun;

public class PhotonManager : MonoBehaviourPunCallbacks
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
        // 위치와 회전을 설정합니다.
        Vector3 spawnPosition = new Vector3(-11.6700001f, 8.22999954f, -61.1100006f);
        Quaternion spawnRotation = Quaternion.Euler(0, 83.0210037f, 0); // 회전은 Quaternion으로 변환

        // 인스턴스화 시킵니다.
        GameObject PI = PhotonNetwork.Instantiate("Prefabs/Player", spawnPosition, spawnRotation);
    }
}
