using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginScene : BaseScene
{
    // UI 요소를 변수로 선언
    public InputField userIdInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button joingButton;
    private InputField[] inputFields; // 입력 필드를 배열로 관리
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        joingButton.onClick.AddListener(OnJoinButtonClicked);

        inputFields = new InputField[] { userIdInputField, passwordInputField };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Login();
        }
    }

    // 다음 InputField로 이동하는 함수
    private void SelectNextInputField()
    {
        // 현재 선택된 UI 객체 확인
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected.GetComponent<InputField>() != null)
        {
            // 현재 선택된 InputField의 인덱스 찾기
            int currentIndex = System.Array.IndexOf(inputFields, currentSelected.GetComponent<InputField>());

            // 다음 InputField로 이동, 마지막 필드에서는 첫 번째 필드로 돌아감
            int nextIndex = (currentIndex + 1) % inputFields.Length;

            // 다음 InputField로 포커스 이동
            EventSystem.current.SetSelectedGameObject(inputFields[nextIndex].gameObject);
        }
    }

    public void Login()
    {
        string userId = userIdInputField.text;
        string password = passwordInputField.text;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            Debug.Log("아이디 또는 비밀번호 입력");
            Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "아이디 또는 비밀번호를 입력하세요" });
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
                     (user) => {
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

                 StartCoroutine(Managers.API.GetUserCardInfo(
                        userId,
                        () => {
                            // 카드 정보를 성공적으로 가져왔을 때 처리
                            Debug.Log("카드 정보를 성공적으로 가져왔습니다.");

                            // 가져온 카드 정보를 출력하거나 필요한 곳에 활용
                            foreach (var card in Managers.Data.cards)
                            {
                                Debug.Log($"카드 이름: {card.name}, 설명: {card.description}");
                            }
                        },
                        (error) => {
                            // 카드 정보를 가져오는 데 실패했을 때 처리
                            Debug.LogError($"카드 정보 가져오기 실패: {error}");
                        }
                    ));

                 StartCoroutine(Managers.API.GetUserTitleInfo(
                        userId,
                        () => {
                            // 카드 정보를 성공적으로 가져왔을 때 처리
                            Debug.Log("업적 정보를 성공적으로 가져왔습니다.");

                            // 가져온 카드 정보를 출력하거나 필요한 곳에 활용
                            foreach (var title in Managers.Data.titles)
                            {
                                Debug.Log($"업적 이름: {title.userTitle}");
                            }
                         
                        },
                        (error) => {
                            // 카드 정보를 가져오는 데 실패했을 때 처리
                            Debug.LogError($"업적 정보 가져오기 실패: {error}");
                        }
                    ));
             },
             (error) => {
                 // 로그인 실패 시 처리
                 Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "아이디 또는 비밀번호가 틀렸습니다" });
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
