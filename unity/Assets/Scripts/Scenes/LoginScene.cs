using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScene : BaseScene
{
    // UI 요소를 변수로 선언
    public InputField userIdInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button joingButton;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        joingButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Login();
        }
    }

    public void Login()
    {
        string userId = userIdInputField.text;
        string password = passwordInputField.text;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.Log("아이디 또는 비밀번호 입력");
            return;
        }
        // 로그인 시도
        StartCoroutine(Managers.API.Login(
             userId,
             password,
             (message) => {
                 // 로그인 성공 시 처리
                 Debug.Log(message);

                 // 로그인 성공 후 유저 정보 가져오기
                 StartCoroutine(Managers.API.GetUserInfo(
                     (userInfoResponse) => {
                         // UserInfoResponse -> User로 변환
                         User user = new User
                         {
                             name = userInfoResponse.userName,
                             title = userInfoResponse.title, 
                             imgNo = userInfoResponse.img
                         };

                         // 유저 정보 저장
                         Managers.Data.SetUserInfo(user);
                         Debug.Log("유저 정보 저장 성공: " + user.name);

                         // 박물관 씬으로 이동
                         Managers.Scene.LoadScene(Define.Scene.Museum);
                     },
                     (error) => {
                         Debug.LogError($"유저 정보 가져오기 실패: {error}");
                     }
                 ));
             },
             (error) => {
                 // 로그인 실패 시 처리
                 Debug.LogError($"로그인 실패: {error}");
             }
         ));
    }


    private void OnLoginButtonClicked()
    {
        Login();
    }

    private void OnJoinButtonClicked()
    {
        Managers.Scene.LoadScene(Define.Scene.Join);
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
}
