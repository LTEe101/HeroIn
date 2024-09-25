using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MuseumScene : BaseScene
{
    private Vector3 originalScale; // ��ü�� ���� Scale�� ������ ����
    public float highlightScaleFactor = 1.02f; // ���̶���Ʈ �� Ŀ�� ����

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Museum;

        // InputManager�� MouseAction�� �̺�Ʈ ���
        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;
    }

    void Update()
    {
    }

    // ���콺 �Է� ó�� �޼���
    void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Click:
                Debug.Log("���콺 Ŭ���� �����Ǿ����ϴ�.");
                OnMouseClicked();
                break;
            case Define.MouseEvent.PointerEnter:
                Debug.Log("���콺�� ��ü ���� �ֽ��ϴ�.");
                OnPointerEnter(Managers.Input.HoveredObject); // ���� ȣ���� ��ü ��� �ϱ�
                break;
            case Define.MouseEvent.PointerExit:
                Debug.Log("���콺�� ��ü���� ������ϴ�.");
                OnPointerExit(Managers.Input.HoveredObject); // ���̶���Ʈ ����
                break;
        }
    }

    // ���̶���Ʈ�� ���� ���·� �����ϴ� �Լ�
    void ResetHighlight(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // ���� Scale�� ����
            hoveredObject.transform.localScale = originalScale;
            hoveredObject = null; // ���̶���Ʈ ��ü �ʱ�ȭ

        }
    }
    void ChangeOutlineColor(GameObject go, Color newColor)
    {
        // ������Ʈ���� Outline ������Ʈ�� ������
        Outline outline = go.GetComponent<Outline>();

        // Outline ������Ʈ�� ������ ���� ����
        if (outline != null)
        {
            // �ƿ����� �÷� ����
            outline.OutlineColor = newColor;
        }
        else
        {
            Debug.LogWarning("Outline ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void OnPointerEnter(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        Debug.Log(hoveredObject.name);
        switch (hoveredObject.name)
        {
            case "Spaceship":
                // UI_Enter Ȱ��ȭ
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(true);
               
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            case "1592":
                Debug.Log("test");
                HighlightObject(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.white);
                break;
            default:
                break;

        }
    }
    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        switch (hoveredObject.name)
        {
            case "Spaceship":
                // UI_Enter ��Ȱ��ȭ
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(false);
                
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            case "1592":
                ResetHighlight(hoveredObject);
                ChangeOutlineColor(hoveredObject, Color.black);
                break;
            default:
                break;

        }
    }
    // ���콺 ȣ�� �� ��ü�� Ű��� UI_Enter ����ϴ� �Լ�
    void HighlightObject(GameObject hoveredObject)
    {
        if (hoveredObject != null)
        {
            // ���� Scale ����
            originalScale = hoveredObject.transform.localScale;

            // ���̶���Ʈ Scale ����
            hoveredObject.transform.localScale = originalScale * highlightScaleFactor; // Scale ����
        }
    }

    // ���콺 Ŭ�� �� ȣ��Ǵ� �Լ�
    void OnMouseClicked()
    {
        GameObject currentHovered = Managers.Input.HoveredObject;

        if (currentHovered != null)
        {
            string hitObjectName = currentHovered.name;
            Debug.Log(hitObjectName);
            switch (hitObjectName)
            {
                case "Spaceship":
                    Managers.Scene.LoadScene(Define.Scene.Home);
                    break;
                case "1592":
                    Managers.UI.ShowPopupUI<UI_Museum_1592>();
                    break;
                default:
                    Debug.Log("�ش� ������Ʈ�� ���� �˾��� �����ϴ�.");
                    break;
            }
        }
    }

    public override void Clear()
    {
        // ���� ����� �� ȣ��Ǵ� �Լ�, �ʿ信 ���� �߰� �۾� ����
        Managers.Input.Clear(); // InputManager�� �̺�Ʈ�� Ŭ����
    }
}
