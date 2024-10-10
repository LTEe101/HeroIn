using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static PlayerController;

public class MuseumScene : BaseScene
{
    private Vector3 originalScale; // ��ü�� ���� Scale�� ������ ����
    public float highlightScaleFactor = 1.02f; // ���̶���Ʈ �� Ŀ�� ����

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Museum;
        if(!Managers.Data.ControlPannel)
        {
            Managers.UI.ShowPopupUI<UI_Control>();
        }
        // InputManager�� MouseAction�� �̺�Ʈ ���
        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;

        Managers.Sound.Play("UltimateBGM/Low Poly Ambience Loop", Define.Sound.Bgm);
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
        
        if (hoveredObject.CompareTag("InteractableButton"))
        {
            if (hoveredObject.name.Equals("Spaceship"))
            {
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(true);
            }
            ChangeOutlineColor(hoveredObject, Color.white);
            HighlightObject(hoveredObject);
        }
    }
    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;

        if (hoveredObject.CompareTag("InteractableButton"))
        {
            if (hoveredObject.name.Equals("Spaceship"))
            {
                Util.FindChild(hoveredObject, "UI_Enter").SetActive(false);
            }
            ChangeOutlineColor(hoveredObject, Color.black);
            ResetHighlight(hoveredObject);
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
    // ���콺 Ŭ�� �� ȣ��Ǵ� �Լ�
    void OnMouseClicked()
    {
       
        GameObject currentHovered = Managers.Input.HoveredObject;

        if (currentHovered != null)
        {
            string hitObjectName = currentHovered.name;
            Debug.Log(hitObjectName);
            UI_MuseumStory popup = null;
            switch (hitObjectName)
            {
                case "Spaceship":
                    Managers.Sound.Play("ProEffect/Doors/door_lock_close_01", Define.Sound.Effect, 0.8f);
                    Managers.Scene.LoadScene(Define.Scene.Home);
                    break;
                case "1592":
                    PlayButtonSound();
                    popup = Managers.UI.ShowPopupUI<UI_MuseumStory>();
                    popup.InitStory(Define.MuseumStoryType.Story1592);
                    break;
                case "1919":
                    PlayButtonSound();
                    popup = Managers.UI.ShowPopupUI<UI_MuseumStory>();
                    popup.InitStory(Define.MuseumStoryType.Story1919);
                    break;
                case "1443":
                    PlayButtonSound();
                    popup = Managers.UI.ShowPopupUI<UI_MuseumStory>();
                    popup.InitStory(Define.MuseumStoryType.Story1443);
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
    private void PlayButtonSound()
    {
        Managers.Sound.Play("UIClickEffect/Button/SFX_UI_Button_Organic_Plastic_Thin_Generic_1", Define.Sound.Effect, 1.0f);
    }
}
