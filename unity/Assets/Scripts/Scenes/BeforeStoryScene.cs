using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class BeforeStoryScene : BaseScene
{
    public CameraController _cameraController;
    private void Start()
    {
        _cameraController.onCloseUpComplete = OnCloseUpComplete;
       
    }
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.BeforeStory;
        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartCloseUp(new Vector3(-13.1f, 55.7f, -79.8f), new Vector3(34.9f, 26.5f, -59.3f),
            new Quaternion(-0.24f, -0.54f, 0.16f, -0.8f), new Quaternion(0.26f, -0.17f, 0.05f, 0.95f),
             1.0f);
        Managers.Input.MouseAction -= OnMouseAction;
        Managers.Input.MouseAction += OnMouseAction;
    }

    private void Update()
    {
    }

    public override void Clear()
    {
        Debug.Log("BeforeStoryScene Clear!");
    }
    void OnMouseAction(Define.MouseEvent mouseEvent)
    {
        switch (mouseEvent)
        {
            case Define.MouseEvent.Click:
                Debug.Log("마우스 클릭이 감지되었습니다.");
                OnMouseClicked();
                break;
            case Define.MouseEvent.PointerEnter:
                Debug.Log("마우스가 객체 위에 있습니다.");
                OnPointerEnter(Managers.Input.HoveredObject); // 현재 호버된 객체 밝게 하기
                break;
            case Define.MouseEvent.PointerExit:
                Debug.Log("마우스가 객체에서 벗어났습니다.");
                OnPointerExit(Managers.Input.HoveredObject); // 하이라이트 리셋
                break;
        }
    }
    void OnPointerEnter(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        Debug.Log(hoveredObject.name);
        switch (hoveredObject.name)
        {

        }
    }
    void OnPointerExit(GameObject hoveredObject)
    {
        if (hoveredObject == null) return;
        switch (hoveredObject.name)
        {

        }
    }
    void OnMouseClicked()
    {
        GameObject currentHovered = Managers.Input.HoveredObject;

        if (currentHovered != null)
        {
            string hitObjectName = currentHovered.name;
            Debug.Log(hitObjectName);
            switch (hitObjectName)
            {
            }
        }
    }
    void OnCloseUpComplete()
    {
        UI_Paper popup = Managers.UI.ShowPopupUI<UI_Paper>();
        popup.InitStory(Define.StoryType.Before);
    }
}
