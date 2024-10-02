using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;
using UnityEngine.EventSystems;

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
    public Button imageButton;
    private InputField[] inputFields;  // 입력 필드를 배열로 관리

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Join;
        BackButton.onClick.AddListener(OnBackButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        imageButton.onClick.AddListener(SelectAndUploadImage);  // 클릭 이벤트 설정
        inputFields = new InputField[] { nameInputField, IdInputField, pwdInputField, pwdCheckInputField };
    }

    private void Update()
    {
        // Tab 키를 눌렀을 때 다음 InputField로 이동
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnJoinButtonClicked();
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

    public void SelectAndUploadImage()
    {
        FileBrowser
        .SetFilters(true, new FileBrowser
        .Filter("Files", ".jpg", ".png")
        , new FileBrowser.Filter("Text Files", ".txt", ".pdf"));

        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Coroutine
        StartCoroutine(CorOpenImageFile());
    }

    IEnumerator CorOpenImageFile()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {
            var rawData = System.IO.File.ReadAllBytes(FileBrowser.Result[0]);
            Texture2D tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter
            tex.LoadImage(rawData);

            // ImgNom,버튼 크기 가져오기
            RectTransform imgNoRect = ImgNo.GetComponent<RectTransform>();
            RectTransform buttonRect = imageButton.GetComponent<RectTransform>();

            // 버튼의 크기를 ImgNo의 크기와 동일하게 설정
            buttonRect.sizeDelta = imgNoRect.sizeDelta - new Vector2(10, 10);

            // 원본 텍스처의 비율 계산
            float aspectRatio = (float)tex.width / tex.height;

            // ImgNo의 크기
            float imgNoWidth = imgNoRect.sizeDelta.x;
            float imgNoHeight = imgNoRect.sizeDelta.y;

            // 자를 영역 계산 (ImgNo 크기에 맞게)
            int cropWidth, cropHeight;
            int cropX = 0, cropY = 0;

            if (aspectRatio > 1) // 가로가 더 길면
            {
                // ImgNo의 너비에 맞춰 자르기 (높이는 유지)
                cropWidth = Mathf.FloorToInt(imgNoWidth * tex.height / imgNoHeight);
                cropHeight = tex.height;

                // 이미지 가운데를 자르기 위해 x 위치 설정
                cropX = (tex.width - cropWidth) / 2;
            }
            else // 세로가 더 길면
            {
                // ImgNo의 높이에 맞춰 자르기 (너비는 유지)
                cropHeight = Mathf.FloorToInt(imgNoHeight * tex.width / imgNoWidth);
                cropWidth = tex.width;

                // 이미지 가운데를 자르기 위해 y 위치 설정
                cropY = (tex.height - cropHeight) / 2;
            }

            // 새로운 텍스처 생성
            Texture2D croppedTex = new Texture2D(cropWidth, cropHeight);

            // 원본 텍스처에서 픽셀을 가져와서 잘라낸 부분을 새로운 텍스처에 적용
            Color[] pixels = tex.GetPixels(cropX, cropY, cropWidth, cropHeight);
            croppedTex.SetPixels(pixels);
            croppedTex.Apply(); // 변경 사항 적용

            // 자른 텍스처를 버튼에 설정
            imageButton.GetComponent<Image>().sprite = Sprite.Create(croppedTex, new Rect(0, 0, croppedTex.width, croppedTex.height), new Vector2(0.5f, 0.5f));

        }
    }

    private void OnBackButtonClicked()
    {
        LoadScencLogin();
    }

    // Join 버튼을 클릭했을 때 팝업 띄우기
    private void OnJoinButtonClicked()
    {
        string name = nameInputField.text;
        string Id = IdInputField.text;
        string pwd = pwdInputField.text;
        string pwdCheck = pwdCheckInputField.text;
        Image imgNo = imageButton.image;

        if (string.IsNullOrEmpty(name))
        {
            Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "이름을 입력하세요" });
            return;
        }
        else if (string.IsNullOrEmpty(Id))
        {
            Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "아이디를 입력하세요" });
            return;
        }
        else if (string.IsNullOrEmpty(pwd))
        {
            Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "비밀번호를 입력하세요" });
            return;
        }
        else if (pwd != pwdCheck)
        {
            Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "비밀번호가 달라요" });
            return;
        }

        // 팝업을 띄우고 확인 버튼 클릭 시 회원가입 진행
        var popup = Managers.UI.ShowPopupUI<UI_Check_Alert>(null, new object[] { "회원가입을 진행하시겠습니까?" });
        popup.OnCheck = OnJoinConfirmed; // 확인 버튼 클릭 시 OnJoinConfirmed 호출
    }

    // 팝업에서 확인 버튼을 눌렀을 때 회원가입을 진행하는 함수
    private void OnJoinConfirmed()
    {
        string name = nameInputField.text;
        string Id = IdInputField.text;
        string pwd = pwdInputField.text;
        Image imgNo = imageButton.image;

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
                Managers.UI.ShowPopupUI<UI_Alert>(null, new object[] { "이미 존재하는 아이디 입니다" });
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
