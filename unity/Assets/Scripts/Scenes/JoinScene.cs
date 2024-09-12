using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinScene : BaseScene
{
    // UI 요소를 변수로 선언
    public InputField IdInputField;
    public InputField pwdInputField;
    public InputField nameInputField;
    public InputField pwdCheckInputField;
    public Button BackButton;
    public Button joinButton;
    public Image ImgNo;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Join;
        BackButton.onClick.AddListener(OnBackButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnJoinButtonClicked();
        }
    }

    private void OnBackButtonClicked()
    {
        LoadScencLogin();
    }

    private void OnJoinButtonClicked()
    {
        string name = nameInputField.text;
        string Id = IdInputField.text;
        string pwd = pwdInputField.text;
        string pwdCheck = pwdCheckInputField.text;
        Image imgNo = ImgNo;
        Debug.Log(pwd);
        Debug.Log(pwdCheck);
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("이름 입력");
            return;
        }
        else if (string.IsNullOrEmpty(Id)) 
        {
            Debug.Log("아이디 입력");
            return;
        }
        else if (string.IsNullOrEmpty(pwd)) 
        {
            Debug.Log("비밀번호 입력");
            return;
        }
        else if (pwd != pwdCheck)
        {
            Debug.Log("이미지 등록");
            return;
        }

        // 회원가입 시도
        StartCoroutine(Managers.API.Join(
            name,
            Id,
            pwd,
            imgNo,
            (message) =>
            {
                // 회원가입 성공 시 처리
                Debug.Log(message);
                LoadScencLogin();
            },
            (error) =>
            {
                // 회원가입 실패 시 처리
                Debug.LogError($"회원가입 실패: {error}");
            }
        ));

    }

    private void LoadScencLogin()
    {
        Managers.Scene.LoadScene(Define.Scene.Login);
    }


    public override void Clear()
    {
        Debug.Log("JoingScene Clear!");
    }
}
