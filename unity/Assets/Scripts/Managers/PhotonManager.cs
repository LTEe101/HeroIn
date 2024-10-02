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
}
