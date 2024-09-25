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
        Init();  // UI�� Ȱ��ȭ�� �� Init ȣ��
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
            fillImage.fillAmount = Mathf.Clamp01(amount); // ä��� ���� ����
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

            // ��ũ���� �߽��� (0, 0)�̶�� ����
            float halfScreenWidth = screenWidth / 2f;   // ���� ȭ���� ����
            float halfScreenHeight = screenHeight / 2f; // ���� ȭ���� ����

            // ���θ� 8���, ���θ� 4���
            float sectionWidth = screenWidth / 9f;
            float sectionHeight = screenHeight / 6f;

            // ù ��° y ��ġ (ȭ���� ���κ�)
            float yPosition = halfScreenHeight - sectionHeight; // ��ũ���� ��� 1��� ����

            switch (targetName)
            {
                case "TargetShip1":
                    // 4��° ��ġ (ȭ���� �����̹Ƿ� ����)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 3.5f, yPosition);
                    break;
                case "TargetShip2":
                    // 5��° ��ġ (�߾� ��ó, ���� ��)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 4.8f, yPosition);
                    break;
                case "TargetShip3":
                    // 6��° ��ġ (ȭ���� ����, ���)
                    newPosition = new Vector2(-halfScreenWidth + sectionWidth * 6.2f, yPosition);
                    break;
                default:
                    newPosition = new Vector2(0, 0); // �⺻ ��ġ
                    break;
            }

            // BarImage�� ��Ŀ�� ��ġ ����
            barImageTransform.anchoredPosition = newPosition; // anchoredPosition���� ����
            Debug.Log("BarImage anchoredPosition: " + barImageTransform.anchoredPosition);
        }
    }

}
