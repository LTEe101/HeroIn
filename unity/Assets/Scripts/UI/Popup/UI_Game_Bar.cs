using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Bar : UI_Popup
{
    enum Images
    {
        BarImage,
        FillImage
    }
    private Image fillImage;
    private RectTransform barImageTransform;
    void OnEnable()
    {
        Init();  // UI가 활성화될 때 Init 호출
    }
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        fillImage = Get<Image>((int)Images.FillImage);
        barImageTransform = Get<Image>((int)Images.BarImage).GetComponent<RectTransform>();

    }
    public void SetFillAmount(float amount)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(amount); // 채우기 비율 설정
        }
        else
        {
            Debug.Log("fillImage cannot find!");
        }
    }
    public void SetBarImagePosition(string targetName)
    {
        if (barImageTransform != null)
        {
            Vector2 newPosition;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // 스크린의 중심이 (0, 0)이라고 가정
            float halfScreenWidth = screenWidth / 2f;   // 가로 화면의 절반
            float halfScreenHeight = screenHeight / 2f; // 세로 화면의 절반

            // 가로를 8등분, 세로를 4등분
            float sectionWidth = screenWidth / 9f;
            float sectionHeight = screenHeight / 6f;

            // 첫 번째 y 위치 (화면의 윗부분)
            float yPosition = halfScreenHeight - sectionHeight; // 스크린의 상단 1등분 지점

            switch (targetName)
            {
                case "TargetShip1":
                    // 4번째 위치 (화면의 좌측이므로 음수)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 3.5f, yPosition);
                    break;
                case "TargetShip2":
                    // 5번째 위치 (중앙 근처, 좌측 끝)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 4.8f, yPosition);
                    break;
                case "TargetShip3":
                    // 6번째 위치 (화면의 우측, 양수)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 6.2f, yPosition);
                    break;
                default:
                    newPosition = new Vector2(0, 0); // 기본 위치
                    break;
            }

            // BarImage의 앵커와 위치 설정
            barImageTransform.anchoredPosition = newPosition; // anchoredPosition으로 설정
            Debug.Log("BarImage anchoredPosition: " + barImageTransform.anchoredPosition);
        }
    }

}
