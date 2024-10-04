using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingScreen;  // 로딩 화면 UI 오브젝트
    [SerializeField] private GameObject chtting;  // 로딩 화면 UI 오브젝트
    [SerializeField] private Button chttingButton;  // 로딩 화면 UI 오브젝트
    [SerializeField] private RectTransform chatScrollView;  // 스크롤 뷰의 RectTransform
    [SerializeField] private RectTransform topIcon;  // 스크롤 뷰의 RectTransform

    private bool isChatMinimized = false; // 채팅이 축소되었는지 여부를 저장

    void Start()
    {
        ShowLoadingScreen(true);  // 연결 시도 전 로딩 화면 활성화
        PhotonNetwork.ConnectUsingSettings();

        // 버튼 클릭 이벤트에 메서드 연결
        chttingButton.onClick.AddListener(ToggleChatting);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.NickName = Managers.Data.userInfo.name;

        // 1초 지연을 주고 메인 씬으로 참가
        StartCoroutine(DelayedJoinMainScene());
    }

    IEnumerator DelayedJoinMainScene()
    {
        yield return new WaitForSeconds(1f); // 1초 지연
        JoinMainScene();
        ShowLoadingScreen(false);  // 연결 완료 시 로딩 화면 숨기기
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

    // 포톤 서버에 연결 실패했을 때 호출되는 콜백 함수
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"포톤 서버 연결 실패: {cause}");
        Managers.Scene.LoadScene(Define.Scene.Museum);
    }

    // 로딩 화면을 보여주거나 숨기는 함수
    private void ShowLoadingScreen(bool show)
    {
        loadingScreen.SetActive(show);  // true면 로딩 화면 활성화, false면 비활성화
        chtting.SetActive(!show);
    }

    // 채팅 UI 활성/비활성 토글 함수
    private void ToggleChatting()
    {
        if (!isChatMinimized)
        {
            chatScrollView.gameObject.SetActive(false);
            topIcon.anchoredPosition = new Vector2(topIcon.anchoredPosition.x, -343.454f);
        }
        else
        {
            chatScrollView.gameObject.SetActive(true);
            topIcon.anchoredPosition = new Vector2(topIcon.anchoredPosition.x, -3.453949f);  // Y 값을 조정하여 위치 이동


        }
        isChatMinimized = !isChatMinimized; // 상태를 토글
    }
}
